using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Data;


namespace EYE_Sampling
{
    public class property
    {
        public string name;
        public string value;

        public property(string n, string v)
        {
            name = n;
            value = v;
        }
    }

    public class Item
    {
        private List<property> properties;
        public bool enabled;
        public bool selected;

        public Item()
        {
            selected=false;
            enabled = true;
            properties = new List<property>();
        }

        public Item(DataTable t, int r, MapFieldList mf)
        {
            properties = new List<property>();
            for (int i = 0; i < t.Columns.Count; i++)
            {
                properties.Add(new property(mf[i].Name, t.Rows[r][i].ToString()));
            }
        }

        public property getProperty(string pname)
        {
            foreach (property p in properties)
            {
                if (p.name == pname) return p;
            }
            return null;
        }
        public property getProperty(int i)
        {
            return properties[i];
        }
    }

    public class ListOfItems : List<Item>
    {
        public ListOfItems()
        {
        }

        public void Load(DataTable t, MapFieldList mf)
        {
            for (int i = 0; i < t.Rows.Count; i++)
            {
                Item it = new Item(t, i, mf);
                this.Add(it);
            }
        }

        public void UnselectItems()
        {
            foreach (Item t in this)
                t.selected = false;
        }
        public void EnableItems()
        {
            foreach (Item t in this)
                t.enabled = true;
        }
        public void DisableItems()
        {
            foreach (Item t in this)
                t.enabled = false;
        }
        public void ClearCheck()
        {
            foreach (Item t in this)
            {
                t.getProperty("plithos_elegxon").value = "0" ;
                t.getProperty("synolo_poinon").value = "0";
            }
        }

        public void CopyEnabledItems(ListOfItems LI)
        {
            LI.Clear();
            foreach (Item i in this)
                if (i.enabled && !i.selected)
                    LI.Add(i);
        }

        public void ToDataTable(DataTable T, MapFieldList mf)
        {
            T.Columns.Clear();
            T.Clear();
            for (int i = 0; i < mf.Count; i++)
            {
                T.Columns.Add(mf[i].Label);
            }
            
            foreach (Item it in this)
            {
                var values = new object[mf.Count];
                for (int i = 0; i < mf.Count; i++)
                {
                    values[i] = it.getProperty(i).value;
                }
                T.Rows.Add(values);
            }
            

        }
        public void ToResultDataTable(DataTable T, MapFieldList mf)
        {
            T.Columns.Clear();
            T.Clear();
            for (int i = 0; i < mf.Count; i++)
            {
                T.Columns.Add(mf[i].Label);
            }
            T.Columns.Add("Επιλογή");

            foreach (Item it in this)
            {
                var values = new object[mf.Count+1];
                for (int i = 0; i < mf.Count; i++)
                {
                    values[i] = it.getProperty(i).value;
                }
                values[mf.Count] = (it.selected?"Από Ρυθμίσεις":"Δειγματοληπτικά");
                T.Rows.Add(values);
            }


        }

    }

}
