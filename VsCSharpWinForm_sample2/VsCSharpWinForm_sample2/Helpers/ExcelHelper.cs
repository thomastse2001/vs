using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DocumentFormat.OpenXml;/// For OpenXml.
//using DocumentFormat.OpenXml.Spreadsheet;/// For OpenXml.
//using DocumentFormat.OpenXml.Packaging;/// For OpenXml.
using System.Data;
using System.Data.OleDb;
//using ClosedXML.Excel;/// For ClosedXML.
//using OfficeOpenXml;/// For EPPlus.

namespace VsCSharpWinForm_sample2.Helpers
{
    public class ExcelHelper
    {
        /// Updated date: 2020-08-86
        public static TLog Logger { get; set; }

        ///// Need to add reference "Microsoft Excel XX Object Library" into the project.
        ///// 1. Click "Project > Add Reference...".
        ///// 2. Go to the "COM" page. Select "Microsoft Excel XX Object Library". Click the OK button.
        ///// https://docs.microsoft.com/en-us/previous-versions/office/troubleshoot/office-developer/automate-excel-from-visual-c
        ///// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/interop/how-to-access-office-onterop-objects
        ///// http://csharp.net-informations.com/excel/csharp-create-excel.htm
        ///// https://stackoverflow.com/questions/2663212/using-microsoft-office-interop-to-save-created-file-with-c-sharp
        //public class OfficeInterop
        //{
        //    public static bool ExportFile(string path)
        //    {
        //        Microsoft.Office.Interop.Excel.Application xlApp = null;
        //        Microsoft.Office.Interop.Excel.Workbook xlWorkbook = null;
        //        try
        //        {
        //            xlApp = new Microsoft.Office.Interop.Excel.Application();
        //            if (xlApp == null) { throw new Exception("EXCEL cannot start. Check file Microsoft.Office.Interop.Excel.dll."); }
        //            object misValue = System.Reflection.Missing.Value;
        //            xlWorkbook = xlApp.Workbooks.Add(misValue);
        //            Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkbook?.ActiveSheet;
        //            if (xlWorksheet == null) { throw new Exception("Excel WorkSheet is NULL."); }
        //            /// Values.
        //            xlWorksheet.Cells[1, 2] = "ABCDEF";
        //            xlWorksheet.Cells[1, 3] = "FFFF";
        //            xlWorksheet.Cells[1, 4] = "Total";
        //            xlWorksheet.Cells[2, 2] = "10.45";
        //            xlWorksheet.Cells[2, 3] = "3";
        //            xlWorksheet.Cells[3, 2] = "52.10";
        //            xlWorksheet.Cells[3, 3] = "2";
        //            xlWorksheet.Cells[4, 2] = "15.03";
        //            xlWorksheet.Cells[4, 3] = "4";
        //            xlWorksheet.Cells[2, 4].Formula = "=B2*C2";
        //            xlWorksheet.Cells[3, 4].Formula = "=B3*C3";
        //            xlWorksheet.Cells[4, 4].Formula = "=B4*C4";
        //            /// Format.
        //            xlWorksheet.Range[xlWorksheet.Cells[1, 2], xlWorksheet.Cells[1, 4]].Font.Bold = true;
        //            /// Save.
        //            xlWorkbook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
        //            xlWorkbook.Close(true, misValue, misValue);
        //            xlApp.Quit();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger?.Error("Path = {0}", path);
        //            Logger?.Error(ex);
        //            return false;
        //        }
        //        finally
        //        {
        //            if (xlWorkbook != null)
        //            {
        //                try { System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook); }
        //                catch (Exception ex2) { Logger?.Error(ex2); }
        //                xlWorkbook = null;
        //            }
        //            if (xlApp != null)
        //            {
        //                try { System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp); }
        //                catch (Exception ex2) { Logger?.Error(ex2); }
        //                xlApp = null;
        //            }
        //        }
        //    }
        //
        //    /// Return value = Error message if fail. Return null if success.
        //    /// If sheetName is empty, go to the first sheet.
        //    /// https://stackoverflow.com/questions/12255166/c-sharp-and-excel-deleting-rows
        //    /// http://csharp.net-informations.com/excel/csharp-read-excel.htm
        //    public static string RemoveRows(string path, string sheetName)
        //    {
        //        Microsoft.Office.Interop.Excel.Application xlApp = null;
        //        Microsoft.Office.Interop.Excel.Workbook xlWorkbook = null;
        //        try
        //        {
        //            if (!System.IO.File.Exists(path))
        //                return "Cannot find file";
        //            xlApp = new Microsoft.Office.Interop.Excel.Application() { Visible = false };
        //            xlWorkbook = xlApp.Workbooks.Open(path);
        //            Microsoft.Office.Interop.Excel.Worksheet xlSheet = string.IsNullOrWhiteSpace(sheetName) ?
        //                (Microsoft.Office.Interop.Excel.Worksheet)xlWorkbook.Worksheets[1] :
        //                (Microsoft.Office.Interop.Excel.Worksheet)xlWorkbook.Worksheets[sheetName];
        //            string a1String = (string)(xlSheet.Cells[1, 1] as Microsoft.Office.Interop.Excel.Range).Value;
        //            string a2String = (string)(xlSheet.Cells[2, 1] as Microsoft.Office.Interop.Excel.Range).Value;
        //            bool shouldSave = false;
        //            if (string.IsNullOrWhiteSpace(a2String))
        //            {
        //                ((Microsoft.Office.Interop.Excel.Range)xlSheet.Rows[2, Type.Missing]).EntireRow.Delete(Microsoft.Office.Interop.Excel.XlDeleteShiftDirection.xlShiftUp);
        //                shouldSave = true;
        //            }
        //            if (string.Equals("Query for ELMS Upload", a1String, StringComparison.OrdinalIgnoreCase))
        //            {
        //                ((Microsoft.Office.Interop.Excel.Range)xlSheet.Rows[1, Type.Missing]).EntireRow.Delete(Microsoft.Office.Interop.Excel.XlDeleteShiftDirection.xlShiftUp);
        //                shouldSave = true;
        //            }
        //            if (shouldSave)
        //                xlWorkbook.Save();
        //            return null;
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //        }
        //        finally
        //        {
        //            if (xlWorkbook != null)
        //            {
        //                try { xlWorkbook.Close(); System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook); }
        //                catch (Exception ex2) { Logger?.Error(ex2); }
        //                xlWorkbook = null;
        //            }
        //            if (xlApp != null)
        //            {
        //                try { xlApp.Quit(); System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp); }
        //                catch (Exception ex2) { Logger?.Error(ex2); }
        //                xlApp = null;
        //            }
        //        }
        //    }
        //}

