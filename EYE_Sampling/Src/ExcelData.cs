using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;


namespace EYE_Sampling
{
    using Excel = Microsoft.Office.Interop.Excel;

    public class ExcelData
    {
        private string filename;
        private MapFieldList MFL;
        public DataView Data;
        public ExcelData(string fname, MapFieldList mf)
        {
            filename = fname;
            MFL = mf;
            ProgressDialog dlg2 = new ProgressDialog();
            dlg2.Owner = null;
            for (int index = 0; index < App.Current.Windows.Count; index++)
            {
                if (App.Current.Windows[index].Title == "ΕΥΕ Sampling")
                {
                    dlg2.Owner = App.Current.Windows[index];
                    break;
                }
            } 
            dlg2.DialogText = "Εισαγωγή Δεδομένων από αρχείο";
            int startValue = 0;
            dlg2.RunWorkerThread(startValue, GetData);
        }
        public ExcelData(string fname, MapFieldList mf, bool check=true)
        {
            filename = fname;
            MFL = mf;
            ProgressDialog dlg2 = new ProgressDialog();
            dlg2.Owner = null;
            for (int index = 0; index < App.Current.Windows.Count; index++)
            {
                if (App.Current.Windows[index].Title == "ΕΥΕ Sampling")
                {
                    dlg2.Owner = App.Current.Windows[index];
                    break;
                }
            }
            dlg2.DialogText = "Εισαγωγή Δεδομένων από αρχείο";
            int startValue = 0;
            dlg2.RunWorkerThread(startValue, GetDataCheck);
        }
        private string parseHeader(string h)
        {
            h=h.Replace(".","");
            h = h.Replace("(", "");
            h = h.Replace(")", "");
            h = h.Replace("/", "");
            while (h.EndsWith(" "))
                h = h.Substring(0, h.Length - 2);
            return h;
        }

