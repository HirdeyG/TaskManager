using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using Aspose.Cells;
using Excel;
using System.Reflection;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Optinuity.TaskManager.UI.Helpers
{
    /// <summary>
    /// Excel utility to import export data from excel
    /// </summary>
    public static class ExcelUtility
    {
        /// <summary>
        /// Sets asponse licence.
        /// </summary>
        public static void SetLicence()
        {
            License licence = new License();
            licence.SetLicense(HttpContext.Current.Server.MapPath("~/app_data/Aspose.Cells.lic"));
        }

        /// <summary>
        /// Loads dataset from aspose.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataSet LoadDataSet(Stream stream)
        {
            // set licence.
            SetLicence();

            DataSet rVal = new DataSet();

            Workbook workbook = new Workbook();
            workbook.Open(stream);

            foreach (Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Cells.Count == 0)
                    break;


                //DataTable table = sheet.Cells.ExportDataTableAsString(1, 0, sheet.Cells.MaxRow + 1,
                //         sheet.Cells.MaxColumn + 1);

                object[,] array = sheet.Cells.ExportArray(1, 0, sheet.Cells.MaxRow + 1,
                         sheet.Cells.MaxColumn + 1);

                DataTable table = loadFromObjectArray(array);

                for (int i = 0; i < sheet.Cells.MaxColumn + 1; i++)
                {
                    if (string.IsNullOrWhiteSpace(sheet.Cells[0, i].StringValue))
                        table.Columns[i].ColumnName = "Column " + i;
                    else
                        table.Columns[i].ColumnName = sheet.Cells[0, i].StringValue;
                }

                table.TableName = sheet.Name;

                rVal.Tables.Add(table);
            }

            return rVal;
        }

        /// <summary>
        /// Loads dataset from aspose starting from a specific row.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataSet LoadDataSet(Stream stream,int RowNum)
        {
            // set licence.
            SetLicence();

            DataSet rVal = new DataSet();

            Workbook workbook = new Workbook();
            workbook.Open(stream);

            foreach (Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Cells.Count == 0)
                    break;


                //DataTable table = sheet.Cells.ExportDataTableAsString(1, 0, sheet.Cells.MaxRow + 1,
                //         sheet.Cells.MaxColumn + 1);

                object[,] array = sheet.Cells.ExportArray(RowNum, 0, sheet.Cells.MaxRow + 1,
                         sheet.Cells.MaxColumn + 1);

                DataTable table = loadFromObjectArray(array);

                for (int i = 0; i < sheet.Cells.MaxColumn + 1; i++)
                {
                    if (string.IsNullOrWhiteSpace(sheet.Cells[RowNum, i].StringValue))
                        table.Columns[i].ColumnName = "Column " + i;
                    else
                        table.Columns[i].ColumnName = sheet.Cells[RowNum, i].StringValue + "_"+ i;
                }

                table.TableName = sheet.Name;

                rVal.Tables.Add(table);
            }

            return rVal;
        }
        /// <summary>
        /// Load From Object Array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static DataTable loadFromObjectArray(object[,] array)
        {
            DataTable rVal = new DataTable();
            int columnCount = array.GetUpperBound(1);
            int rowCount = array.GetUpperBound(0);

            for (int col = 0; col <= columnCount; col++)
            {
                rVal.Columns.Add(string.Format("Column_{0}", col));
            }

            for (int row = 0; row <= rowCount; row++)
            {
                DataRow tableRow = rVal.NewRow();
                for (int col = 0; col <= columnCount; col++)
                {

                    object cellVal = array[row, col];

                    if (cellVal != null)
                    {
                        if (cellVal.GetType() == typeof(Double))
                        {
                            double doubleVal = (double)cellVal;
                            if (doubleVal != Math.Round(doubleVal))
                                tableRow[col] = string.Format("{0:n6}", cellVal);
                            else
                                tableRow[col] = cellVal.ToString();

                        }
                        else
                        {
                            tableRow[col] = cellVal.ToString();
                        }
                    }

                }
                rVal.Rows.Add(tableRow);
            }
            return rVal;
        }


        /// <summary>
        /// Uses native export to excel feature from the underlaying provider
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataSet LoadDataSet2(Stream stream)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            excelReader.IsFirstRowAsColumnNames = true;
            //excelReader.
            return excelReader.AsDataSet();
        }

        /// <summary>
        /// Write data to excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fileName"></param>
        /// <param name="mappings"></param>
        public static void WriteToExcel<T>(IList<T> list, string fileName, List<ColumnMapping> mappings)
        {
            MemoryStream stream = Export<T>(list, mappings, "Sheet1");
            File.WriteAllBytes(fileName, stream.ToArray());
        }

        private static Dictionary<string, PropertyInfo> getPropertyDictionary(Type t)
        {
            PropertyInfo[] p = (t).GetProperties();
            Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo prop in p)
            {
                properties.Add(prop.Name.ToLower(), prop);
            }

            return properties;
        }

        /// <summary>
        /// Exports data from excel from stringly typed collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="mappings"></param>
        /// <param name="workSheetName"></param>
        /// <returns></returns>
        public static MemoryStream Export<T>(IList<T> collection, List<ColumnMapping> mappings, string workSheetName)
        {
            SetLicence();

            MemoryStream rVal = new MemoryStream();
            Type t = typeof(T);
            Dictionary<string, PropertyInfo> properties = getPropertyDictionary(t);


            // we will use same sequence of columns as defined in the mappings
            int seq = 0;
            foreach (ColumnMapping mapping in mappings)
            {
                mapping.Position = seq;
                seq++;
            }

            Workbook workbook = new Workbook();

            Worksheet worksheet = workbook.Worksheets[0];
            worksheet.Name = workSheetName;



            int column = 0;
            // add column headers..
            foreach (ColumnMapping mapping in mappings)
            {
                worksheet.Cells[0, mapping.Position].PutValue(mapping.ColumnName);
                column++;
            }

            Range headerRange = worksheet.Cells.CreateRange(0, 0, 1, mappings.Count);
            Style headerStyle = worksheet.Cells[0, 0].GetStyle();
            //headerStyle.BackgroundColor =
            headerStyle.ForegroundColor = Color.FromArgb(219, 232, 249);
            headerStyle.Pattern = BackgroundType.Solid;
            headerStyle.Font.IsBold = true;
            StyleFlag headerFlag = new StyleFlag();
            headerFlag.All = true;
            headerRange.ApplyStyle(headerStyle, headerFlag);
            // add data
            int row = 1;
            foreach (T obj in collection)
            {
                foreach (ColumnMapping mapping in mappings)
                {
                    object val = null;
                    Type returnType = null;

                    PropertyInfo p = properties[mapping.ObjectName.ToLower()];

                    val = p.GetValue(obj, null);

                    returnType = p.PropertyType;
                    Style cellStyle = worksheet.Cells[row, mapping.Position].GetStyle();

                    // set data...
                    if (!string.IsNullOrEmpty(mapping.Format) && returnType != typeof(DateTime))
                    {
                        string format = "{0:" + mapping.Format + "}";
                        worksheet.Cells[row, mapping.Position].PutValue(string.Format(format, val));

                    }
                    else
                    {
                        if (returnType == typeof(DateTime) || returnType == typeof(DateTime?))
                        {
                            if (string.IsNullOrWhiteSpace(mapping.Format))
                            {
                                cellStyle.Number = 14;
                                worksheet.Cells[row, mapping.Position].SetStyle(cellStyle);
                            }
                            else
                            {
                                cellStyle.Number = int.Parse(mapping.Format);
                                worksheet.Cells[row, mapping.Position].SetStyle(cellStyle);
                            }
                        }

                        if (returnType == typeof(Decimal) || returnType == typeof(Decimal?))
                        {
                            if (string.IsNullOrWhiteSpace(mapping.Format))
                            {
                                cellStyle.Number = 4;
                                worksheet.Cells[row, mapping.Position].SetStyle(cellStyle);
                            }
                            else
                            {
                                cellStyle.Number = int.Parse(mapping.Format);
                                worksheet.Cells[row, mapping.Position].SetStyle(cellStyle);
                            }
                        }

                        worksheet.Cells[row, mapping.Position].PutValue(val);
                    }
                }
                row++;
            }

            foreach (ColumnMapping mapping in mappings)
            {
                worksheet.AutoFitColumn(mapping.Position);
            }

            if (collection.Count > 0)
            {
                Range range = worksheet.Cells.CreateRange(0, 0, collection.Count + 1, mappings.Count);
                Style style = range.GetCellOrNull(1, 0).GetStyle();
                style.Borders[BorderType.TopBorder].Color = Color.Black;
                style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.LeftBorder].Color = Color.Black;
                style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.RightBorder].Color = Color.Black;
                style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.BottomBorder].Color = Color.Black;
                style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                StyleFlag flag = new StyleFlag();
                flag.Borders = true;
                range.ApplyStyle(style, flag);

            }

            workbook.Save(rVal, FileFormatType.Excel2007Xlsx);
            return rVal;
        }

        /// <summary>
        /// Export data from dynamic collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="mappings"></param>
        /// <param name="workSheetName"></param>
        /// <returns></returns>
        public static MemoryStream ExportDynamic(IList<object> collection, List<ColumnMapping> mappings, string workSheetName)
        {
            SetLicence();

            MemoryStream rVal = new MemoryStream();

            object first = collection[0];

            Type t = first.GetType();

            Dictionary<string, PropertyInfo> properties = getPropertyDictionary(t);

            // we will use same sequence of columns as defined in the mappings
            int seq = 0;
            foreach (ColumnMapping mapping in mappings)
            {
                mapping.Position = seq;
                seq++;
            }

            Workbook workbook = new Workbook();

            Worksheet worksheet = workbook.Worksheets[0];
            worksheet.Name = workSheetName;



            int column = 0;
            // add column headers..
            foreach (ColumnMapping mapping in mappings)
            {
                worksheet.Cells[0, mapping.Position].PutValue(mapping.ColumnName);
                column++;
            }

            Range headerRange = worksheet.Cells.CreateRange(0, 0, 1, mappings.Count);
            Style headerStyle = worksheet.Cells[0, 0].GetStyle();
            //headerStyle.BackgroundColor =
            headerStyle.ForegroundColor = Color.FromArgb(219, 232, 249);
            headerStyle.Pattern = BackgroundType.Solid;
            headerStyle.Font.IsBold = true;
            StyleFlag headerFlag = new StyleFlag();
            headerFlag.All = true;
            headerRange.ApplyStyle(headerStyle, headerFlag);
            // add data
            int row = 1;
            foreach (dynamic obj in collection)
            {


                foreach (ColumnMapping mapping in mappings)
                {
                    object val = null;
                    Type returnType = null;

                    PropertyInfo p = properties[mapping.ObjectName.ToLower()];

                    val = p.GetValue(obj, null);

                    returnType = p.PropertyType;
                    Style cellStyle = worksheet.Cells[row, mapping.Position].GetStyle();

                    if (mapping.WrapText)
                        cellStyle.ShrinkToFit = true;

                    // set data...
                    if (!string.IsNullOrEmpty(mapping.Format) && returnType != typeof(DateTime))
                    {
                        string format = "{0:" + mapping.Format + "}";
                        worksheet.Cells[row, mapping.Position].PutValue(string.Format(format, val));

                    }
                    else
                    {
                        if (returnType == typeof(DateTime) || returnType == typeof(DateTime?))
                        {
                            if(string.IsNullOrWhiteSpace(mapping.Format)){
                                cellStyle.Number = 14;
                                worksheet.Cells[row, mapping.Position].SetStyle(cellStyle);
                            } else {
                                cellStyle.Number = int.Parse(mapping.Format);
                                worksheet.Cells[row, mapping.Position].SetStyle(cellStyle);
                            }
                        }

                        if (returnType == typeof(Decimal) || returnType == typeof(Decimal?))
                        {
                            if (string.IsNullOrWhiteSpace(mapping.Format))
                            {
                                cellStyle.Number = 4;
                                worksheet.Cells[row, mapping.Position].SetStyle(cellStyle);
                            }
                            else
                            {
                                cellStyle.Number = int.Parse(mapping.Format);
                                worksheet.Cells[row, mapping.Position].SetStyle(cellStyle);
                            }
                        }
                        

                        worksheet.Cells[row, mapping.Position].PutValue(val);
                    }
                }

                // add dummy cell at to end to avoid overflow..
                worksheet.Cells[row, mappings.Count].PutValue(" ");

                row++;
            }

            foreach (ColumnMapping mapping in mappings)
            {
                if (mapping.AutoFitColumn)
                    worksheet.AutoFitColumn(mapping.Position);

                if (mapping.ColumnWidth > 0)
                    worksheet.Cells.SetColumnWidth(mapping.Position, mapping.ColumnWidth);

            }

            //Range range = worksheet.Cells.CreateRange(0, 0, collection.Count + 1, mappings.Count);
            //Style style = range.GetCellOrNull(1, 0).GetStyle();
            //style.Borders[BorderType.TopBorder].Color = Color.Black;
            //style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            //style.Borders[BorderType.LeftBorder].Color = Color.Black;
            //style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            //style.Borders[BorderType.RightBorder].Color = Color.Black;
            //style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            //style.Borders[BorderType.BottomBorder].Color = Color.Black;
            //style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            //StyleFlag flag = new StyleFlag();
            //flag.Borders = true;
            //range.ApplyStyle(style, flag);



            workbook.Save(rVal, FileFormatType.Excel2007Xlsx);
            return rVal;
        }

        public static string StripHTML(string HTMLText, bool decode = true)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            var stripped = reg.Replace(HTMLText, " ");
            return decode ? HttpUtility.HtmlDecode(stripped) : stripped;
        }

    }

    /// <summary>
    /// Column mapping class. maps excel column with collection.
    /// </summary>
    public class ColumnMapping
    {
        /// <summary>
        /// Name of the object
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Name of the excel column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Type if the object (.net Type)
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Display position
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Format of the data in string formatting is required.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Yes if autofit excel feature is required.
        /// </summary>
        public bool AutoFitColumn { get; set; }

        /// <summary>
        /// Yes if text wrapping is required.
        /// </summary>
        public bool WrapText { get; set; }

        /// <summary>
        /// Width of the column.
        /// </summary>
        public int ColumnWidth { get; set; }
    }
}