        ///// Need to add "DocumentFormat.OpenXml" in NuGet Package Manager.
        ///// https://www.c-sharpcorner.com/article/creating-excel-file-using-openxml/
        ///// http://www.dispatchertimer.com/tutorial/how-to-create-an-excel-file-in-net-using-openxml-part-1-basics/
        ///// http://www.dispatchertimer.com/tutorial/how-to-create-an-excel-file-in-net-using-openxml-part-3-add-stylesheet-to-the-spreadsheet/
        ///// https://docs.microsoft.com/en-us/office/open-xml/how-to-insert-text-into-a-cell-in-a-spreadsheet
        //public class OpenXml
        //{
        //    /// Cell Style Index.
        //    public static class CellStyleIndex
        //    {
        //        public static UInt32Value Default = 0;
        //        public static UInt32Value TableBody = 1;
        //        public static UInt32Value TableHeader = 2;
        //        public static UInt32Value Title = 3;
        //    }

        //    private static readonly CellValues CellValueOfString = CellValues.SharedString;

        //    /// http://www.dispatchertimer.com/tutorial/how-to-create-an-excel-file-in-net-using-openxml-part-3-add-stylesheet-to-the-spreadsheet/
        //    private static Stylesheet GenerateStyleSheet()
        //    {
        //        Stylesheet rStyleSheet = null;
        //        try
        //        {
        //            Fonts fonts = new Fonts(
        //                new Font(/// Index 0 default.
        //                    new FontName() { Val = "Times New Roman" },
        //                    new FontSize() { Val = 12 }
        //                    ),
        //                new Font(/// Index 1 - table header.
        //                    new Bold(),
        //                    new FontName() { Val = "Times New Roman" },
        //                    new FontSize() { Val = 10 }
        //                    ),
        //                new Font(/// Index 2 - Title.
        //                    new Bold(),
        //                    new FontName() { Val = "Times New Roman" },
        //                    new FontSize() { Val = 20 }
        //                    )
        //                );

        //            Fills fills = new Fills(
        //                new Fill(new PatternFill { PatternType = PatternValues.None })/// Index 0 default.
        //                );

        //            Borders borders = new Borders(
        //                new Border(),/// index 0 default.
        //                new Border(/// index 1 black border.
        //                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
        //                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
        //                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
        //                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
        //                    new DiagonalBorder()
        //                    )
        //                );

        //            Alignment alignCenter = new Alignment() { Horizontal = HorizontalAlignmentValues.Center };

        //            CellFormats cellFormats = new CellFormats(
        //                new CellFormat() { FormatId = CellStyleIndex.Default },/// default.
        //                new CellFormat() { FormatId = CellStyleIndex.TableBody, FontId = 0, ApplyFont = true, FillId = 0, ApplyFill = true, BorderId = 1, ApplyBorder = true },// table body.
        //                new CellFormat() { FormatId = CellStyleIndex.TableHeader, FontId = 1, ApplyFont = true, FillId = 0, ApplyFill = true, BorderId = 1, ApplyBorder = true, Alignment = alignCenter, ApplyAlignment = true },// table header.
        //                new CellFormat() { FormatId = CellStyleIndex.Title, FontId = 2, ApplyFont = true, FillId = 0, ApplyFill = true, BorderId = 0, ApplyBorder = true }// title.
        //                );