        private void doMessage(BackgroundWorker worker, int startvalue, int step, string m)
        {
            string msg = m;
            msg = String.Format(msg, startvalue, step);
            worker.ReportProgress(step, msg);
        }
        public void GetData(object sender, DoWorkEventArgs e)
        {
            const int startRow = 3;
            object OPT = System.Reflection.Missing.Value;
            Application excelApp = new ApplicationClass();
            Workbook workbook = (Workbook)excelApp.Workbooks.Open(filename, OPT, OPT, OPT, OPT, OPT, OPT, OPT, OPT, true, OPT, OPT, OPT, OPT, OPT);

                System.Data.DataTable dt = new System.Data.DataTable();
                BackgroundWorker worker = (BackgroundWorker)sender;
                int startValue = (int)e.Argument;

                try
                {

                    foreach (Mapfield m in MFL)
                    {
                        //if (m.Column == "-" && m.Name != "praxi") continue;
                        dt.Columns.Add(m.Label, typeof(string));
                    }
                    
                    foreach (Worksheet worksheet in workbook.Worksheets)
                    {
                        string praxi = "";
                        if (worksheet.Name.Contains("ΠΡΑΞΗ 1") && worksheet.Name.Contains("ΕΝΤΥΠΑ"))
                            praxi = "1";
                        else if (worksheet.Name.Contains("ΠΡΑΞΗ 2") && worksheet.Name.Contains("ΕΝΤΥΠΑ"))
                            praxi = "2";
                        else if (worksheet.Name.Contains("ΠΡΑΞΗ 3") && worksheet.Name.Contains("ΕΝΤΥΠΑ"))
                            praxi = "3";
                        else
                            continue;


                        doMessage(worker, startValue, 0, "Εισαγωγή worksheet " + worksheet.Name.ToString());

                        int column = 0;
                        int row = 0;

                        for (row = startRow; row <= worksheet.UsedRange.Rows.Count; row++)
                        {
                            int percent = Int32.Parse(praxi) * 33;

                            doMessage(worker, startValue, percent, "Εισαγωγή " + (row - startRow + 1).ToString() + " γραμμής από Excel, ΠΡΑΞΗ "+praxi);

                            DataRow dr = dt.NewRow();
                            column = 0;

                            foreach (Mapfield m in MFL)
                            {
                                if (m.Name == "praxi") dr[column] = praxi;
                                else
                                    if (m.Column == "-")
                                    {
                                        dr[column] = "";
                                    }
                                    else
                                    {
                                        dr[column] = (worksheet.get_Range(m.Column + row.ToString(), System.Reflection.Missing.Value) as Excel.Range).Text.ToString();
                                        if (dr[column].ToString().Contains('€'))
                                            dr[column] = (worksheet.get_Range(m.Column + row.ToString(), System.Reflection.Missing.Value) as Excel.Range).Value2.ToString();
                                        if (m.Type == "date" && dr[column].ToString() != "")
                                        {
                                            //dr[column] = new DateTime(1899, 12, 31).AddDays(Int32.Parse(dr[column].ToString())).ToString("dd/M/yyyy");
                                            string[] s = dr[column].ToString().Split('/');
                                            dr[column] = new DateTime(Int32.Parse("20" + s[2]), Int32.Parse(s[1]), Int32.Parse(s[0])).ToString("dd/M/yyyy");
                                        }
                                    }
                                column++;
                            }
                            //if (dr[0].ToString() == "") break;

                            dt.Rows.Add(dr);
                            dt.AcceptChanges();
                        }
                    }
                }
                catch (Exception ae)
                {
                   string s=ae.Message;
                   Data = null;
                }
                finally
                {
                    workbook.Close(true);
                    excelApp.Quit();

                    //release all memory - stop EXCEL.exe from hanging around.
                    if (workbook != null) { Marshal.ReleaseComObject(workbook); } //release each workbook like this
                    if (excelApp != null) { Marshal.ReleaseComObject(excelApp); } //release the Excel application
                    workbook = null; //set each memory reference to null.
                    excelApp = null;
                    GC.Collect();
                }

                Data = dt.DefaultView;
        }
        public void GetDataCheck(object sender, DoWorkEventArgs e)
        {
            const int startRow = 2;
            object OPT = System.Reflection.Missing.Value;
            Application excelApp = new ApplicationClass();
            Workbook workbook = (Workbook)excelApp.Workbooks.Open(filename, OPT, OPT, OPT, OPT, OPT, OPT, OPT, OPT, true, OPT, OPT, OPT, OPT, OPT);

            System.Data.DataTable dt = new System.Data.DataTable();
            BackgroundWorker worker = (BackgroundWorker)sender;
            int startValue = (int)e.Argument;

            try
            {

                foreach (Mapfield m in MFL)
                {
                    //if (m.Column == "-" && m.Name != "praxi") continue;
                    dt.Columns.Add(m.Name, typeof(string));
                }

                Worksheet worksheet = (Worksheet)workbook.Worksheets[1];

                int column = 0;
                int row = 0;
                int maxrows = worksheet.UsedRange.Rows.Count;

                for (row = startRow; row <= maxrows; row++)
                {
                    int percent = (row - startRow + 1) * 100 / maxrows;

                    doMessage(worker, startValue, percent, "Εισαγωγή " + (row - startRow + 1).ToString() + " γραμμής από Excel");

                    DataRow dr = dt.NewRow();
                    column = 0;

                    foreach (Mapfield m in MFL)
                    {
                        if (m.Column == "-")
                        {
                            dr[column] = "";
                        }
                        else
                        {
                            dr[column] = (worksheet.get_Range(m.Column + row.ToString(), System.Reflection.Missing.Value) as Excel.Range).Text.ToString();
                            if (dr[column].ToString().Contains('€'))
                                dr[column] = (worksheet.get_Range(m.Column + row.ToString(), System.Reflection.Missing.Value) as Excel.Range).Value2.ToString();
                            column++;
                        }
                    }
                    if (dr[0].ToString() == "") break;
                    dt.Rows.Add(dr);
                    dt.AcceptChanges();
                }
            }
            catch (Exception ae)
            {
                string s = ae.Message;
                Data = null;
            }
            finally
            {
                workbook.Close(true);
                excelApp.Quit();

                //release all memory - stop EXCEL.exe from hanging around.
                if (workbook != null) { Marshal.ReleaseComObject(workbook); } //release each workbook like this
                if (excelApp != null) { Marshal.ReleaseComObject(excelApp); } //release the Excel application
                workbook = null; //set each memory reference to null.
                excelApp = null;
                GC.Collect();
            }

            Data = dt.DefaultView;
        }
    }
}
