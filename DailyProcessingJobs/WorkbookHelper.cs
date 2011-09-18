using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data.SqlClient;
using DailyProcessingJobs.Model;
using System.IO;

namespace DailyProcessingJobs
{
    public static class WorkbookHelper
    {
        private static SpreadsheetDocument CreateWorkbook(string path, string worksheet)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                Create(path, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            //// Append a new worksheet and associate it with the workbook.
            //Sheet sheet = new Sheet()
            //{
            //    Id = spreadsheetDocument.WorkbookPart.
            //        GetIdOfPart(worksheetPart),
            //    SheetId = 1,
            //    Name = worksheet
            //};
            //sheets.Append(sheet);

            return spreadsheetDocument;
        }


        public static bool IsWorksheetPresent(WorkbookPart workbookPart, string sheetName)
        {
            var sheets = workbookPart.Workbook.Sheets;

            // For each sheet, display the sheet information.
            foreach (var element in sheets)
            {
                Sheet sheet = element as Sheet;
                if (sheet.Name == sheetName)
                    return true;
            }

            return false;
        }

        public static void DeleteWorksheet(WorkbookPart workbookPart, string sheetName)
        {
            string Sheetid = "";

            // Get the pivot Table Parts
            IEnumerable<PivotTableCacheDefinitionPart> pvtTableCacheParts = workbookPart.PivotTableCacheDefinitionParts;
            Dictionary<PivotTableCacheDefinitionPart, string> pvtTableCacheDefinationPart = new Dictionary<PivotTableCacheDefinitionPart, string>();
            foreach (PivotTableCacheDefinitionPart Item in pvtTableCacheParts)
            {
                PivotCacheDefinition pvtCacheDef = Item.PivotCacheDefinition;
                //Check if this CacheSource is linked to SheetToDelete
                var pvtCahce = pvtCacheDef.Descendants<CacheSource>().Where(s => s.WorksheetSource.Sheet == sheetName);
                if (pvtCahce.Count() > 0)
                {
                    pvtTableCacheDefinationPart.Add(Item, Item.ToString());
                }
            }
            foreach (var Item in pvtTableCacheDefinationPart)
            {
                workbookPart.DeletePart(Item.Key);
            }

            //Get the SheetToDelete from workbook.xml
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();
            if (sheet == null)
            {
                // The specified sheet doesn't exist.
                return;
            }

            //Store the SheetID for the reference
            Sheetid = sheet.SheetId;

            // Remove the sheet reference from the workbook.
            WorksheetPart worksheetPart = (WorksheetPart)(workbookPart.GetPartById(sheet.Id));
            sheet.Remove();

            // Delete the worksheet part.
            workbookPart.DeletePart(worksheetPart);

            //Get the DefinedNames
            var definedNames = workbookPart.Workbook.Descendants<DefinedNames>().FirstOrDefault();
            if (definedNames != null)
            {
                foreach (DefinedName Item in definedNames)
                {
                    // This condition checks to delete only those names which are part of Sheet in question
                    if (Item.Text.Contains(sheetName + "!"))
                        Item.Remove();
                }
            }

            // Get the CalculationChainPart 
            //Note: An instance of this part type contains an ordered set of references to all cells in all worksheets in the 
            //workbook whose value is calculated from any formula

            CalculationChainPart calChainPart;
            calChainPart = workbookPart.CalculationChainPart;
            if (calChainPart != null)
            {
                var calChainEntries = calChainPart.CalculationChain.Descendants<CalculationCell>().Where(c => c.SheetId == Sheetid);
                foreach (CalculationCell Item in calChainEntries)
                {
                    Item.Remove();
                }
                if (calChainPart.CalculationChain.Count() == 0)
                {
                    workbookPart.DeletePart(calChainPart);
                }
            }

        }

        private static void AddTextToWorkSheet(string path, string sheetName, string text, string columnName, uint rowIndex)
        {
            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(path, true))
            {
                // Get the SharedStringTablePart. If it does not exist, create a new one.
                SharedStringTablePart shareStringPart;
                if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                // Insert the text into the SharedStringTablePart.
                int index = InsertSharedStringItem(spreadSheet.WorkbookPart, text);

                // Insert a new worksheet.
                WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart, sheetName);

