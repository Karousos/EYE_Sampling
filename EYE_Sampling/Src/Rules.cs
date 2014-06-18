using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EYE_Sampling
{
    abstract public class BaseRule
    {
        public string name;
        public string type;
        protected bool _enabled;
        public abstract bool enabled { get; set; }
        public abstract bool compare(Item t);
        public abstract string ToString();
    }

    public class PraxiRule : BaseRule
    {
        public List<string> praxeis;
        public override bool enabled
        {
            get
            {
                return (praxeis.Count > 0 && _enabled);
            }
            set
            {
                _enabled = value;
            }
        }

        public PraxiRule()
        {
            _enabled = false;
            name = "Praxi";
            type = "Participation";
            praxeis = new List<string>();
        }

        public void addPraxi(string r)
        {
            praxeis.Add(r);
        }

        override public bool compare(Item t)
        {
            if (praxeis.Count == 0) return true;
            if (praxeis[0].ToString() == "All") return true;
            string itemregion = t.getProperty("praxi").value;
            foreach (string r in praxeis)
            {
                if (r == itemregion) return true;
            }
            return false;
        }

        public override string ToString()
        {
            string allRegions = "";
            if (praxeis.Count > 0)
            {
                foreach (string r in praxeis)
                    allRegions += r + ", ";
                allRegions = allRegions.Substring(0, allRegions.Length - 2);
            }
            return "Να συμμετέχουν τα projects που υλοποιούνται στις εξής πράξεις:" + allRegions + ".";
        }

    }
    public class RegionRule : BaseRule
    {
        public List<string> regions;
        public override bool enabled
        {
            get
            {
                return (regions.Count > 0 && _enabled);
            }
            set
            {
                _enabled = value;
            }
        }

        public RegionRule()
        {
            _enabled = false;
            name="Region";
            type="Participation";
            regions = new List<string>();
        }

        override public bool compare(Item t)  
        {
            if (regions.Count == 0) return false;
            foreach (string s in regions)
            {
                string[] ss = s.Split('|');
                if (ss[0] == t.getProperty("periferia").value)
                    if (ss[1] == "*") return true;
                    else
                        if (ss[1] == t.getProperty("nomos").value)
                            if (ss[2] == "*") return true;
                            else
                                if (ss[2] == t.getProperty("dimos").value) return true;
            }
            return false;
        }

        public override string ToString()
        {
            string allRegions="";
            if (regions.Count>0)
            {
                //πρέπει να γίνει πιο ευανάγνωστο.
                foreach (string r in regions)
                {
                    string[] s=r.Split('|');
                    if (s[2]=="*")
                        if (s[1]=="*")
                            allRegions+="\n\tΠεριφέρεια: "+s[0]+", όλοι οι Νομοί και Δήμοι";
                        else
                            allRegions += "\n\tΠεριφέρεια: "+s[0]+", Νομός: " + s[1] +", όλοι οι Δήμοι";
                    else
                        allRegions += "\n\tΠεριφέρεια: " + s[0] + ", Νομός: " + s[1] + ", Δήμος: " + s[2];
                }
            }
            return "Να συμμετέχουν τα projects που υλοποιούνται στις εξής περιοχές:" + allRegions+".";
        }

    }
    public class DateRule : BaseRule
    {
        public string StartDate;
        public string StopDate;
        public override bool enabled
        {
            get
            {
                return (StartDate != "" && StopDate != "" && _enabled);
            }
            set
            {
                _enabled = value;
            }

        }

        public DateRule()
        {
            StartDate = "";
            StopDate = "";
            _enabled = false;
            name = "Date";
            type = "Participation";
        }

        override public bool compare(Item t)
        {
            string end = t.getProperty("theoritiki_start_date").value;
            if ((DateTime.Parse(StartDate) < DateTime.Parse(end))) return true; //&& DateTime.Parse(end) < DateTime.Parse(StopDate)
            return false;
        }

        public override string ToString()
        {
            return "Να είναι σε εξέλιξη το χρονικό διάστημα από " + DateTime.Parse(StartDate).ToString("dd'/'MM'/'yyyy") + " έως " + DateTime.Parse(StopDate).ToString("dd'/'MM'/'yyyy");
        }

    }
    public class MoneyRule : BaseRule
    {
        public double limit_from;
        public double limit_to;
        public override bool enabled
        {
            get
            {
                return (_enabled);
            }
            set
            {
                _enabled = value;
            }

        }

        public MoneyRule()
        {
            limit_from = 0;
            limit_to = 50000;
            _enabled = false;
            name = "Money";
            type = "Participation";
        }

        override public bool compare(Item t)
        {
            double money = Double.Parse(t.getProperty("proipologismos").value);
            if (limit_from<=money && money<=limit_to) return true;
            return false;
        }

        public override string ToString()
        {
            return "Ο προυπολογισμός τους να είναι από " + limit_from.ToString() + " έως και "+ limit_to.ToString()+" ευρώ.";
        }

    }
    public class TheoriticalRule : BaseRule
    {
        public bool istheoritical;
        public override bool enabled
        {
            get
            {
                return (_enabled);
            }
            set
            {
                _enabled = value;
            }

        }

        public TheoriticalRule()
        {
            istheoritical = true;
            _enabled = false;
            name = "Theoritical";
            type = "Participation";
        }

        override public bool compare(Item t)
        {
            string th = t.getProperty("theoritiki_start_date").value;
            string pr = t.getProperty("praktiki_start_date").value;
            if (istheoritical)
                if (th != "" && pr == "") return true;
                else
                    return false;
            else
                if (th == "" && pr != "") return true;
                else
                    return false;
        }

        public override string ToString()
        {
            return "Να αφορούν μόνο σε " + (istheoritical ? "Θεωρητική κατάρτιση" : "πρακτική άσκηση")+".";
        }

    }
    public class StudentsRule : BaseRule
    {
        public double limit;
        public override bool enabled
        {
            get
            {
                return (limit != 0 && _enabled);
            }
            set
            {
                _enabled = value;
            }

        }

        public StudentsRule()
        {
            limit = 0;
            _enabled = false;
            name = "Students";
            type = "Participation";
        }

        override public bool compare(Item t)
        {
            int workers = Int32.Parse(t.getProperty("arithmos_katartizomenon").value);
            if (workers > limit) return true;
            return false;
        }

        public override string ToString()
        {
            return "Ο αριθμός των καταρτιζόμενων στην επιχείρηση που θα ελεγχθεί να είναι μεγαλύτερος του " + limit.ToString()+".";
        }

    }
    public class AnadoxoiRule : BaseRule
    {
        public double limit;
        public override bool enabled
        {
            get
            {
                return (_enabled);
            }
            set
            {
                _enabled = value;
            }

        }

        public AnadoxoiRule()
        {
            limit = 0;
            _enabled = false;
            name = "Anadoxoi";
            type = "Participation";
        }

        override public bool compare(Item t)
        {
            int workers = Int32.Parse(t.getProperty("plithos_elegxon").value);
            if (workers >= limit) return false;
            return true;
        }

        public override string ToString()
        {
            return "Να μή συμμετέχουν ανάδοχοι που την τελευταία διετία έχουν ελεγχθεί τουλάχιστον " + limit.ToString() + " φορές.";
        }

    }
    public class YpAnadoxoiRule : BaseRule
    {
        public double limit;
        public override bool enabled
        {
            get
            {
                return (_enabled);
            }
            set
            {
                _enabled = value;
            }

        }

        public YpAnadoxoiRule()
        {
            limit = 0;
            _enabled = false;
            name = "YpAnadoxoi";
            type = "Selection";
        }

        override public bool compare(Item t)
        {
            int workers = Int32.Parse(t.getProperty("plithos_elegxon").value);
            if (workers < limit) return true;
            return false;
        }

        public override string ToString()
        {
            return "Να επιλεγούν ανάδοχοι που την τελευταία διετία έχουν ελεγχθεί λιγότερο από " + limit.ToString() + " φορές.";
        }

    }
    public class YpPoinesRule : BaseRule
    {
        public double limit;
        public override bool enabled
        {
            get
            {
                return (_enabled);
            }
            set
            {
                _enabled = value;
            }

        }

        public YpPoinesRule()
        {
            limit = 0;
            _enabled = false;
            name = "YpPoines";
            type = "Selection";
        }

        override public bool compare(Item t)
        {
            double workers = double.Parse(t.getProperty("synolo_poinon").value);
            if (workers > limit) return true;
            return false;
        }

        public override string ToString()
        {
            return "Να επιλεγούν ανάδοχοι που την τελευταία διετία έχουν επιβληθεί ποινές πάνω από " + limit.ToString() + " ευρώ.";
        }

    }

    public class RuleList : List<BaseRule>
    {
        public string type;

        public int NumOfEnabled
        {
            get
            {
                int i=0;
                foreach (BaseRule r in this)
                    if (r.enabled) i++;
                return i;
            }
        }

        public RuleList(string t)
        {
            type = t;
        }

        public BaseRule getRule(string name)
        {
            foreach (BaseRule r in this)
            {
                if (r.name == name) return r;
            }
            return null;
        }

        public override string ToString()
        {
            string str="";
            if (NumOfEnabled == 0) return "";
            if (type == "Participation" && Count>0)
                str = "Ρυθμίσεις συμμετοχής:\n";
            else
                if (type == "Selection" && Count > 0)
                    str = "Ρυθμίσεις Επιλογής:\n";
            foreach (BaseRule r in this)
            {
                if (r.enabled) str += "-  "+r.ToString() + "\n"; 
            }
            return str+"\n";
        }

    }
}
