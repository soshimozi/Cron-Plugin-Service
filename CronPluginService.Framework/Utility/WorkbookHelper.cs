using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data.SqlClient;

namespace CronPluginService.Framework.Utility
{
    public static class WorkbookHelper
    {
        public static void InitializeWorkbook(string path)
        {
            CreateWorkbook(path, "Default Sheet");
        }

        public static void CreateWorkbook(string path, string worksheetName)
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

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = worksheetName
            };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }


        public static bool IsWorksheetPresent(string path, string sheetName)
        {
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(path, false))
            {
                var sheets = spreadSheetDocument.WorkbookPart.Workbook.Sheets;

                // For each sheet, display the sheet information.
                foreach (var element in sheets)
                {
                    Sheet sheet = element as Sheet;
                    if (sheet.Name == sheetName)
                        return true;
                }

                return false;
            }
        }

        public static void DeleteWorksheet(string path, string sheetName)
        {
            string Sheetid = "";
            //Open the workbook
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(path, true))
            {
                WorkbookPart wbPart = document.WorkbookPart;

                // Get the pivot Table Parts
                IEnumerable<PivotTableCacheDefinitionPart> pvtTableCacheParts = wbPart.PivotTableCacheDefinitionParts;
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
                    wbPart.DeletePart(Item.Key);
                }
                //Get the SheetToDelete from workbook.xml
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();
                if (theSheet == null)
                {
                    // The specified sheet doesn't exist.
                }
                //Store the SheetID for the reference
                Sheetid = theSheet.SheetId;

                // Remove the sheet reference from the workbook.
                WorksheetPart worksheetPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
                theSheet.Remove();

                // Delete the worksheet part.
                wbPart.DeletePart(worksheetPart);

                //Get the DefinedNames
                var definedNames = wbPart.Workbook.Descendants<DefinedNames>().FirstOrDefault();
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
                calChainPart = wbPart.CalculationChainPart;
                if (calChainPart != null)
                {
                    var calChainEntries = calChainPart.CalculationChain.Descendants<CalculationCell>().Where(c => c.SheetId == Sheetid);
                    foreach (CalculationCell Item in calChainEntries)
                    {
                        Item.Remove();
                    }
                    if (calChainPart.CalculationChain.Count() == 0)
                    {
                        wbPart.DeletePart(calChainPart);
                    }
                }

                // Save the workbook.
                wbPart.Workbook.Save();
            }
        }

        public static void AddColumnHeadersFromSqlDataReader(string path, string sheetName, SqlDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);

                AddTextToWorkSheet(path, sheetName, fieldName, GetColumnName(i + 1), 1);
            }
        }

        public static void AddRowToWorksheetFromSqlDataReader(string path, string sheetName, SqlDataReader reader, uint rowIndex)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                AddTextToWorkSheet(path, sheetName, reader[i].ToString(), GetColumnName(i + 1), rowIndex);
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
                int index = InsertSharedStringItem(text, shareStringPart);

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

        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart, string sheetName)
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
                workbookPart.Workbook.Save();
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

                worksheet.Save();
                return newCell;
            }
        }
        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
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
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        private static string GetColumnName(int columnNumber)
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

    }
}