                // Insert cell A1 into the new worksheet.
                Cell cell = InsertCellInWorksheet(worksheetPart, columnName, rowIndex);

                // Set the value of cell A1.
                cell.CellValue = new CellValue(index.ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                // Save the new worksheet.
                worksheetPart.Worksheet.Save();
            }
        }

        public static WorksheetPart InsertWorksheet(WorkbookPart workbookPart, string sheetName)
        {
            Sheet foundSheet = null;
            foreach (Sheet sheet in workbookPart.Workbook.Sheets)
            {
                if (sheet.Name == sheetName)
                {
                    foundSheet = sheet;
                    break;
                }
            }

            if (foundSheet == null)
            {
                // Add a new worksheet part to the workbook.
                WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new Worksheet(new SheetData());
                newWorksheetPart.Worksheet.Save();

                Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

                // Get a unique ID for the new sheet.
                uint sheetId = 1;
                if (sheets.Elements<Sheet>().Count() > 0)
                {
                    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                // Append the new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
                sheets.Append(sheet);
                return newWorksheetPart;
            }
            else
            {
                return (WorksheetPart)workbookPart.GetPartById(foundSheet.Id);
            }

        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(WorksheetPart worksheetPart, string columnName, uint rowIndex)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }
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
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));

            return i;
        }

        private static string GetColumnIdentifier(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
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
            SharedStringTablePart shareStringPart;
            if (workbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
            {
                shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                shareStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
            }

            return shareStringPart;
        }

        // Adds child parts and generates content of the specified part.
        public static void CreateTable(WorkbookPart workbookPart, WorksheetPart worksheetPart, string[] columns, int topLeftColumn, int topLeftRow, int width, int height)
        {
            uint maxTableId = 0;

            List<WorksheetPart> worksheets = workbookPart.GetPartsOfType<WorksheetPart>().ToList();
            foreach (WorksheetPart ws in worksheets)
            {
                // get table parts
                List<TableDefinitionPart> tableDefinitions = ws.TableDefinitionParts.ToList();
                foreach (TableDefinitionPart tableDef in tableDefinitions)
                {
                    maxTableId = Math.Max(tableDef.Table.Id, maxTableId);
                }
            }

            uint tableId = maxTableId + 1;

            TableParts tables = new TableParts() { Count = (UInt32Value)1U };
            worksheetPart.Worksheet.Append(tables);

            TableDefinitionPart newTableDefnPart = worksheetPart.AddNewPart<TableDefinitionPart>();
            string relationshipId = worksheetPart.GetIdOfPart(newTableDefnPart);

            string cellReference = string.Format("{0}{1}:{2}{3}", GetColumnIdentifier(topLeftColumn), topLeftRow, GetColumnIdentifier(topLeftColumn + width - 1), topLeftRow + height);
            Table table1 = new Table() { Id = tableId, Name = "Table" + relationshipId, DisplayName = "Table" + relationshipId, Reference = cellReference, TotalsRowShown = false };
            AutoFilter autoFilter1 = new AutoFilter() { Reference = cellReference };

            TableColumns tableColumns1 = new TableColumns() { Count = (UInt32Value)(uint)columns.Length };
            for (int iColumn = 0; iColumn < columns.Length; iColumn++)
            {
                TableColumn tableColumn = new TableColumn() { Id = (UInt32Value)(uint)iColumn + 1, Name = columns[iColumn] };
                tableColumns1.Append(tableColumn);
            }
            TableStyleInfo tableStyleInfo1 = new TableStyleInfo() { Name = "TableStyleMedium2", ShowFirstColumn = false, ShowLastColumn = false, ShowRowStripes = true, ShowColumnStripes = false };

            table1.Append(autoFilter1);
            table1.Append(tableColumns1);
            table1.Append(tableStyleInfo1);

            newTableDefnPart.Table = table1;

            TablePart table = new TablePart() { Id = relationshipId };
            tables.Append(table);

            //TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };
            //worksheetPart.Worksheet.Append(tableStyles1);
        }

        public static SpreadsheetDocument OpenOrCreateWorkbook(string workbookPath, string defaultWorksheet)
        {
            if( File.Exists(workbookPath))
                return SpreadsheetDocument.Open(workbookPath, true);
            else
                return CreateWorkbook(workbookPath, defaultWorksheet);
        }
    }
}
