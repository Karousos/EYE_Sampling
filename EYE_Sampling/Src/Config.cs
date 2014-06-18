using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace EYE_Sampling
{
 
    public class Mapfield  
    {
        public String Name;
        public String Type;
        public String Label;
        public int Sheet;
        public String Column;


        public Mapfield(string name)
        {
            Name = name;
            Type = "text";
            Label = "";
            Sheet = -1;
            Column = "-";
        }
        
        public Mapfield()
        {
            Name = "";
            Type = "text";
            Label = "";
            Sheet = -1;
            Column = "-";
        }
    }

    public class MapFieldList : List<Mapfield>
    {
        String filename;
        String AppDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public MapFieldList(string fname)
        {
            filename = fname;
            init();
        }

        private void init()
        {
            Load();
        }


        public int Save()
        {
            try
            {
                XmlWriter writer = XmlWriter.Create(AppDirectory+"\\"+filename);
                writer.WriteStartDocument();
                writer.WriteStartElement("SamplingConfiguration");

                writer.WriteStartElement("FieldMappings");
                foreach (Mapfield el in this)
                {                   
                    writer.WriteStartElement("MapField");
                    writer.WriteStartElement("Name");
                    writer.WriteString(el.Name);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Type");
                    writer.WriteString(el.Type);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Label");
                    writer.WriteString(el.Label);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Sheet");
                    writer.WriteString((el.Sheet==-1?"-":el.Sheet.ToString())); 
                    writer.WriteEndElement();
                    writer.WriteStartElement("Column");
                    writer.WriteString(el.Column);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                return 0;
            }
            catch
            {
                return -1;
            }
        }
        public int Load()
        {
            try
            {
                this.Clear();
                XmlTextReader m_xmlr = new XmlTextReader(AppDirectory + "\\"+filename);
                m_xmlr.WhitespaceHandling = WhitespaceHandling.None;
                m_xmlr.Read();
                string t = "";
                while (!m_xmlr.EOF)
                {
                    switch (m_xmlr.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (m_xmlr.Name == "SamplingConfiguration")
                            {
                                m_xmlr.Read();
                                continue;
                            }
                            if (m_xmlr.Name == "FieldMappings")
                            {
                                m_xmlr.Read();
                                continue;
                            }
                            if (m_xmlr.Name == "MapField")
                            {
                                bool EndOfElement = false;
                                Mapfield el = new Mapfield();
                                while (!EndOfElement)
                                {
                                    if (m_xmlr.Name == "Name")
                                    {
                                        el.Name = m_xmlr.ReadElementContentAsString();
                                        continue;
                                    }
                                    if (m_xmlr.Name == "Type")
                                    {
                                        el.Type = m_xmlr.ReadElementContentAsString();
                                        continue;
                                    }
                                    if (m_xmlr.Name == "Label")
                                    {
                                        el.Label = m_xmlr.ReadElementContentAsString();
                                        continue;
                                    }
                                    if (m_xmlr.Name == "Sheet")
                                    {
                                        t = m_xmlr.ReadElementContentAsString();
                                        el.Sheet = (t=="-"?-1:Int32.Parse(t));
                                        continue;
                                    }
                                    if (m_xmlr.Name == "Column")
                                    {
                                        el.Column = m_xmlr.ReadElementContentAsString();
                                        EndOfElement = true;
                                        continue;
                                    }
                                    m_xmlr.Read();
                                }
                                this.Add(el);
                            }
                            break;
                        default: m_xmlr.Read(); break;
                    }
                }
                m_xmlr.Close();
                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
    
}
