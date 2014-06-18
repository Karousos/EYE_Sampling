using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SQLite;

namespace EYE_Sampling
{
    /// <summary>
    /// Interaction logic for Window_Login.xaml
    /// </summary>
    public partial class Window_Login : Window
    {
        public SQLiteDatabase database;
        public string username = "";

        public Window_Login()
        {
            InitializeComponent();
        }

        private string striplogin(string h)
        {
            h = h.Replace("'", "");
            h = h.Replace("\"", "");
            h = h.Replace(";", "");
            return h;
        }

        private void BT_FileImport_Click(object sender, RoutedEventArgs e)
        {
            username = "";
            string u = database.ExecuteScalar("select id_user from User where username='" + striplogin(TB_Username.Text) + "' and password='" + striplogin(TB_Password.Password) + "';");
            if (u != "")
            {
                username = TB_Username.Text;
                this.Close();
            }
            else
                LB_Error.Visibility = System.Windows.Visibility.Visible;        
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            TB_Username.Focus();
        }

        private void TB_Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BT_Login.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}
