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
    /// Interaction logic for Window_Regions.xaml
    /// </summary>
    
    public partial class Window_Regions : Window
    {
        public string selected_regions;
        public List<string> Sel_Regions;
        public List<string> All_Regions;

        public Window_Regions()
        {
            InitializeComponent();
        }

        private TreeViewItem GetNomoPoli(TreeViewItem p, string s)
        {
            foreach (TreeViewItem x in p.Items)
            {
                if (((Label)((StackPanel)x.Header).Children[1]).Content.ToString() == s) return x;
            }
            return null;
        }
        private TreeViewItem GetPeriferia(string s)
        {
            foreach (TreeViewItem x in TV_Regions.Items)
            {
                if (((Label)((StackPanel)x.Header).Children[1]).Content.ToString() == s) return x;
            }
            return null;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            TV_Regions.Items.Clear();
            foreach (string r in All_Regions)
            {
                string[] s = r.Split('|');
                // Gia perifereia
                //tsek an einai idi mesa
                TreeViewItem cperiferia = GetPeriferia(s[0]);
                if (cperiferia==null)
                {
                    cperiferia = new TreeViewItem();
                    cperiferia.IsExpanded = true;
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    Label lbl = new Label();
                    lbl.Content = s[0];
                    CheckBox cb = new CheckBox();
                    sp.Children.Add(cb);
                    sp.Children.Add(lbl);
                    cperiferia.Header = sp;
                    TV_Regions.Items.Add(cperiferia);
                }
                // Gia nomo
                //tsek an einai idi mesa
                TreeViewItem cnomos = GetNomoPoli(cperiferia, s[1]);               
                if (cnomos==null)
                {
                    cnomos = new TreeViewItem();
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    Label lbl = new Label();
                    lbl.Content = s[1];
                    CheckBox cb = new CheckBox();
                    sp.Children.Add(cb);
                    sp.Children.Add(lbl);
                    cnomos.Header = sp;
                    cperiferia.Items.Add(cnomos);
                }
                // Gia nomo
                //tsek an einai idi mesa
                TreeViewItem cpoli = GetNomoPoli(cnomos, s[2]);
                if (cpoli == null)
                {
                    cpoli = new TreeViewItem();
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    Label lbl = new Label();
                    lbl.Content = s[2];
                    CheckBox cb = new CheckBox();
                    sp.Children.Add(cb);
                    sp.Children.Add(lbl);
                    cpoli.Header = sp;
                    cnomos.Items.Add(cpoli);
                }
            }
            if (Sel_Regions.Count > 0)
            {
                foreach (string r in Sel_Regions)
                {
                    string[] s = r.Split('|');
                    TreeViewItem p = GetPeriferia(s[0]);
                    if (s[1] == "*") { ((CheckBox)((StackPanel)p.Header).Children[0]).IsChecked = true; continue; }
                    TreeViewItem n = GetNomoPoli(p, s[1]);
                    if (s[2] == "*") { ((CheckBox)((StackPanel)n.Header).Children[0]).IsChecked = true; continue; }
                    TreeViewItem pol = GetNomoPoli(n, s[2]);
                        ((CheckBox)((StackPanel)pol.Header).Children[0]).IsChecked = true;

                }
            }

        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
        }

        private void BT_OK_Click(object sender, RoutedEventArgs e)
        {
            Sel_Regions.Clear();
            foreach (TreeViewItem i in TV_Regions.Items)
            {
                if (((CheckBox)((StackPanel)i.Header).Children[0]).IsChecked==true)
                    Sel_Regions.Add(((Label)((StackPanel)i.Header).Children[1]).Content.ToString() + "|*|*");
                else
                    foreach (TreeViewItem n in i.Items)
                    {
                        if (((CheckBox)((StackPanel)n.Header).Children[0]).IsChecked==true)
                            Sel_Regions.Add(((Label)((StackPanel)i.Header).Children[1]).Content.ToString() + "|"+((Label)((StackPanel)n.Header).Children[1]).Content.ToString()+"|*");
                        else
                            foreach (TreeViewItem p in n.Items)
                            {
                                if (((CheckBox)((StackPanel)p.Header).Children[0]).IsChecked==true)
                                    Sel_Regions.Add(((Label)((StackPanel)i.Header).Children[1]).Content.ToString() + "|"+((Label)((StackPanel)n.Header).Children[1]).Content.ToString()+"|"+((Label)((StackPanel)p.Header).Children[1]).Content.ToString());
                            }
                    }

            }
            this.Close();

        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
