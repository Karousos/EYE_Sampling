using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading;
using System.ComponentModel;


namespace EYE_Sampling
{
    public class SQLiteDatabase
    {
        public String dbConnection;
        public String Password = "Sampling";

        private String table;
        private ExcelData edata;
        private ListOfItems ldata;
        private MapFieldList mf;

        /// <summary>
        ///     Default Constructor for SQLiteDatabase Class.
        /// </summary>
        public SQLiteDatabase()
        {
            dbConnection = "Data Source=ppp.pp";
        }

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">The File containing the DB</param>
        public SQLiteDatabase(String inputFile)
        {
            dbConnection = String.Format("Data Source={0}", inputFile);
        }

        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        public SQLiteDatabase(Dictionary<String, String> connectionOpts)
        {
            String str = "";
            foreach (KeyValuePair<String, String> row in connectionOpts)
            {
                str += String.Format("{0}={1}; ", row.Key, row.Value);
            }
            str = str.Trim().Substring(0, str.Length - 1);
            dbConnection = str;
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Rows.Clear();
            dt.Columns.Clear();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.HasErrors)
                    {
                        System.Diagnostics.Debug.Write("Row ");
                        foreach (DataColumn dc in dt.Columns)
                            System.Diagnostics.Debug.Write(dc.ColumnName + ": '" + dr.ItemArray[dc.Ordinal] + "', ");
                        System.Diagnostics.Debug.WriteLine(" has error: " + dr.RowError);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            //cnn.SetPassword(Password);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(SQLiteConnection cnn, string sql)
        {
            //cnn.SetPassword(Password);
            //cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            //cnn.Close();
            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            //cnn.SetPassword(Password);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch 
            {
                //MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool MultiInsert(String tableName, List<Dictionary<String, String>> datalist)
        {
            String columns = "";
            string values = " SELECT null as 'id_data', ";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in datalist[0])
            {
                columns += val.Key.ToString()+", ";
                values += "'" + val.Value + "' as '" + val.Key + "', ";
            }
            values=values.Substring(0,values.Length-2);
            columns = "("+columns.Substring(0, columns.Length - 2)+")";
            string sql = "insert into " + tableName;
            for(int i=1; i<datalist.Count; i++)
            {
                values += " UNION SELECT null, ";
                foreach (KeyValuePair<String, String> val in datalist[i])
                {
                    values += "'" + val.Value + "', ";
                }
                values = values.Substring(0, values.Length - 2);
            }
            sql += values;
            try
            {
                this.ExecuteNonQuery(sql);
            }
            catch 
            {
                //MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            }
            catch
            {
                //MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(SQLiteConnection cnn, String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                this.ExecuteNonQuery(cnn, String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            }
            catch
            {
                //MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        
        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB()
        {
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    this.ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(String table)
        {
            try
            {

                this.ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void doMessage(BackgroundWorker worker, int startvalue, int step, string m)
        {
            string msg = m;
            msg = String.Format(msg, startvalue, step);
            worker.ReportProgress(step, msg);
        }
        
        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool FillData(String table, ExcelData edata, MapFieldList mf)
        {
            try
            {

               foreach(DataRow r in edata.Data.Table.Rows)
               {
                   Dictionary<string, string> dict = new Dictionary<string, string>(){};
                   int i = 0;
                   foreach(Mapfield m in mf)
                   {
                       dict.Add(m.Name,r[i++].ToString());
                   }
                   this.Insert(table, dict);
               }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void FillData(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            int startValue = (int)e.Argument;
            List<Dictionary<string, string>> datalist = new List<Dictionary<string, string>>();

            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            using (SQLiteTransaction mytransaction = cnn.BeginTransaction())
            {
                try
                {
                    int percent = 0;
                    int row = 0;
                    foreach (DataRow r in edata.Data.Table.Rows)
                    {
                        percent = (int)Math.Round((Double)row * 100 / edata.Data.Table.Rows.Count, 0);

                        doMessage(worker, startValue, percent, "Καταγραφή δεδομένων στη Βάση. Γραμμή: " + row++.ToString());

                        Dictionary<string, string> dict = new Dictionary<string, string>() { };
                        int i = 0;
                        foreach (Mapfield m in mf)
                        {
                            dict.Add(m.Name, r[i++].ToString());
                        }
                        this.Insert(cnn,table, dict);
                    }
                }
                catch
                {
                }
                mytransaction.Commit();
            }
            cnn.Close();

        }

        public void DoFillData(String t, ExcelData ed, MapFieldList m)
        {
            table = t;
            edata = ed;
            mf = m;

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
            dlg2.DialogText = "Καταγραφή δεδομένων στη Βάση.";
            int startValue = 0;
            dlg2.RunWorkerThread(startValue, FillData);
        }

        public void FillDatal(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            int startValue = (int)e.Argument;
            List<Dictionary<string, string>> datalist = new List<Dictionary<string, string>>();

            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            using (SQLiteTransaction mytransaction = cnn.BeginTransaction())
            {
                try
                {
                    int percent = 0;
                    int row = 0;
                    foreach (Item r in ldata)
                    {
                        percent = (int)Math.Round((Double)row * 100 / ldata.Count, 0);

                        doMessage(worker, startValue, percent, "Καταγραφή δεδομένων στη Βάση. Γραμμή: " + row++.ToString());

                        Dictionary<string, string> dict = new Dictionary<string, string>() { };
                        foreach (Mapfield m in mf)
                        {
                            dict.Add(m.Name, r.getProperty(m.Name).value.ToString());
                        }
                        this.Insert(cnn, table, dict);
                    }
                }
                catch
                {
                }
                mytransaction.Commit();
            }
            cnn.Close();

        }
        public void DoFillData(String t, ListOfItems li, MapFieldList m)
        {
            table = t;
            mf = m;
            ldata = li;

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
            dlg2.DialogText = "Καταγραφή δεδομένων στη Βάση.";
            int startValue = 0;
            dlg2.RunWorkerThread(startValue, FillDatal);
        }
        
        public Project GetProject()
        {
            Project p = new Project();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                // cnn.SetPassword(Password);
                cnn.Open();
                //  cnn.ChangePassword("");
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "Select pr_table_prefix from project order by id_project asc limit 1";
                SQLiteDataReader reader = mycommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        p.SetPrefix(reader.GetString(0));
                    }
                }
                else
                {
                    p = null;
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return p;
        }

        public bool FillDataNew(String table, DataTable edata)
        {
            try
            {
                string q="CREATE TABLE ["+table+"] ([id_data] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,[eponymia] VARCHAR(100)  NOT NULL,[ar_progr] INTEGER  NOT NULL,[titlos_progr] VARCHAR(200)  NULL)";

                this.ExecuteScalar(q);

               foreach(DataRow r in edata.Rows)
               {
                   Dictionary<string, string> dict = new Dictionary<string, string>()
                   {
                      {"eponymia", r[0].ToString()},
                      {"ar_progr", r[1].ToString()},
                      {"titlos_progr", r[2].ToString()}
                   };
                   this.Insert(table, dict);
               }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}