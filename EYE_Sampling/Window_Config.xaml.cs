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
    /// Interaction logic for Window_Config.xaml
    /// </summary>
    public partial class Window_Config : Window
    {
        public MapFieldList Mappings;
        public Window_Config()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (Mapfield m in Mappings)
            {
                if (m.Name=="praxi") continue;

                Label Lb = new Label();
                Lb.Name = m.Name;
                Lb.Content = m.Label;
                Lb.Width = 140;
                Lb.Height = 30;
                double left = 10, top = 40+(i*30), right = 0, bottom = 0;
                Lb.Margin = new Thickness(left, top, right, bottom);
                Lb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                Lb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                MapGrid.Children.Add(Lb);
                MapGrid.RegisterName(Lb.Name, Lb);

                TextBox txtNumber = new TextBox();
                txtNumber.Name = "TB_" + m.Name;
                txtNumber.Text =m.Column.ToString();
                txtNumber.MinWidth = 50;
                left = 160;
                txtNumber.Margin = new Thickness(left, top, right, bottom);
                txtNumber.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                txtNumber.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                txtNumber.TextWrapping = TextWrapping.Wrap;
                MapGrid.Children.Add(txtNumber);
                MapGrid.RegisterName(txtNumber.Name, txtNumber);
                i++;
            }
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private String getColumnByName(string Name)
        {
            foreach (TextBox t in MapGrid.Children.OfType<TextBox>())
            {
                if (t.Name == "TB_" + Name)
                {
                    if (t.Text != "")
                        return t.Text;
                }
            }
            return "-";
        }

        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (Mapfield m in Mappings)
            {
                m.Column = getColumnByName(m.Name);
            }
            Mappings.Save();
            Close();
        }
    }
}
