using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;

namespace DailyProcessingJobs
{
    public static class WorkbookHelper
    {
        private static SpreadsheetDocument CreateWorkbook(string path)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                Create(path, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild(new Sheets());

            return spreadsheetDocument;
        }


        public static bool IsWorksheetPresent(WorkbookPart workbookPart, string sheetName)
        {
            var sheets = workbookPart.Workbook.Sheets;

            // For each sheet, display the sheet information.
            return sheets.
                Select(element => element as Sheet).
                Any(sheet => sheet != null && sheet.Name == sheetName);
        }

        public static void DeleteWorksheet(WorkbookPart workbookPart, string sheetName)
        {
            //string sheetid = "";

            // Get the pivot Table Parts
            IEnumerable<PivotTableCacheDefinitionPart> pvtTableCacheParts = workbookPart.PivotTableCacheDefinitionParts;
            var pvtTableCacheDefinationPart = 
                (from pivotTableCacheDefinitionPart in pvtTableCacheParts 
                 let pvtCacheDef = pivotTableCacheDefinitionPart.PivotCacheDefinition 
                 let pvtCache = pvtCacheDef.Descendants<CacheSource>().
                 Where(s => s.WorksheetSource.Sheet == sheetName) 
                 where pvtCache.Count() > 0 select pivotTableCacheDefinitionPart).
                 ToDictionary(pivotTableCacheDefinitionPart => pivotTableCacheDefinitionPart, pivotTableCacheDefinitionPart => pivotTableCacheDefinitionPart.ToString());

            foreach (var item in pvtTableCacheDefinationPart)
            {
                workbookPart.DeletePart(item.Key);
            }

            //Get the SheetToDelete from workbook.xml
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();
            if (sheet == null)
            {
                // The specified sheet doesn't exist.
                return;
            }

            //Store the SheetID for the reference
            var sheetid = sheet.SheetId;

            // Remove the sheet reference from the workbook.
            var worksheetPart = (WorksheetPart)(workbookPart.GetPartById(sheet.Id));
            sheet.Remove();

            // Delete the worksheet part.
            workbookPart.DeletePart(worksheetPart);

            //Get the DefinedNames
            var definedNames = workbookPart.Workbook.Descendants<DefinedNames>().FirstOrDefault();
            if (definedNames != null)
            {
                foreach (DefinedName item in definedNames)
                {
                    // This condition checks to delete only those names which are part of Sheet in question
                    if (item.Text.Contains(sheetName + "!"))
                        item.Remove();
                }
            }

            // Get the CalculationChainPart 
            //Note: An instance of this part type contains an ordered set of references to all cells in all worksheets in the 
            //workbook whose value is calculated from any formula

            CalculationChainPart calChainPart = workbookPart.CalculationChainPart;
            if (calChainPart != null)
            {
                var calChainEntries = calChainPart.CalculationChain.Descendants<CalculationCell>().Where(c => (uint)c.SheetId.Value == sheetid);
                foreach (CalculationCell item in calChainEntries)
                {
                    item.Remove();
                }

                if (calChainPart.CalculationChain.Count() == 0)
                {
                    workbookPart.DeletePart(calChainPart);
                }
            }

        }

        //private static void AddTextToWorkSheet(string path, string sheetName, string text, string columnName, uint rowIndex)
        //{
        //    // Open the document for editing.
        //    using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(path, true))
        //    {
        //        // Get the SharedStringTablePart. If it does not exist, create a new one.
        //        SharedStringTablePart shareStringPart;
        //        if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
        //        {
        //            shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
        //        }
        //        else
        //        {
        //            shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
        //        }

        //        // Insert the text into the SharedStringTablePart.
        //        int index = InsertSharedStringItem(spreadSheet.WorkbookPart, text);

        //        // Insert a new worksheet.
        //        WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart, sheetName);

        //        // Insert cell A1 into the new worksheet.
        //        Cell cell = InsertCellInWorksheet(worksheetPart, columnName, rowIndex);

        //        // Set the value of cell A1.
        //        cell.CellValue = new CellValue(index.ToString());
        //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        //        // Save the new worksheet.
        //        worksheetPart.Worksheet.Save();
        //    }
        //}