        //            rStyleSheet = new Stylesheet(fonts, fills, borders, cellFormats);
        //        }
        //        catch (Exception ex) { Logger?.Error(ex); }
        //        return rStyleSheet;
        //    }

        //    /// https://docs.microsoft.com/en-us/office/open-xml/how-to-insert-text-into-a-cell-in-a-spreadsheet
        //    private static SharedStringTablePart GetSharedStringTablePart(SpreadsheetDocument oDoc)
        //    {
        //        try
        //        {
        //            if (oDoc?.WorkbookPart == null) { return null; }
        //            if (oDoc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
        //            {
        //                return oDoc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
        //            }
        //            else
        //            {
        //                return oDoc.WorkbookPart.AddNewPart<SharedStringTablePart>();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger?.Error(ex);
        //            return null;
        //        }
        //    }

        //    /// https://docs.microsoft.com/en-us/office/open-xml/how-to-insert-text-into-a-cell-in-a-spreadsheet
        //    /// Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet.
        //    /// If the cell already exists, returns it.
        //    private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        //    {
        //        try
        //        {
        //            if (worksheetPart == null) { return null; }
        //            Worksheet worksheet = worksheetPart.Worksheet;
        //            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
        //            string cellReference = columnName + rowIndex;
        //            /// If the worksheet does not contain a row with the specified row index, insert one.
        //            Row row;
        //            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
        //            {
        //                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        //            }
        //            else
        //            {
        //                row = new Row() { RowIndex = rowIndex };
        //                sheetData.Append(row);
        //            }
        //            /// If there is not a cell with the specified column name, insert one.
        //            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
        //            {
        //                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
        //            }
        //            else
        //            {
        //                /// Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
        //                Cell refCell = null;
        //                foreach (Cell cell in row.Elements<Cell>())
        //                {
        //                    if (cell.CellReference.Value.Length == cellReference.Length)
        //                    {
        //                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
        //                        {
        //                            refCell = cell;
        //                            break;
        //                        }
        //                    }
        //                }
        //                Cell newCell = new Cell() { CellReference = cellReference };
        //                row.InsertBefore(newCell, refCell);
        //                worksheet.Save();
        //                return newCell;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger?.Error(ex);
        //            return null;
        //        }
        //    }

        //    /// Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        //    /// and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        //    /// https://docs.microsoft.com/en-us/office/open-xml/how-to-insert-text-into-a-cell-in-a-spreadsheet
        //    private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        //    {
        //        try
        //        {
        //            /// If the part does not contain a SharedStringTable, create one.
        //            if (shareStringPart.SharedStringTable == null) { shareStringPart.SharedStringTable = new SharedStringTable(); }
        //            int i = 0;
        //            /// Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
        //            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
        //            {
        //                if (item.InnerText == text) { return i; }
        //                i++;
        //            }
        //            /// The text does not exist in the part. Create the SharedStringItem and return its index.
        //            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
        //            shareStringPart.SharedStringTable.Save();
        //            return i;
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger?.Error(ex);
        //            throw ex;
        //        }
        //    }

        //    ///// https://docs.microsoft.com/en-us/office/open-xml/how-to-insert-text-into-a-cell-in-a-spreadsheet
        //    //private static int InsertSharedStringItemByWorksheetPart(string text, WorksheetPart worksheetPart)
        //    //{
        //    //    try
        //    //    {
        //    //        if (worksheetPart == null) { return -1; }
        //    //        /// Get the SharedStringTablePart. If it does not exist, create a new one.
        //    //        //SharedStringTablePart shareStringPart;
        //    //        //if (worksheetPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
        //    //        //{
        //    //        //    shareStringPart = worksheetPart.GetPartsOfType<SharedStringTablePart>().First();
        //    //        //}
        //    //        //else
        //    //        //{
        //    //        //    shareStringPart = worksheetPart.AddNewPart<SharedStringTablePart>();
        //    //        //}
        //    //        if (worksheetPart.GetPartsOfType<SharedStringTablePart>().Count() < 1)
        //    //        {
        //    //            throw new Exception(string.Format("InsertSharedStringItemByWorksheetPart cannot find SharedStringTablePart. Text = {0}", text));
        //    //        }
        //    //        SharedStringTablePart shareStringPart = worksheetPart.GetPartsOfType<SharedStringTablePart>().First();
        //    //        /// Insert the text into the SharedStringTablePart.
        //    //        return InsertSharedStringItem(text, shareStringPart);
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Logger?.Error(ex);
        //    //        throw ex;
        //    //    }
        //    //}

