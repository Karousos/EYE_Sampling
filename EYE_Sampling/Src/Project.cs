using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EYE_Sampling
{
    public class Project
    {
        private string prefix;
        private string _settings;
        public string title;
        public string mis;
        public ListOfItems InputItems;
        public ListOfItems SamplingItems;
        public ListOfItems ResultItems;
        public MapFieldList mf;
        public MapFieldList mf_check;
        public RuleList ParticipationRules;
        public RuleList SelectionRules;
        public bool isMus;
        public int NumOfSamples;
        public int NumOfPreSelected;
        public List<string> All_Regions = new List<string>();

        public Project()
        {
            isMus = false;
            mis = "";
            title = "";
            prefix = "";
            NumOfSamples = 0;
            NumOfPreSelected = 0;
            InputItems = new ListOfItems();
            SamplingItems = new ListOfItems();
            ResultItems = new ListOfItems();
            mf = new MapFieldList("config.xml");
            mf_check = new MapFieldList("config_check.xml");
            create_rules();
        }

        public int NumOfItems
        {
            get
            {
                return InputItems.Count;
            }
        }

        public string Settings
        {
            get
            {
                string s = "";
                s += "Συνολικός Αρχικός Πληθυσμός:" + NumOfItems.ToString() + "  " + "Ζητούμενα Δείγματα:" + ResultItems.Count.ToString() + "\n\n";
                s += ParticipationRules.ToString();
                s += SelectionRules.ToString();
                if (SelectionRules.NumOfEnabled > 0 && NumOfPreSelected > 0)
                    s += "Προεπιλεγμένα δείγματα:" + NumOfPreSelected.ToString() + "  ";
                s += "Πληθυσμός για δειγματοληψία:" + SamplingItems.Count.ToString() + "\n\n";
                s += "Μέθοδος Δειγματοληψίας: " + (isMus ? "Συστηματική με βαρύτητα σε προΰπολογισμό." : "Τυχαία") + "\n";
                return s;
            }
            set
            {
                _settings = value;
            }
        }

        public void Get_All_Regions()
        {
            All_Regions.Clear();
            foreach (Item i in InputItems)
            {
                string r = i.getProperty("periferia").value + "|" + i.getProperty("nomos").value + "|" + i.getProperty("dimos").value;
                All_Regions.Add(r);
            }
            All_Regions.Sort();
        }

        public string ProjectTitle
        {
            get
            {
                return title;
            }
        }
        
        public string ProjectDateTime
        { 
            get
            {
            string[] p=prefix.Split('_');
            return p[2] + "/" + p[1] + "/" + p[0] + " " + p[3] + ":" + p[4] + ":" + p[5];
            }
        }

        public string Date
        {
            get{
                string[] p=prefix.Split('_');
                return p[2] + "/" + p[1] + "/" + p[0] ;
            }
        }

        public string Time
        {
            get{
                string[] p=prefix.Split('_');
                return p[3] + ":" + p[4] + ":" + p[5];
            }
        }

        private void create_rules()
        {
            ParticipationRules = new RuleList("Participation");
            SelectionRules = new RuleList("Selection");
            ParticipationRules.Add(new PraxiRule());
            ParticipationRules.Add(new RegionRule());
            ParticipationRules.Add(new DateRule());
            ParticipationRules.Add(new MoneyRule());
            ParticipationRules.Add(new TheoriticalRule());
            ParticipationRules.Add(new StudentsRule());
            ParticipationRules.Add(new AnadoxoiRule());
            SelectionRules.Add(new YpAnadoxoiRule());
            SelectionRules.Add(new YpPoinesRule());
        }

        private int ApplyParticipationRules(ListOfItems LI, RuleList RL)
        {
            int NumOfEnabled = LI.Count();
            foreach (BaseRule r in RL)
            {
                if (!r.enabled) continue;
                foreach (Item t in LI)
                {
                    if (!t.enabled) continue;
                    if (!r.compare(t))
                    {
                        t.enabled = false;
                        NumOfEnabled--;
                    }
                }
            }
            return NumOfEnabled;

        }
        private int ApplySelectionRules(ListOfItems LI, RuleList RL)
        {
            int NumOfSelected = 0;
            foreach (BaseRule r in RL)
            {
                if (!r.enabled) continue;
                foreach (Item t in LI)
                {
                    if (!t.enabled) continue;
                    if (r.compare(t))
                    {
                        t.selected = true;
                        NumOfSelected++;
                    }
                }
            }
            return NumOfSelected;

        }

        public int ApplyRules(ListOfItems LI, RuleList RL)
        {
            switch (RL.type)
            {
                case "Participation":
                   LI.EnableItems();
                   return ApplyParticipationRules(LI, RL);
                case "Selection":
                   LI.UnselectItems();
                   return ApplySelectionRules(LI, RL);
            }
            return 0;
        }

        public static List<T> Shuffle<T>(List<T> list)
        {
            List<T> SamplingList = new List<T>();
            foreach (T x in list)
                SamplingList.Add(x);

            List<T> randomizedList = new List<T>();
            Random rnd = new Random();
            while (SamplingList.Count > 0)
            {
                int index = rnd.Next(0, SamplingList.Count); //pick a random item from the master list
                randomizedList.Add(SamplingList[index]); //place it at the end of the randomized list
                SamplingList.RemoveAt(index);
            }
            return randomizedList;
        }

        public void doRandomSampling()
        {
            ResultItems.Clear();
            List<Item> l = Shuffle(SamplingItems);
            foreach (Item s in InputItems)
                if (s.selected)
                    ResultItems.Add(s);
            for (int i = 0; i < NumOfSamples - NumOfPreSelected; i++)
                ResultItems.Add((Item)l[i]);
        }

        private void MUS(List<Item> InputList, List<Item> OutputList, int N)
        {
            double Total=0;
            List<double> Sums = new List<double>();
            foreach (Item I in InputList)
            {
                Total += double.Parse(I.getProperty("proipologismos").value.ToString());
                Sums.Add(Total);
            }

            int interval = (int)Math.Round(Total / N,0);
            Random r = new Random();
            double position = r.Next(1, interval);
            int positionItem;
            int selected = 0;

            while (selected < N)
            {
                positionItem = -1;
                for (int i = 0; i < Sums.Count; i++)
                {
                    if (position <= (double)Sums[i])
                    {
                        positionItem = i;
                        break;
                    };
                }
                bool exists = false;
                foreach (Item o in OutputList)
                {
                    if (o.Equals(InputList[positionItem]))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    OutputList.Add(InputList[positionItem]);
                    selected++;
                }
                position += interval;
                if (position > Total) position = position - Total;
            }
 
        }
        public void doMusSampling()
        {
            ResultItems.Clear();
            int NumOfSamplesFinal = NumOfSamples - NumOfPreSelected;
            List<Item> l = Shuffle(SamplingItems);
            foreach (Item s in InputItems)
                if (s.selected)
                    ResultItems.Add(s);
            MUS(l, ResultItems, NumOfSamplesFinal);
        }

        
        public void SetPrefix(string s)
        {
            prefix = s;
        }
        
        public string InputTable()
        {
            return "Input_"+prefix;
        }
        public string OutputTable()
        {
            return "Output_" + prefix;
        }
        public string OriginalExcel()
        {
            return prefix + "_OriginalExcel.xls";
        }
        public string InputExcel()
        {
            return prefix + "_InputExcel.xls";
        }
        public string OutputExcel()
        {
            return prefix + "_OutputExcel.xls";
        }
        public string Report()
        {
            return prefix + "_report.rtf";
        }
        public string AlgorithmSettings()
        {
            return prefix + "AlgorithmSettings.xml";
        }
    }
}
