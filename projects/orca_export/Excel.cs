using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;


namespace orca_export
{
    class Excel
    {

        public void ExportToExcel(bool dateTimeStamp, string name, ArrayList inputCheckedFields, object[,] inputAttributeTable, string filePath)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application objExcel;
                //Microsoft.Office.Interop.Excel.Workbooks objBooks;
                Microsoft.Office.Interop.Excel.Workbook objBook;
                //Microsoft.Office.Interop.Excel.Sheets objSheets;
                //Microsoft.Office.Interop.Excel.Worksheet objSheet;
                //Microsoft.Office.Interop.Excel.Range objRange;
                Microsoft.Office.Interop.Excel.Range cells;
                Microsoft.Office.Interop.Excel.Worksheet oSheet;

                objExcel = new Microsoft.Office.Interop.Excel.Application();

                //objExcel.Visible = true;

                objExcel.DisplayAlerts = true;
                objBook = objExcel.Workbooks.Add();

                oSheet = objBook.Worksheets[1];
                cells = oSheet.Cells;

                if (orca_export.Properties.Settings.Default.formatAsText == true)
                    cells.NumberFormat = "@";
                else
                    cells.NumberFormat = "";

                oSheet.Name = name;
                oSheet.Range["A1"].Resize[1, inputCheckedFields.Count].Value = inputCheckedFields.ToArray();
                oSheet.Range["A1"].Resize[1, inputCheckedFields.Count].Font.Bold = true;
                oSheet.Range["A1"].Resize[1, inputCheckedFields.Count].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(204, 204, 204));
                oSheet.Range["A1"].Resize[1, inputCheckedFields.Count].Borders.Color = System.Drawing.Color.Black.ToArgb();
                oSheet.Range["A1"].Resize[1, inputCheckedFields.Count].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                oSheet.Range["A2"].Resize[inputAttributeTable.GetLength(0), inputCheckedFields.Count].Value = inputAttributeTable; //fieldList_fullName, fieldListAlias
                objExcel.Cells.EntireColumn.AutoFit();

                //numOfFilesCreated += 1;
                //SetDataWriteTime();

                if (dateTimeStamp == true)
                {
                    int dateTimeCell = inputAttributeTable.GetLength(0) + 4;
                    objExcel.Cells[dateTimeCell, 1].Value = "Export Date: " + DateTime.Now;
                    objExcel.Cells[dateTimeCell, 1].Font.Bold = true;
                    //objExcel.Cells(dateTimeCell, 1).Borders.Color = System.Drawing.Color.Black.ToArgb()
                }
               
                if (orca_export.Properties.Settings.Default.openInExcel == false)
                {
                    objBook.SaveAs(filePath);
                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Excel file successfully created.", "Success");
                    //MessageBox.Show("Excel file successfully created.", "Success");
                    objExcel.Quit();
                }
                else
                {
                    objExcel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please select a valid layer or table from the ArcMap table of contents...");
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message);

            }
        }
    }
}