        //    private static void SetCellValue(string columnName, uint rowIndex, WorksheetPart worksheetPart, SharedStringTablePart sharedStringPart, string value, CellValues dataType, uint styleIndex = 0)
        //    {
        //        try
        //        {
        //            Cell cell = InsertCellInWorksheet(columnName, rowIndex, worksheetPart);
        //            if (cell != null)
        //            {
        //                cell.StyleIndex = styleIndex;
        //                if (dataType == CellValues.Number)
        //                {
        //                    cell.DataType = new EnumValue<CellValues>(dataType);
        //                    cell.CellValue = new CellValue(value);
        //                }
        //                //else if (dataType == CellValues.String)// Cannot open by Microsoft Excel.
        //                //{
        //                //    cell.DataType = new EnumValue<CellValues>(dataType);
        //                //    cell.CellValue = new CellValue(value);
        //                //}
        //                //else if (dataType == CellValues.InlineString)// Cannot open by Microsoft Excel.
        //                //{
        //                //    cell.DataType = new EnumValue<CellValues>(dataType);
        //                //    cell.InlineString = new InlineString() { Text = new Text(value) };
        //                //}
        //                else
        //                {
        //                    //int index = InsertSharedStringItemByWorksheetPart(value, worksheetPart);
        //                    int index = InsertSharedStringItem(value, sharedStringPart);
        //                    if (index < 0)
        //                    {
        //                        //cell.DataType = new EnumValue<CellValues>(CellValues.InlineString);
        //                        //cell.InlineString = new InlineString() { Text = new Text(value) };
        //                        throw new Exception(string.Format("SetCellValue cannot set {0}", value));
        //                    }
        //                    //else
        //                    //{
        //                    //    cell.CellValue = new CellValue(index.ToString());
        //                    //    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                    //}
        //                    cell.CellValue = new CellValue(index.ToString());
        //                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                }
        //            }
        //        }
        //        catch (Exception ex) { Logger?.Error(ex); }
        //    }

        //    /// https://stackoverflow.com/questions/400733/how-to-get-ascii-value-of-string-in-c-sharp
        //    private static void SetTableHeader(uint startRowIndex, WorksheetPart worksheetPart, SharedStringTablePart sharedStringTablePart)
        //    {
        //        try
        //        {
        //            uint nextRowIndex = startRowIndex + 1;
        //            SetCellValue("A", startRowIndex, worksheetPart, sharedStringTablePart, "", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("B", startRowIndex, worksheetPart, sharedStringTablePart, "TMB (N1) in km/hr", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("C", startRowIndex, worksheetPart, sharedStringTablePart, "", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("D", startRowIndex, worksheetPart, sharedStringTablePart, "", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("E", startRowIndex, worksheetPart, sharedStringTablePart, "TMB (N2) in km/hr", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("F", startRowIndex, worksheetPart, sharedStringTablePart, "", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("G", startRowIndex, worksheetPart, sharedStringTablePart, "", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("H", startRowIndex, worksheetPart, sharedStringTablePart, "TMB (N3) in km/hr", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("I", startRowIndex, worksheetPart, sharedStringTablePart, "", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("J", startRowIndex, worksheetPart, sharedStringTablePart, "", CellValueOfString, CellStyleIndex.TableHeader);
        //            //MergeCells mergeCells = new MergeCells();
        //            //mergeCells.Append(new MergeCell() { Reference = new StringValue(string.Format("B{0}:D{0}", startRowIndex)) });
        //            //mergeCells.Append(new MergeCell() { Reference = new StringValue(string.Format("E{0}:G{0}", startRowIndex)) });
        //            //mergeCells.Append(new MergeCell() { Reference = new StringValue(string.Format("H{0}:J{0}", startRowIndex)) });
        //            //worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault());
        //            SetCellValue("K", startRowIndex, worksheetPart, sharedStringTablePart, "Average Value1", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("A", nextRowIndex, worksheetPart, sharedStringTablePart, "Time", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("B", nextRowIndex, worksheetPart, sharedStringTablePart, "Value1", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("C", nextRowIndex, worksheetPart, sharedStringTablePart, "Value2", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("D", nextRowIndex, worksheetPart, sharedStringTablePart, "Value3", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("E", nextRowIndex, worksheetPart, sharedStringTablePart, "Value1", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("F", nextRowIndex, worksheetPart, sharedStringTablePart, "Value2", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("G", nextRowIndex, worksheetPart, sharedStringTablePart, "Value3", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("H", nextRowIndex, worksheetPart, sharedStringTablePart, "Value1", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("I", nextRowIndex, worksheetPart, sharedStringTablePart, "Value2", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("J", nextRowIndex, worksheetPart, sharedStringTablePart, "Value3", CellValueOfString, CellStyleIndex.TableHeader);
        //            SetCellValue("K", nextRowIndex, worksheetPart, sharedStringTablePart, "of N1, N2 & N3", CellValueOfString, CellStyleIndex.TableHeader);
        //            //Row headerRow2 = new Row();
        //            //headerRow2.Append(
        //            //    ConstructCell("Time", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("10-Min Mean", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("Hourly Mean", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("Gust Wind", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("10-Min Mean", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("Hourly Mean", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("Gust Wind", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("10-Min Mean", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("Hourly Mean", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("Gust Wind", CellValues.String, CellStyleIndex.TableHeader),
        //            //    ConstructCell("of AN1, AN2 & AN3", CellValues.String, CellStyleIndex.TableHeader)
        //            //    );
        //            ////headerRow2.AppendChild(ConstructCell("Time", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("10-Min Mean", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("Hourly Mean", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("Gust Wind", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("10-Min Mean", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("Hourly Mean", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("Gust Wind", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("10-Min Mean", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("Hourly Mean", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("Gust Wind", CellValues.String, CellStyleIndex.TableHeader));
        //            ////headerRow2.AppendChild(ConstructCell("of AN1, AN2 & AN3", CellValues.String, CellStyleIndex.TableHeader));
        //            ////InsertRow(nextRowIndex, headerRow2, worksheetPart, sheetData);
        //            //sheetData.AppendChild(headerRow2);
        //        }
        //        catch (Exception ex) { Logger?.Error(ex); }
        //    }