        public static WorksheetPart InsertWorksheet(WorkbookPart workbookPart, string sheetName)
        {
            Sheet foundSheet = workbookPart.Workbook.Sheets.Cast<Sheet>().FirstOrDefault(sheet => sheet.Name == sheetName);

            if (foundSheet == null)
            {
                // Add a new worksheet part to the workbook.
                var newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new Worksheet(new SheetData());
                newWorksheetPart.Worksheet.Save();

                var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

                // Get a unique ID for the new sheet.
                uint sheetId = 1;
                if (sheets.Elements<Sheet>().Count() > 0)
                {
                    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                // Append the new worksheet and associate it with the workbook.
                var sheet = new Sheet { Id = relationshipId, SheetId = sheetId, Name = sheetName };
                sheets.Append((IEnumerable<OpenXmlElement>)sheet);
                return newWorksheetPart;
            }

            return (WorksheetPart)workbookPart.GetPartById(foundSheet.Id);

        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(WorksheetPart worksheetPart, string columnName, uint rowIndex)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row {RowIndex = rowIndex};
                sheetData.Append((IEnumerable<OpenXmlElement>) row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }

            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            var refCell =
                row.Elements<Cell>().FirstOrDefault(
                    cell => string.Compare(cell.CellReference.Value, cellReference, true) > 0);
            var newCell = new Cell {CellReference = cellReference};
            row.InsertBefore(newCell, refCell);

            return newCell;
        }


        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(WorkbookPart workbookPart, string text)
        {
            SharedStringTablePart shareStringPart = GetSharedStringPart(workbookPart);

            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));

            return i;
        }

        private static string GetColumnIdentifier(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = ((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static void AddCellText(WorkbookPart workbookPart,
                                        WorksheetPart worksheetPart, 
                                        string text, 
                                        uint colIndex, 
                                        uint rowIndex)
        {
            string columnName = GetColumnIdentifier((int)colIndex);

            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(workbookPart, text);

            // Insert cell A1 into the new worksheet.
            Cell cell = InsertCellInWorksheet(worksheetPart, columnName, rowIndex);

            // Set the value of cell A1.
            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        }

        private static SharedStringTablePart GetSharedStringPart(WorkbookPart workbookPart)
        {
            return workbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0 ? workbookPart.GetPartsOfType<SharedStringTablePart>().First() : workbookPart.AddNewPart<SharedStringTablePart>();
        }

        // Adds child parts and generates content of the specified part.
        public static void CreateTable(WorkbookPart workbookPart, WorksheetPart worksheetPart, string[] columns, int topLeftColumn, int topLeftRow, int width, int height)
        {
            List<WorksheetPart> worksheets = workbookPart.GetPartsOfType<WorksheetPart>().ToList();
            uint maxTableId = worksheets.Select(ws => ws.TableDefinitionParts.ToList()).SelectMany(tableDefinitions => tableDefinitions).Aggregate<TableDefinitionPart, uint>(0, (current, tableDef) => Math.Max(tableDef.Table.Id, current));

            uint tableId = maxTableId + 1;

            var tables = new TableParts { Count = 1U };
            worksheetPart.Worksheet.Append((IEnumerable<OpenXmlElement>)tables);

            var newTableDefnPart = worksheetPart.AddNewPart<TableDefinitionPart>();
            string relationshipId = worksheetPart.GetIdOfPart(newTableDefnPart);

            string cellReference = string.Format("{0}{1}:{2}{3}", GetColumnIdentifier(topLeftColumn), topLeftRow, GetColumnIdentifier(topLeftColumn + width - 1), topLeftRow + height);
            var table1 = new Table { Id = tableId, Name = "Table" + relationshipId, DisplayName = "Table" + relationshipId, Reference = cellReference, TotalsRowShown = false };
            var autoFilter1 = new AutoFilter { Reference = cellReference };

            var tableColumns1 = new TableColumns { Count = (uint)columns.Length };
            for (int iColumn = 0; iColumn < columns.Length; iColumn++)
            {
                var tableColumn = new TableColumn { Id = (UInt32Value)(uint)iColumn + 1, Name = columns[iColumn] };
                tableColumns1.Append((IEnumerable<OpenXmlElement>)tableColumn);
            }
            var tableStyleInfo1 = new TableStyleInfo { Name = "TableStyleMedium2", ShowFirstColumn = false, ShowLastColumn = false, ShowRowStripes = true, ShowColumnStripes = false };

            table1.Append((IEnumerable<OpenXmlElement>)autoFilter1);
            table1.Append((IEnumerable<OpenXmlElement>)tableColumns1);
            table1.Append((IEnumerable<OpenXmlElement>)tableStyleInfo1);

            newTableDefnPart.Table = table1;

            var table = new TablePart { Id = relationshipId };
            tables.Append((IEnumerable<OpenXmlElement>)table);

            //TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };
            //worksheetPart.Worksheet.Append(tableStyles1);
        }

        public static SpreadsheetDocument OpenOrCreateWorkbook(string workbookPath)
        {
            if (File.Exists(workbookPath))
                return SpreadsheetDocument.Open(workbookPath, true);

            return CreateWorkbook(workbookPath);
        }
    }
}
