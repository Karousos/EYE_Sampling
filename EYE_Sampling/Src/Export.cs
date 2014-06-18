using System;
using System.Data;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;

namespace EYE_Sampling
{

    public class Export
    {
        
        public Export()
        {
        }

        public static void toXLS(DataTable ds, string filename)
        {
            string data;
            int i = 0;
            int j = 0;

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            for (i = 0; i <= ds.Columns.Count - 1; i++)
            {
                data = ds.Columns[i].Caption.ToString();
                xlWorkSheet.Cells[1, i + 1] = data;

            }
            
            for (i = 0; i <= ds.Rows.Count - 1; i++)
            {
                for (j = 0; j <= ds.Columns.Count - 1; j++)
                {
                    data = ds.Rows[i].ItemArray[j].ToString();
                    xlWorkSheet.Cells[i + 2, j + 1] = data;
                }
            }

            xlWorkBook.SaveAs(filename, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, true, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
               // MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}