        //    private static void SetTableDataRow(uint rowIndex, WorksheetPart worksheetPart, SharedStringTablePart sharedStringTablePart, KeyValuePair<int, string> o)
        //    {
        //        try
        //        {
        //            //if (o == null) { return; }
        //            //        SetCellValue("A", rowIndex, worksheetPart, sharedStringTablePart, o.Time.ToString("HH:mm"), CellValueOfString, CellStyleIndex.TableBody);
        //            //        SetCellValue("B", rowIndex, worksheetPart, sharedStringTablePart, o.An1TenMinMean?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("C", rowIndex, worksheetPart, sharedStringTablePart, o.An1HourlyMean?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("D", rowIndex, worksheetPart, sharedStringTablePart, o.An1GustWind?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("E", rowIndex, worksheetPart, sharedStringTablePart, o.An2TenMinMean?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("F", rowIndex, worksheetPart, sharedStringTablePart, o.An2HourlyMean?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("G", rowIndex, worksheetPart, sharedStringTablePart, o.An2GustWind?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("H", rowIndex, worksheetPart, sharedStringTablePart, o.An3TenMinMean?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("I", rowIndex, worksheetPart, sharedStringTablePart, o.An3HourlyMean?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("J", rowIndex, worksheetPart, sharedStringTablePart, o.An3GustWind?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            //        SetCellValue("K", rowIndex, worksheetPart, sharedStringTablePart, o.AvgTenMinMean?.ToString("n2"), CellValues.Number, CellStyleIndex.TableBody);
        //            SetCellValue("A", rowIndex, worksheetPart, sharedStringTablePart, o.Key.ToString(), CellValues.Number, CellStyleIndex.TableBody);
        //            SetCellValue("B", rowIndex, worksheetPart, sharedStringTablePart, o.Value, CellValueOfString, CellStyleIndex.TableBody);
        //            worksheetPart.Worksheet.Save();
        //        }
        //        catch (Exception ex) { Logger?.Error(ex); }
        //    }

        //    private static void SetTableData(uint excelStartRowIndex, int listStartRowIndex, int excelPageNumber, int excelTotalPage, uint excelPageRowCount, int listRowCountPerPage, WorksheetPart worksheetPart, SharedStringTablePart sharedStringTablePart, List<KeyValuePair<int, string>> eList)
        //    {
        //        try
        //        {
        //            if (eList == null || eList.Count < 1) { return; }
        //            uint iExcelRowIndex = excelStartRowIndex;
        //            int listFinishRowIndex = listStartRowIndex + listRowCountPerPage;
        //            if (listFinishRowIndex > eList.Count) { listFinishRowIndex = eList.Count; }
        //            int iList = listStartRowIndex;
        //            while (iList < listFinishRowIndex)
        //            {
        //                SetTableDataRow(iExcelRowIndex, worksheetPart, sharedStringTablePart, eList[iList]);
        //                iExcelRowIndex += 1;
        //                iList += 1;
        //            }
        //            uint PageRowOffset = 2;
        //            //uint iPageRowIndex = excelStartRowIndex + (uint)listRowCountPerPage + Param.Excel.PageRowOffset;
        //            uint iPageRowIndex = excelStartRowIndex + (uint)listRowCountPerPage + PageRowOffset;
        //            SetCellValue("K", iPageRowIndex, worksheetPart, sharedStringTablePart, string.Format("Page : {0} of {1}", excelPageNumber, excelTotalPage), CellValueOfString, CellStyleIndex.Default);
        //        }
        //        catch (Exception ex) { Logger?.Error(ex); }
        //    }

