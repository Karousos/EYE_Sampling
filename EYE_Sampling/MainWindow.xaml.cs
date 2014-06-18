using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Threading;
using DW.RtfWriter;
using System.Diagnostics;
using System.Windows.Media;
namespace EYE_Sampling
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteDatabase db = new SQLiteDatabase("EYE_Sampling_DB.s3db");
        DataTable Table_Input_Items, Table_Sampling_Items, Table_Output_Items;
        ObservableCollection<Project> ProjectList = new ObservableCollection<Project>();
        public Project CurrentProject = new Project();

        public bool isTest = false;
       
        public MainWindow()
        {
            var LoginW= new Window_Login();
            LoginW.database = db;
            LoginW.ShowDialog();
            InitializeComponent();
            Table_Sampling_Items = new DataTable();
            Table_Output_Items = new DataTable();
            TLogger.LogDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            TLogger.LogFile = "History.txt";
            if (LoginW.username=="") Close();
            LB_User.Content = LoginW.username;
        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            TLogger.AddLog("Opening...");
            load_Items();
        }

        #region TABImport

        private void load_Items()
        {

            try
            {
                String query = "select [ekepis],[eponimia],[praxi],[ypoergo],[kod_programmatos], [titlos_programmatos], [proipologismos], [periferia], [nomos], [dimos], [theoritiki_start_date], [theoritiki_end_date], [praktiki_start_date], [arithmos_katartizomenon], [plithos_elegxon], [synolo_poinon] from Current_data";
                Table_Input_Items = db.GetDataTable(query);
                for (int i = 0; i < Table_Input_Items.Columns.Count; i++)
                {
                    Table_Input_Items.Columns[i].ColumnName = CurrentProject.mf[i].Label;
                }
                DG_Items.ItemsSource = Table_Input_Items.AsDataView();
                CurrentProject.InputItems.Clear();
                CurrentProject.InputItems.Load(Table_Input_Items, CurrentProject.mf);
                CurrentProject.Get_All_Regions();
                LB_total_items.Content = CurrentProject.NumOfItems.ToString();

                ResetSettings();
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
                this.Close();
            }
            ApplyRules();
            BT_FileImport_Checks.IsEnabled = true;

        }

        private void BT_Mappings_Click(object sender, RoutedEventArgs e)
        {
            var ConfigW = new Window_Config();
            ConfigW.Mappings = CurrentProject.mf;
            ConfigW.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ConfigW.ShowDialog();
        }

        private void BT_Mappings_Check_Click(object sender, RoutedEventArgs e)
        {
            var ConfigW = new Window_Config();
            ConfigW.Mappings = CurrentProject.mf_check;
            ConfigW.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ConfigW.ShowDialog();
        }

        private void BT_FileImport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel documents|*.xls;*.xlsx";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                ExcelData exceldata = new ExcelData(dlg.FileName, CurrentProject.mf);
                DG_Items.DataContext = null;;
                DG_Items.ItemsSource = null;
                db.ClearTable("Current_Data");
                db.DoFillData("Current_Data", exceldata, CurrentProject.mf);
                load_Items();               

                if (CurrentProject.InputItems.Count() > 0)
                    BT_FileImport_Checks.IsEnabled = true;

                System.IO.File.Copy(dlg.FileName, System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/CURRENT_EXCEL/current.xls", true);
            }
        }

        private void BT_FileImport_Checks_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.title == "")
            {
                MessageBox.Show("Παρακαλώ ορίστε τον τίτλο του Project!");
                return;
            } if (CurrentProject.mis == "")
            {
                MessageBox.Show("Παρακαλώ ορίστε τον κωδικό MIS του Project!");
                return;
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel documents|*.xls;*.xlsx";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                db.ExecuteNonQuery("Update Current_Data set plithos_elegxon=0, synolo_poinon=0");
                ExcelData exceldata_Check = new ExcelData(dlg.FileName, CurrentProject.mf_check, true);
                string poin = "";
                foreach (DataRow r in exceldata_Check.Data.Table.Rows)
                {
                    if (r["ergo"].ToString() != CurrentProject.title && r["mis"].ToString() != CurrentProject.mis) continue;
                    poin = "";
                    if (r["poines"].ToString()!="")
                        poin=", synolo_poinon=synolo_poinon + "+r["poines"].ToString();
                    db.ExecuteNonQuery("Update Current_Data set plithos_elegxon=plithos_elegxon+1"+poin+" where ekepis=" + r["ekepis"].ToString() + " and praxi=" + r["praxi"].ToString());
                }
                DG_Items.DataContext = null; ;
                DG_Items.ItemsSource = null;
                load_Items();
            }


        }


        private void TB_Title_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentProject.title = TB_Title.Text;
        }

        private void TB_MIS_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentProject.mis = TB_MIS.Text;
        }


        #endregion

        #region TABOptions

        public void ApplyRules()
        {
            LB_Participation_Rest.Content = CurrentProject.ApplyRules(CurrentProject.InputItems, CurrentProject.ParticipationRules).ToString();
            if (Int32.Parse(LB_Participation_Rest.Content.ToString()) < Int32.Parse(TB_NumOfSamples.Text))
            {
                LB_Participation_Rest.Foreground = new SolidColorBrush(Colors.Red);
                LB_Participation_Rest.ToolTip = "Μη επαρκούμενο πλήθος πληθυσμού.";
            }
            else
            {
                LB_Participation_Rest.Foreground = new SolidColorBrush(Colors.Black);
                LB_Participation_Rest.ToolTip = "";
            }
            LB_Selection.Content = CurrentProject.ApplyRules(CurrentProject.InputItems, CurrentProject.SelectionRules).ToString();
            LB_Selection_Rest.Content = (Int32.Parse(LB_Participation_Rest.Content.ToString())-Int32.Parse(LB_Selection.Content.ToString())).ToString();
            CurrentProject.NumOfPreSelected = Int32.Parse(LB_Selection.Content.ToString());
            if (Int32.Parse(LB_Selection.Content.ToString()) > Int32.Parse(TB_NumOfSamples.Text))
            {
                LB_Selection.Foreground = new SolidColorBrush(Colors.Red);
                LB_Selection.ToolTip = "Επιλέχθηκαν περισσότερα από τα επιτρεπόμενα.";
            }
            else
            {
                LB_Selection.Foreground = new SolidColorBrush(Colors.Black);
                LB_Selection.ToolTip = "";
            }
            Disable_sampling_buttons();
            DG_Input_Projects.ItemsSource = null;
            DG_Output_Projects.ItemsSource = null;
            LB_Green.Visibility = Visibility.Hidden;
            LB_Blue.Visibility = Visibility.Hidden;

        }

        private void ResetSettings()
        {
            LB_Participation_Total.Content = CurrentProject.NumOfItems.ToString();
            LB_Participation_Rest.Content = CurrentProject.NumOfItems.ToString();
            LB_Selection.Content = "0";
            LB_Selection_Rest.Content = CurrentProject.NumOfItems.ToString();
            CB_Praxi.IsChecked = false;
            CB_Region.IsChecked = false;
            CB_Date.IsChecked = false;
            CB_Money.IsChecked = false;
            CB_Students.IsChecked = false;
            CB_Theoritical.IsChecked = false;
        }

        private void TB_NumOfSamples_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CurrentProject.NumOfSamples = Int32.Parse(TB_NumOfSamples.Text);
            }
            catch
            {
            }
        }


        private void TB_NumOfSamples_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TB_NumOfSamples.Text = Math.Round(((double)Int32.Parse(TB_Percent.Text) * CurrentProject.NumOfItems/ 100), 0).ToString();
            }
            catch
            {
                TB_NumOfSamples.Text = "";
            }
        }

        private void TB_Percent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_NumOfSamples == null) return;
            try
            {
                TB_NumOfSamples.Text = Math.Round(((double)Int32.Parse(TB_Percent.Text) * CurrentProject.NumOfItems / 100), 0).ToString();
            }
            catch
            {
                TB_NumOfSamples.Text = "";
            }
        }

        private void TB_NumOfSamples_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Percent == null) return;
            try
            {
                TB_Percent.Text = (CurrentProject.NumOfItems == 0 ? "" : Math.Round(((double)Int32.Parse(TB_NumOfSamples.Text) * 100 / CurrentProject.NumOfItems), 0).ToString());
            }
            catch
            {
                TB_Percent.Text = "";
            }
        }

        #region Participation_Filters

        private void CB_Praxi_Checked(object sender, RoutedEventArgs e)
        {
            ((PraxiRule)CurrentProject.ParticipationRules.getRule("Praxi")).enabled = CB_Praxi.IsChecked.Value;
            BT_Praxi.IsEnabled = CB_Praxi.IsChecked.Value;
            ApplyRules();
        }
        private void CB_Praxi_Unchecked(object sender, RoutedEventArgs e)
        {
            ((PraxiRule)CurrentProject.ParticipationRules.getRule("Praxi")).enabled = CB_Praxi.IsChecked.Value;
            BT_Praxi.IsEnabled = CB_Praxi.IsChecked.Value;
            ApplyRules();

        }
        private void BT_Praxi_Click(object sender, RoutedEventArgs e)
        {
            var PraxiW = new Window_Praxi();
            PraxiW.Owner = this;
            PraxiW.selected_praxeis = LB_Praxeis.Content.ToString();
            PraxiW.ShowDialog();
            LB_Praxeis.Content = PraxiW.selected_praxeis;
            string[] s = PraxiW.selected_praxeis.Split(',');
            ((PraxiRule)CurrentProject.ParticipationRules.getRule("Praxi")).praxeis.Clear();
            foreach (string p in s)
                ((PraxiRule)CurrentProject.ParticipationRules.getRule("Praxi")).praxeis.Add(p);
            ApplyRules();

        }

        private void CB_Region_Checked(object sender, RoutedEventArgs e)
        {
            ((RegionRule)CurrentProject.ParticipationRules.getRule("Region")).enabled = CB_Region.IsChecked.Value;
            BT_Region.IsEnabled = CB_Region.IsChecked.Value;
            ApplyRules();
        }
        private void CB_Region_Unchecked(object sender, RoutedEventArgs e)
        {
            ((RegionRule)CurrentProject.ParticipationRules.getRule("Region")).enabled = CB_Region.IsChecked.Value;
            BT_Region.IsEnabled = CB_Region.IsChecked.Value;
            ApplyRules();
        }
        private void BT_Region_Click(object sender, RoutedEventArgs e)
        {
            var regionW = new Window_Regions();
            regionW.Owner = this;
            regionW.All_Regions= CurrentProject.All_Regions;
            regionW.Sel_Regions = ((RegionRule)CurrentProject.ParticipationRules.getRule("Region")).regions;
            regionW.ShowDialog();
            ApplyRules();
        }

        private void CB_Date_Checked(object sender, RoutedEventArgs e)
        {
            ((DateRule)CurrentProject.ParticipationRules.getRule("Date")).enabled = CB_Date.IsChecked.Value;
            DP_Start.IsEnabled = CB_Date.IsChecked.Value;
            DP_Stop.IsEnabled = CB_Date.IsChecked.Value;
                ApplyRules();
        }
        private void CB_Date_Unchecked(object sender, RoutedEventArgs e)
        {
            ((DateRule)CurrentProject.ParticipationRules.getRule("Date")).enabled = CB_Date.IsChecked.Value;
            DP_Start.IsEnabled = CB_Date.IsChecked.Value;
            DP_Stop.IsEnabled = CB_Date.IsChecked.Value;
                ApplyRules();
        }
        private void DP_Start_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DateRule)CurrentProject.ParticipationRules.getRule("Date")).StartDate = DP_Start.SelectedDate.Value.ToString();
            ApplyRules();
        }
        private void DP_Stop_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DateRule)CurrentProject.ParticipationRules.getRule("Date")).StopDate = DP_Stop.SelectedDate.Value.ToString();
            ApplyRules();
        }

        private void CB_Money_Unchecked(object sender, RoutedEventArgs e)
        {
            ((MoneyRule)CurrentProject.ParticipationRules.getRule("Money")).enabled = CB_Money.IsChecked.Value;
            TB_Money_From.IsEnabled = CB_Money.IsChecked.Value;
            TB_Money_To.IsEnabled = CB_Money.IsChecked.Value;
            ApplyRules();

        }
        private void CB_Money_Checked(object sender, RoutedEventArgs e)
        {
            ((MoneyRule)CurrentProject.ParticipationRules.getRule("Money")).enabled = CB_Money.IsChecked.Value;
            TB_Money_From.IsEnabled = CB_Money.IsChecked.Value;
            TB_Money_To.IsEnabled = CB_Money.IsChecked.Value;
            try
            {
                ((MoneyRule)CurrentProject.ParticipationRules.getRule("Money")).limit_from = Double.Parse(TB_Money_From.Text);
                ((MoneyRule)CurrentProject.ParticipationRules.getRule("Money")).limit_to = Double.Parse(TB_Money_To.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }
        private void TB_Money_From_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MoneyRule)CurrentProject.ParticipationRules.getRule("Money")).limit_from = Double.Parse(TB_Money_From.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }
        private void TB_Money_To_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MoneyRule)CurrentProject.ParticipationRules.getRule("Money")).limit_to = Double.Parse(TB_Money_To.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }

        private void CB_Theoritical_CheckEvent(object sender, RoutedEventArgs e)
        {
            ((TheoriticalRule)CurrentProject.ParticipationRules.getRule("Theoritical")).enabled = CB_Theoritical.IsChecked.Value;
            Combo_Theoritical.IsEnabled = CB_Theoritical.IsChecked.Value;
            ApplyRules();
        }
        private void Combo_Theoritical_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ((TheoriticalRule)CurrentProject.ParticipationRules.getRule("Theoritical")).istheoritical = Combo_Theoritical.Text != "Θεωρητική Κατάρτιση";
                ApplyRules();
            }
            catch
            {
                return;
            }
        }

        private void CB_Students_Checked(object sender, RoutedEventArgs e)
        {
            ((StudentsRule)CurrentProject.ParticipationRules.getRule("Students")).enabled = CB_Students.IsChecked.Value;
            TB_Students.IsEnabled = CB_Students.IsChecked.Value;
            try
            {
                ((StudentsRule)CurrentProject.ParticipationRules.getRule("Students")).limit = Int32.Parse(TB_Students.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }
        private void CB_Students_Unchecked(object sender, RoutedEventArgs e)
        {
            ((StudentsRule)CurrentProject.ParticipationRules.getRule("Students")).enabled = CB_Students.IsChecked.Value;
            TB_Students.IsEnabled = CB_Students.IsChecked.Value;
            ApplyRules();
        }
        private void TB_Students_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((StudentsRule)CurrentProject.ParticipationRules.getRule("Students")).limit = Int32.Parse(TB_Students.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }

        private void CB_Anadoxoi_Checked(object sender, RoutedEventArgs e)
        {
            ((AnadoxoiRule)CurrentProject.ParticipationRules.getRule("Anadoxoi")).enabled = CB_Anadoxoi.IsChecked.Value;
            TB_Anadoxoi.IsEnabled = CB_Anadoxoi.IsChecked.Value;
            try
            {
                ((AnadoxoiRule)CurrentProject.ParticipationRules.getRule("Anadoxoi")).limit = Int32.Parse(TB_Anadoxoi.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }
        private void CB_Anadoxoi_Unchecked(object sender, RoutedEventArgs e)
        {
            ((AnadoxoiRule)CurrentProject.ParticipationRules.getRule("Anadoxoi")).enabled = CB_Anadoxoi.IsChecked.Value;
            TB_Anadoxoi.IsEnabled = CB_Anadoxoi.IsChecked.Value;
            ApplyRules();
        }
        private void TB_Anadoxoi_LostFocus(object sender, RoutedEventArgs e)
        {
            ((AnadoxoiRule)CurrentProject.ParticipationRules.getRule("Anadoxoi")).enabled = CB_Anadoxoi.IsChecked.Value;
            TB_Anadoxoi.IsEnabled = CB_Anadoxoi.IsChecked.Value;
            try
            {
                ((AnadoxoiRule)CurrentProject.ParticipationRules.getRule("Anadoxoi")).limit = Int32.Parse(TB_Anadoxoi.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }

        private void CB_Yp_Anadoxoi_Checked(object sender, RoutedEventArgs e)
        {
            ((YpAnadoxoiRule)CurrentProject.SelectionRules.getRule("YpAnadoxoi")).enabled = CB_Yp_Anadoxoi.IsChecked.Value;
            TB_Yp_Anadoxoi.IsEnabled = CB_Yp_Anadoxoi.IsChecked.Value;
            try
            {
                ((YpAnadoxoiRule)CurrentProject.SelectionRules.getRule("YpAnadoxoi")).limit = Int32.Parse(TB_Yp_Anadoxoi.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }
        private void CB_Yp_Anadoxoi_Unchecked(object sender, RoutedEventArgs e)
        {
            ((YpAnadoxoiRule)CurrentProject.SelectionRules.getRule("YpAnadoxoi")).enabled = CB_Yp_Anadoxoi.IsChecked.Value;
            TB_Yp_Anadoxoi.IsEnabled = CB_Yp_Anadoxoi.IsChecked.Value;
            ApplyRules();
        }
        private void TB_Yp_Anadoxoi_LostFocus(object sender, RoutedEventArgs e)
        {
            ((YpAnadoxoiRule)CurrentProject.SelectionRules.getRule("YpAnadoxoi")).enabled = CB_Yp_Anadoxoi.IsChecked.Value;
            TB_Yp_Anadoxoi.IsEnabled = CB_Yp_Anadoxoi.IsChecked.Value;
            try
            {
                ((YpAnadoxoiRule)CurrentProject.SelectionRules.getRule("YpAnadoxoi")).limit = Int32.Parse(TB_Yp_Anadoxoi.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();

        }

        private void CB_Yp_Poines_Checked(object sender, RoutedEventArgs e)
        {
            ((YpPoinesRule)CurrentProject.SelectionRules.getRule("YpPoines")).enabled = CB_Yp_Poines.IsChecked.Value;
            TB_Yp_Poines.IsEnabled = CB_Yp_Poines.IsChecked.Value;
            try
            {
                ((YpPoinesRule)CurrentProject.SelectionRules.getRule("YpPoines")).limit = double.Parse(TB_Yp_Poines.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }
        private void CB_Yp_Poines_Unchecked(object sender, RoutedEventArgs e)
        {
            ((YpPoinesRule)CurrentProject.SelectionRules.getRule("YpPoines")).enabled = CB_Yp_Poines.IsChecked.Value;
            TB_Yp_Poines.IsEnabled = CB_Yp_Poines.IsChecked.Value;
            ApplyRules();
        }
        private void TB_Yp_Poines_LostFocus(object sender, RoutedEventArgs e)
        {
            ((YpPoinesRule)CurrentProject.SelectionRules.getRule("YpPoines")).enabled = CB_Yp_Poines.IsChecked.Value;
            TB_Yp_Poines.IsEnabled = CB_Yp_Poines.IsChecked.Value;
            try
            {
                ((YpPoinesRule)CurrentProject.SelectionRules.getRule("YpPoines")).limit = double.Parse(TB_Yp_Poines.Text);
            }
            catch
            {
                return;
            }
            ApplyRules();
        }

        #endregion

        private void RB_Random_Checked(object sender, RoutedEventArgs e)
        {
            CurrentProject.isMus = false;
        }
        private void RB_MUS_Checked(object sender, RoutedEventArgs e)
        {
            CurrentProject.isMus = true;
        }

        #endregion

        #region TABSampling

        private void Disable_sampling_buttons()
        {
            BT_Start.IsEnabled = false;
            BT_Sampling.IsEnabled = false;
            BT_Output.IsEnabled = false;
            BT_Report.IsEnabled = false;
            BT_Original.IsEnabled = false;
        }
        private void Enable_sampling_buttons()
        {
            BT_Start.IsEnabled = true;
            BT_Sampling.IsEnabled = true;
            BT_Output.IsEnabled = true;
            BT_Report.IsEnabled = true;
            BT_Original.IsEnabled = true;
        }

        private void BT_LoadProjects_Click(object sender, RoutedEventArgs e)
        {
            CurrentProject.InputItems.CopyEnabledItems(CurrentProject.SamplingItems);
            Table_Sampling_Items.Clear();
            CurrentProject.SamplingItems.ToDataTable(Table_Sampling_Items, CurrentProject.mf);
            DG_Input_Projects.ItemsSource = Table_Sampling_Items.AsDataView();
            LB_Sampling_All.Content = CurrentProject.NumOfItems.ToString();
            LB_Sampling_ForSampling.Content = LB_Selection_Rest.Content;
            LB_Sampling_Asked.Content = TB_NumOfSamples.Text;
            LB_Sampling_Selected.Content = LB_Selection.Content.ToString();
            LB_NumOfRestSamples.Content = (Int32.Parse(TB_NumOfSamples.Text) - Int32.Parse(LB_Selection.Content.ToString())).ToString();
            DG_Input_Projects.IsReadOnly = true;
            BT_Start.IsEnabled = true;
        }
        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.NumOfSamples - CurrentProject.NumOfPreSelected > CurrentProject.SamplingItems.Count)
            {
                MessageBox.Show("Τα ζητούμενα δείγματα είναι περισσότερα από τον πληθυσμό των προγραμμάτων προς δειγματοληψία!");
                return;
            }
            if (CurrentProject.NumOfPreSelected > CurrentProject.NumOfSamples)
            {
                MessageBox.Show("Τα ήδη επιλεγμένα προγράμματα είναι περισσότερα από το επιτρεπτό όριο!");
                return;
            }
            if (CurrentProject.title == "")
            {
                MessageBox.Show("Παρακαλώ ορίστε τον τίτλο του Project (στο αρχικό TAB) για το οποίο κάνετε δειγματοληψία!");
                return;
            }

            //ΕΚΤΕΛΕΣΗ ΑΛΓΟΡΙΘΜΟΥ
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
            dlg2.DialogText = "Λειτουργία δειγματοληψίας";
            int startValue = 0;
            dlg2.RunWorkerThread(startValue, Exec);

            Enable_sampling_buttons();
        }
        private void BT_Sampling_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + CurrentProject.InputExcel());
        }
        private void BT_Original_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + CurrentProject.OriginalExcel());
        }
        private void BT_Output_Click(object sender, RoutedEventArgs e)
        {
            var p = new Process { StartInfo = { FileName = "Demo.rtf" } };

            System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + CurrentProject.OutputExcel());
        }
        private void BT_Report_Click(object sender, RoutedEventArgs e)
        {
            // ==========================================================================
            // Open the RTF file we just saved
            // ==========================================================================
            try
            {
                string file = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + CurrentProject.Report().ToString();
                var p = new Process { StartInfo = { FileName = file } };
                p.Start();
            }
            catch
            {
                return;
            }

        }
        private void DG_Output_Projects_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow row = e.Row;
            DataRowView rView = row.Item as DataRowView;
            if (rView != null && rView["Επιλογή"].ToString().Contains("Από Ρυθμίσεις"))
                row.Foreground = Brushes.Green;
            else
                row.Foreground = Brushes.Blue;

        }

        private void CreateRTF()
        {
            // Create document by specifying paper size and orientation, 
            // and default language.
            var doc = new RtfDocument(PaperSize.A4, PaperOrientation.Portrait, Lcid.TraditionalChinese);
            var times = doc.createFont("Times New Roman");
            var courier = doc.createFont("Arial");

            var red = doc.createColor(new DW.RtfWriter.Color("ff0000"));
            var blue = doc.createColor(new DW.RtfWriter.Color(System.Drawing.Color.Navy));

            // Don't instantiate RtfTable, RtfParagraph, and RtfImage objects by using
            // ``new'' keyword. Instead, use add* method in objects derived from 
            // RtfBlockList class. (See Demos.)
            RtfTable table;
            RtfParagraph par;
            RtfImage img;
            // Don't instantiate RtfCharFormat by using ``new'' keyword, either. 
            // An addCharFormat method are provided by RtfParagraph objects.
            RtfCharFormat fmt;


            // ==========================================================================
            // Demo 1: Font Setting
            // ==========================================================================
            // If you want to use Latin characters only, it is as simple as assigning
            // ``Font'' property of RtfCharFormat objects. If you want to render Far East 
            // characters with some font, and Latin characters with another, you may 
            // assign the Far East font to ``Font'' property and the Latin font to 
            // ``AnsiFont'' property. This Demo contains Traditional Chinese characters.
            // (Note: non-Latin characters are unicoded so you don't have to be worried.)

            img = doc.addImage("images/hellas.gif", ImageFileType.Gif);
            img.Alignment = Align.Center;
            img.Width = 60;

            par = doc.addParagraph();
            par.DefaultCharFormat.Font = doc.createFont("Arial");
            par.DefaultCharFormat.FontSize = 11;
            par.DefaultCharFormat.FgColor = blue;
            par.DefaultCharFormat.AnsiFont = courier;
            par.DefaultCharFormat.FontStyle.addStyle(FontStyleFlag.Bold);
            par.Alignment = Align.Center;
            par.setText("ΕΛΛΗΝΙΚΗ ΔΗΜΟΚΡΑΤΙΑ\n ΥΠΟΥΡΓΕΙΟ ΕΡΓΑΣΙΑΣ, ΚΟΙΝΩΝΙΚΗΣ ΑΣΦΑΛΙΣΗΣ ΚΑΙ ΠΡΟΝΟΙΑΣ\n ΓΕΝΙΚΗ ΓΡΑΜΜΑΤΕΙΑ ΔΙΑΧΕΙΡΙΣΗΣ ΚΟΙΝΟΤΙΚΩΝ ΚΑΙ ΑΛΛΩΝ ΠΟΡΩΝ\n ΕΙΔΙΚΗ ΥΠΗΡΕΣΙΑ ΕΦΑΡΜΟΓΗΣ\nΣΥΓΧΡΗΜΑΤΟΔΟΤΟΥΜΕΝΩΝ ΕΝΕΡΓΕΙΩΝ ΑΠΟ ΤΟ ΕΚΤ");

            par = doc.addParagraph();
            fmt = par.addCharFormat();
            fmt.FontStyle.addStyle(FontStyleFlag.Bold);
            par.Alignment = Align.Center;
            par.setText("\n\nΑΝΑΦΟΡΑ ΔΕΙΓΜΑΤΟΛΗΨΙΑΣ\n");

            par = doc.addParagraph();
            par.Alignment = Align.Center;
            par.setText(" ___________________________________________________________\n");

            par = doc.addParagraph();
            par.Alignment = Align.Center;
            par.setText("Ημερομηνία: " + this.CurrentProject.Date + "                   Ώρα: " + this.CurrentProject.Time);

            par = doc.addParagraph();
            par.Alignment = Align.Center;
            par.setText(" ___________________________________________________________\n");

            par = doc.addParagraph();
            par.Alignment = Align.FullyJustify;
            par.setText("Πραγματοποιήθηκε δειγματοληψία, όπως προβλέπεται από το άρθρο 3 της υπ’ αριθμ. 37156/18953/ 20.05.2008 ΚΥΑ, σε πληθυσμό " + CurrentProject.NumOfItems.ToString() + " προγραμμάτων του έργου «" + CurrentProject.title + "». Ως αποτέλεσμα της δειγματοληψίας, επιλέχθηκε δείγμα " + CurrentProject.NumOfSamples.ToString() + " προγραμμάτων στα οποία θα πραγματοποιηθεί από την αρμόδια Μονάδα Ελέγχου της ΕΥΕ επιτόπιος έλεγχος για τη διαπίστωση της ορθής υλοποίηση των συγχρηματοδοτούμενων ενεργειών και την τήρηση των υποχρεώσεων των Αναδόχων σύμφωνα με το ισχύον θεσμικό πλαίσιο.");


            par = doc.addParagraph();
            par.setText("\n\n");

            par = doc.addParagraph();
            par.DefaultCharFormat.FontStyle.addStyle(FontStyleFlag.Underline);
            par.Alignment = Align.Left;
            par.setText("Ρυθμίσεις δειγματοληψίας.\n\n");
           
            par = doc.addParagraph();
            par.Alignment = Align.FullyJustify;
            par.setText(CurrentProject.Settings + "\n\n");


            par = doc.addParagraph();
            par.DefaultCharFormat.FontStyle.addStyle(FontStyleFlag.Underline);
            par.Alignment = Align.Left;
            par.setText("Πίνακας επιλεγμένων δειγμάτων.\n");


            int tablerows = CurrentProject.ResultItems.Count;
            int x = 1;
            table = doc.addTable(tablerows+1, 7, 415.2f, 9);
            table.Margins[Direction.Bottom] = 20;
            table.setInnerBorder(BorderStyle.Dotted, 1.5f);
            table.setOuterBorder(BorderStyle.Dotted, 3f);
            table.Alignment = Align.Left;
            
            
            for (var j = 0; j < table.ColCount; j++)
            {
                table.cell(0, j).AlignmentVertical = AlignVertical.Middle;
                table.cell(0, j).DefaultCharFormat.FontStyle.addStyle(FontStyleFlag.Bold);
                table.cell(0, j).DefaultCharFormat.FontSize = 9;
            }
            table.cell(0, 0).Width = 20;
            table.cell(0, 0).addParagraph().setText("Α/Α");
            table.cell(0, 1).Width = 30;
            table.cell(0, 1).addParagraph().setText("Πράξη");
            table.cell(0, 2).Width = 120;
            table.cell(0, 2).addParagraph().setText("Επωνυμία Φορέα");
            table.cell(0, 3).Width = 35;
            table.cell(0, 3).addParagraph().setText("Υποέργο");
            table.cell(0, 4).Width = 40;
            table.cell(0, 4).addParagraph().setText("Κωδ.Πρ.");
            table.cell(0, 5).Width = 120;
            table.cell(0, 5).addParagraph().setText("Τίτλος Πρ.");
            table.cell(0, 6).Width = 70;
            table.cell(0, 6).addParagraph().setText("Περιφέρεια");
            for (var i = 1; i < table.RowCount; i++)
            {
                for (var j = 0; j < table.ColCount; j++)
                {
                    table.cell(i, j).AlignmentVertical = AlignVertical.Middle;
                    table.cell(i, j).DefaultCharFormat.FontSize = 8;
                }
                Item t = CurrentProject.ResultItems[i-1];
                table.cell(i, 0).Width = 20;
                table.cell(i, 1).Width = 30;
                table.cell(i, 2).Width = 120;
                table.cell(i, 3).Width = 35;
                table.cell(i, 4).Width = 40;
                table.cell(i, 5).Width = 120;
                table.cell(i, 6).Width = 70;
                table.cell(i, 0).addParagraph().setText(x++.ToString());
                table.cell(i, 1).addParagraph().setText(t.getProperty("praxi").value);
                table.cell(i, 2).addParagraph().setText(t.getProperty("eponimia").value);
                table.cell(i, 3).addParagraph().setText(t.getProperty("ypoergo").value);
                table.cell(i, 4).addParagraph().setText(t.getProperty("kod_programmatos").value);
                table.cell(i, 5).addParagraph().setText(t.getProperty("titlos_programmatos").value);
                table.cell(i, 6).addParagraph().setText(t.getProperty("periferia").value);
            }

            doc.save(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + CurrentProject.Report());
        }
        private void doMessage(BackgroundWorker worker, int startvalue, int step, string m)
        {
            string msg = m;
            msg = String.Format(msg, startvalue, step);
            worker.ReportProgress(step, msg);
        }
        private void Exec(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            int startValue = (int)e.Argument;

            doMessage(worker, startValue, 25, "Εκτέλεση αλγορίθμου δειγματοληψίας...");

            Table_Output_Items.Clear();
            if (CurrentProject.isMus)
                CurrentProject.doMusSampling();
            else
                CurrentProject.doRandomSampling();

            doMessage(worker, startValue, 50, "Εμφάνιση αποτελεσμάτων...");
            CurrentProject.ResultItems.ToResultDataTable(Table_Output_Items, CurrentProject.mf);

            Dispatcher.BeginInvoke(new Action(() => { DG_Output_Projects.ItemsSource = Table_Output_Items.AsDataView(); }));
            Dispatcher.BeginInvoke(new Action(() => { LB_Blue.Visibility = Visibility.Visible; }));
            Dispatcher.BeginInvoke(new Action(() => { LB_Green.Visibility = Visibility.Visible; }));
             

            //ΚΡΑΤΗΣΗ TABLE_PREFIX
            string n = DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString();
            CurrentProject.SetPrefix(n);

            try
            {

                //progressWindow.progress.Text=pr;
                //ΕΝΗΜΕΡΩΣΗ ΒΑΣΗΣ
                doMessage(worker, startValue, 75, "Αποθήκευση στη Βάση Δεδομένων...");
                if (!isTest)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary.Add("pr_table_prefix", n);
                    dictionary.Add("pr_settings", CurrentProject.Settings);
                    dictionary.Add("pr_title", CurrentProject.title);
                    db.Insert("Project", dictionary);

                    //Δημιουργία 2 tables: input και output      
                    db.ExecuteNonQuery("CREATE TABLE [" + CurrentProject.InputTable() + "] ([id_data] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL, [ekepis] VARCHAR(20), [eponimia] VARCHAR(1000)  NULL, [praxi] INTEGER  NULL, [ypoergo] INTEGER  NULL, [kod_programmatos] VARCHAR(200)  NULL, [titlos_programmatos] VARCHAR(1000)  NULL, [proipologismos] FLOAT  NULL, [periferia] VARCHAR(200)  NULL, [nomos] VARCHAR(100)  NULL, [dimos] VARCHAR(100)  NULL, [theoritiki_start_date] VARCHAR(15)  NULL, [theoritiki_end_date] VARCHAR(15)  NULL, [praktiki_start_date] VARCHAR(15)  NULL, [arithmos_katartizomenon] INTEGER  NULL, [plithos_elegxon] INTEGER  NULL, [synolo_poinon] FLOAT  NULL )");
                    Dispatcher.BeginInvoke(new Action(() => { db.DoFillData(CurrentProject.InputTable(), CurrentProject.InputItems, CurrentProject.mf); }));
                    db.ExecuteNonQuery("CREATE TABLE [" + CurrentProject.OutputTable() + "] ([id_data] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL, [ekepis] VARCHAR(20), [eponimia] VARCHAR(1000)  NULL, [praxi] INTEGER  NULL, [ypoergo] INTEGER  NULL, [kod_programmatos] VARCHAR(200)  NULL, [titlos_programmatos] VARCHAR(1000)  NULL, [proipologismos] FLOAT  NULL, [periferia] VARCHAR(200)  NULL, [nomos] VARCHAR(100)  NULL, [dimos] VARCHAR(100)  NULL, [theoritiki_start_date] VARCHAR(15)  NULL, [theoritiki_end_date] VARCHAR(15)  NULL, [praktiki_start_date] VARCHAR(15)  NULL, [arithmos_katartizomenon] INTEGER  NULL, [plithos_elegxon] INTEGER  NULL, [synolo_poinon] FLOAT  NULL )");
                    Dispatcher.BeginInvoke(new Action(() => { db.DoFillData(CurrentProject.OutputTable(), CurrentProject.ResultItems, CurrentProject.mf); })); 
                }

                doMessage(worker, startValue, 100, "Δημιουργία Σχετικών Αρχείων. Παρακαλώ περιμένετε...");
                //ΑΠΟΘΗΚΕΥΣΗ ΑΡΧΕΙΩΝ
                //Αρχικό
                System.IO.File.Copy(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/CURRENT_EXCEL/current.xls", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + CurrentProject.OriginalExcel(), true);
                Export.toXLS(Table_Sampling_Items, System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + CurrentProject.InputExcel());
                //Τελικό
                Export.toXLS(Table_Output_Items, System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + CurrentProject.OutputExcel());

                CreateRTF();

            }
            catch (Exception fail)
            {
                //pr += "\n Σφάλμα στον αλγόριθμο...\n";
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
                this.Close();
            }
            //progressWindow.Close();
        }

        #endregion

        #region TABHistory

        private void LV_History_Loaded(object sender, RoutedEventArgs e)
        {
            String query = "select * from project order by id_project desc;";
            DataTable dt = db.GetDataTable(query);

            // Create a collection for your types
            ProjectList.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Project p = new Project();
                p.mis = dt.Rows[i]["pr_mis"].ToString();
                p.title = dt.Rows[i]["pr_title"].ToString();
                p.Settings = dt.Rows[i]["pr_settings"].ToString();
                p.SetPrefix(dt.Rows[i]["pr_table_prefix"].ToString());
                ProjectList.Add(p);
            }
            LV_History.ItemsSource = ProjectList;
        }

        private void OpenFileOutput(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Project p = b.CommandParameter as Project;
            System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + p.OutputExcel());
        }
        private void OpenFileInput(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Project p = b.CommandParameter as Project;
            System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + p.OriginalExcel());
        }
        private void OpenFileReport(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Project p = b.CommandParameter as Project;
            string file = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/RECENT_SAMPLINGS/" + p.Report().ToString();
            var pr = new Process { StartInfo = { FileName = file } };
            pr.Start();
        }

        #endregion

        private void CB_Test_Checked(object sender, RoutedEventArgs e)
        {
            isTest = true;

        }

        private void CB_Test_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void CB_Test_Unchecked(object sender, RoutedEventArgs e)
        {
            isTest = false;
        }





















 





    }
}
