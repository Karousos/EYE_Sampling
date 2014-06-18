using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EYE_Sampling
{
    /// <summary>
    /// Interaction logic for Window_Praxi.xaml
    /// </summary>
    public partial class Window_Praxi : Window
    {
        public string selected_praxeis;

        public Window_Praxi()
        {
            InitializeComponent();
        }

        private void Window_Praxi_Loaded(object sender, RoutedEventArgs e)
        {

            if (selected_praxeis != "")
            {
                string[] r = selected_praxeis.Split(',');
                foreach (string s in r)
                {
                    ((ListBoxItem)LB_Praxeis.Items[Int32.Parse(s) - 1]).IsSelected = true;
                }
            }
        }

        private void Window_Praxi_Closed(object sender, EventArgs e)
        {

        }

        private void BT_OK_Click(object sender, RoutedEventArgs e)
        {
            selected_praxeis = "";
            foreach (ListBoxItem i in LB_Praxeis.SelectedItems)
            {
                selected_praxeis += i.Content.ToString().Substring(i.Content.ToString().Length-1,1) + ",";
            }
            if (selected_praxeis != "")
            {
                selected_praxeis = selected_praxeis.Substring(0, selected_praxeis.Length - 1);
            }
            this.Close();
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