        //    /// https://docs.microsoft.com/en-us/office/open-xml/working-with-spreadsheetml-documents
        //    /// https://docs.microsoft.com/en-us/office/open-xml/how-to-create-a-spreadsheet-document-by-providing-a-file-name
        //    /// http://www.dispatchertimer.com/tutorial/how-to-create-an-excel-file-in-net-using-openxml-part-2-export-a-collection-to-spreadsheet/
        //    /// http://www.dispatchertimer.com/tutorial/how-to-create-an-excel-file-in-net-using-openxml-part-3-add-stylesheet-to-the-spreadsheet/
        //    public static bool ExportFile(string path)
        //    {
        //        try
        //        {
        //            using (SpreadsheetDocument oDoc = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
        //            {
        //                WorkbookPart oWorkbookPart = oDoc.AddWorkbookPart();
        //                oWorkbookPart.Workbook = new Workbook();
        //                WorksheetPart oWorksheetPart = oWorkbookPart.AddNewPart<WorksheetPart>();
        //                oWorksheetPart.Worksheet = new Worksheet();
        //                /// Add style.
        //                WorkbookStylesPart oStylePart = oWorkbookPart.AddNewPart<WorkbookStylesPart>();
        //                oStylePart.Stylesheet = GenerateStyleSheet();
        //                oStylePart.Stylesheet.Save();
        //                /// Setting up columns.
        //                oWorksheetPart.Worksheet.AppendChild(new Columns(
        //                    new Column { Min = 1, Max = 1, CustomWidth = true, Width = 5 },
        //                    new Column { Min = 2, Max = 10, CustomWidth = true, Width = 10 },
        //                    new Column { Min = 11, Max = 11, CustomWidth = true, Width = 17 }
        //                    ));
        //                oWorkbookPart.Workbook.Save();
        //                /// Shared string.
        //                SharedStringTablePart sharedStringTablePart = GetSharedStringTablePart(oDoc);
        //                //Sheets oSheets = oDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
        //                Sheets oSheets = oWorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
        //                oSheets.Append(new Sheet() { Id = oDoc.WorkbookPart.GetIdOfPart(oWorksheetPart), SheetId = 1, Name = "sheet1" });
        //                oWorksheetPart.Worksheet.Save();

        //                //SheetData oSheetData = oWorksheetPart.Worksheet.GetFirstChild<SheetData>();
        //                SheetData oSheetData = oWorksheetPart.Worksheet.AppendChild(new SheetData());

        //                /// Content.
        //                DateTime tRef = DateTime.Now;
        //                SetCellValue("A", 1, oWorksheetPart, sharedStringTablePart, "Meter Reading Daily Record", CellValueOfString, CellStyleIndex.Title);
        //                SetCellValue("J", 1, oWorksheetPart, sharedStringTablePart, "Date :", CellValueOfString, CellStyleIndex.Title);
        //                SetCellValue("K", 1, oWorksheetPart, sharedStringTablePart, tRef.ToString("dd-MM yyyy"), CellValueOfString, CellStyleIndex.Title);

        //                uint TableHeaderRowIndex = 3;
        //                uint TableHeaderRowCount = 2;
        //                uint TableFooterRowCount = 5;
        //                uint excelPageRowCount = 60;
        //                List<KeyValuePair<int, string>> eList = new List<KeyValuePair<int, string>>()
        //                {
        //                    new KeyValuePair<int, string>(0, "Empty"),
        //                    new KeyValuePair<int, string>(10, "Low"),
        //                    new KeyValuePair<int, string>(50, "Medium"),
        //                    new KeyValuePair<int, string>(90, "High")
        //                };
        //                int recordCountPerPage = (int)excelPageRowCount + 1 - (int)TableHeaderRowIndex - (int)TableHeaderRowCount - (int)TableFooterRowCount;
        //                int excelTotalPage = (int)Math.Ceiling((decimal)eList.Count / recordCountPerPage);
        //                if (eList != null && recordCountPerPage > 0)
        //                {
        //                    //uint excelStartRowIndex = Param.Excel.TableHeaderRowIndex;
        //                    uint excelStartRowIndex = TableHeaderRowIndex;
        //                    int recordStartRowIndex = 0;
        //                    int excelPageNumber = 1;
        //                    while (recordStartRowIndex < eList.Count)
        //                    {
        //                        SetTableHeader(excelStartRowIndex, oWorksheetPart, sharedStringTablePart);
        //                        //SetTableData(excelStartRowIndex + Param.Excel.TableHeaderRowCount,
        //                        //    recordStartRowIndex,
        //                        //    excelPageNumber,
        //                        //    excelTotalPage,
        //                        //    excelPageRowCount,
        //                        //    recordCountPerPage,
        //                        //    oWorksheetPart, sharedStringTablePart, eList);
        //                        SetTableData(excelStartRowIndex + TableHeaderRowCount,
        //                            recordStartRowIndex,
        //                            excelPageNumber,
        //                            excelTotalPage,
        //                            excelPageRowCount,
        //                            recordCountPerPage,
        //                            oWorksheetPart, sharedStringTablePart, eList);
        //                        /// increment.
        //                        excelStartRowIndex += excelPageRowCount;
        //                        recordStartRowIndex += recordCountPerPage;
        //                        excelPageNumber += 1;
        //                    }
        //                }
        //                oWorkbookPart.Workbook.Save();
        //            }
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger?.Error("Path = {0}", path);
        //            Logger?.Error(ex);
        //            return false;
        //        }
        //    }
        //}

        public class OleDb
        {
            /// https://gist.github.com/asurovov/1c13f6bddabaceab423c037494542e26
            /// https://zxxcc0001.pixnet.net/blog/post/14758489-%5Bc%23-.net%5D--%E5%A6%82%E4%BD%95-%E5%9C%A8c%23-%E4%BD%BF%E7%94%A8-oledb-%E8%AE%80%E5%8F%96-excel-%28%E4%B8%80%29
            /// https://www.codingame.com/playgrounds/9014/read-write-excel-file-with-oledb-in-c-without-interop
            /// https://stackoverflow.com/questions/43728201/how-to-read-xlsx-and-xls-files-using-c-sharp-and-oledbconnection
            
            private static string GetConnectionString(string path)
            {
                string strFileType = System.IO.Path.GetExtension(path).Trim().ToLower();
                if (".xls".Equals(strFileType, StringComparison.OrdinalIgnoreCase))
                    //return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                    return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                else if (".xlsx".Equals(strFileType, StringComparison.OrdinalIgnoreCase))
                    return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=No;IMEX=1\"";
                else
                    return null;
            }

            /// Get all sheetnames in the Excel file.
            /// Return Value = array of sheet names.
            public static string[] GetExcelSheetNames(string path)
            {
                if (string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path) == false) return null;
                using (OleDbConnection cn = new OleDbConnection(GetConnectionString(path)))
                {
                    cn.Open();
                    DataTable dt = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null) return null;
                    string[] excelSheets = new string[dt.Rows.Count];
                    int i = 0;
                    // Add the sheet name to the string array.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[i] = row["TABLE_NAME"].ToString().Trim('\'').Trim('$');
                        i++;
                    }
                    dt.Dispose();
                    cn.Close();
                    cn.Dispose();
                    return excelSheets;
                }
            }

            /// Get the content in the specific sheet of the Excel file.
            /// Need to install LinqToExcel by Paul Yoder in NuGet.
            private static EnumerableRowCollection<DataRow> GetWorkSheetCore(string connectionString, string sheetName)
            {
                using (OleDbDataAdapter da = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}$]", sheetName), connectionString))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt.AsEnumerable();
                }
            }

            /// Get the content in the specific sheet of the Excel file.
            /// Returns a specific worksheet as a linq-queryable enumeration.
            public static EnumerableRowCollection<DataRow> GetWorkSheet(string path, string sheetName)
            {
                return GetWorkSheetCore(GetConnectionString(path), sheetName);
            }

            /// Get the content in the first sheet of the Excel file.
            /// Returns a first worksheet as a linq-queryable enumeration.
            public static EnumerableRowCollection<DataRow> GetWorkSheet(string path)
            {
                if (string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path) == false) return null;
                string connectionString = GetConnectionString(path);
                using (OleDbConnection cn = new OleDbConnection(connectionString))
                {
                    cn.Open();
                    DataTable dtSchema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dtSchema == null || dtSchema.Rows.Count < 1) return null;
                    string sheetName = dtSchema.Rows[0]["TABLE_NAME"].ToString().Trim('\'').Trim('$');
                    return GetWorkSheetCore(connectionString, sheetName);
                    //dtSchema.Dispose();
                    //cn.Close();
                    //cn.Dispose();
                }
            }

            public static DataTable ReadExcelToDataTable(string path)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path) == false) return null;
                    using (OleDbConnection conn = new OleDbConnection(GetConnectionString(path)))
                    {
                        conn.Open();
                        DataTable dtSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dtSchema == null || dtSchema.Rows.Count < 1) return null;
                        string sheetName = (string)dtSchema.Rows[0]["TABLE_NAME"];
                        using (OleDbDataAdapter da = new OleDbDataAdapter("SELECT top 100000 * FROM [" + sheetName + "]", conn))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            return dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error("Path = {0}", path);
                    Logger?.Error(ex);
                    return null;
                }
            }
        }

        ///// Need to add "ClosedXML" by Francois Botha, Aleksei Pankratev, Manuel de Leon, Amir Ghezelbash in NuGet Package Manager. The below packages will be installed.
        ///// DocumentFormat.OpenXml.2.7.2
        ///// ExcelNumberFormat.1.0.10
        ///// ClosedXML.0.95.3
        ///// Add "using ClosedXML.Excel;" in the beginning of the code.
        ///// Fail to open by Spreadsheet compare after saving Excel file.
        ///// https://github.com/ClosedXML/ClosedXML/wiki
        //public class ClosedXML
        //{
        //    /// Return value = Error message if fail. Return null if success.
        //    /// If sheetName is empty, go to the first sheet.
        //    public static string DeleteRows(string path, string sheetName)
        //    {
        //        try
        //        {
        //            using (IXLWorkbook wb = new XLWorkbook(path))
        //            {
        //                if (wb.Worksheets.Count < 1) return "no sheet";
        //                IXLWorksheet ws = string.IsNullOrWhiteSpace(sheetName) ? wb.Worksheets.FirstOrDefault() : wb.Worksheet(sheetName);
        //                if (ws == null) return string.Format("Cannot find sheet {0}", sheetName);
        //                string a1String = (string)ws.Cell("A1").Value;
        //                string a2String = (string)ws.Cell("A2").Value;
        //                bool shouldSave = false;
        //                if (string.IsNullOrWhiteSpace(a2String))
        //                {
        //                    ws.Row(2).Delete();
        //                    shouldSave = true;
        //                }
        //                if (string.Equals("Query for ELMS Upload", a1String, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    ws.Row(1).Delete();
        //                    shouldSave = true;
        //                }
        //                if (shouldSave)
        //                    wb.Save();
        //                wb.Dispose();
        //            }
        //            return null;
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //        }
        //    }
        //}

        ///// Need to add "EPPlus" by EPPlus Software AB. The below packages will be installed.
        ///// System.ComponentModel.Annotations.5.0.0
        ///// EPPlus.5.4.2
        ///// Not suggest it because need charge for commercial use. Can use "EPPlus.Core" instead.
        ///// Add "using OfficeOpenXml;" in the beginning of the code.
        //public class EPPlus
        //{
        //    public static string DeleteRows(string path, string sheetName)
        //    {
        //        try
        //        {
        //            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //            using (ExcelPackage ep = new ExcelPackage(new System.IO.FileInfo(path)))
        //            {
        //                if (ep.Workbook.Worksheets.Count < 1) return "no sheet";
        //                ExcelWorksheet ws = string.IsNullOrWhiteSpace(sheetName) ? ep.Workbook.Worksheets.FirstOrDefault() : ep.Workbook.Worksheets.Where(s => string.Equals(s.Name, sheetName)).FirstOrDefault();
        //                if (ws == null) return string.Format("Cannot find sheet {0}", sheetName);
        //                string a1String = (string)ws.Cells["A1"].Value;
        //                string a2String = (string)ws.Cells["A2"].Value;
        //                bool shouldSave = false;
        //                if (string.IsNullOrWhiteSpace(a2String))
        //                {
        //                    ws.DeleteRow(2);
        //                    shouldSave = true;
        //                }
        //                if (string.Equals("Query for ELMS Upload", a1String, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    ws.DeleteRow(1);
        //                    shouldSave = true;
        //                }
        //                if (shouldSave)
        //                    ep.Save();
        //                ep.Dispose();
        //            }
        //            return null;
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //        }
        //    }
        //}

        ///// Need to add "EPPlus.Core" by Vahid Nasiri.
        ///// Add "using OfficeOpenXml;" in the beginning of the code.
        ///// Can open successfully by Spreadsheet compare after saving Excel file.
        ///// https://riptutorial.com/epplus
        //public class EPPlusCore
        //{
        //    public static string DeleteRows(string path, string sheetName)
        //    {
        //        try
        //        {
        //            using (ExcelPackage ep = new ExcelPackage(new System.IO.FileInfo(path)))
        //            {
        //                if (ep.Workbook.Worksheets.Count < 1) return "no sheet";
        //                ExcelWorksheet ws = string.IsNullOrWhiteSpace(sheetName) ? ep.Workbook.Worksheets.FirstOrDefault() : ep.Workbook.Worksheets.Where(s => string.Equals(s.Name, sheetName)).FirstOrDefault();
        //                if (ws == null) return string.Format("Cannot find sheet {0}", sheetName);
        //                string a1String = (string)ws.Cells["A1"].Value;
        //                string a2String = (string)ws.Cells["A2"].Value;
        //                bool shouldSave = false;
        //                if (string.IsNullOrWhiteSpace(a2String))
        //                {
        //                    ws.DeleteRow(2);
        //                    shouldSave = true;
        //                }
        //                if (string.Equals("Query for ELMS Upload", a1String, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    ws.DeleteRow(1);
        //                    shouldSave = true;
        //                }
        //                if (shouldSave)
        //                    ep.Save();
        //                ep.Dispose();
        //            }
        //            return null;
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //        }
        //    }
        //}


    }
}
