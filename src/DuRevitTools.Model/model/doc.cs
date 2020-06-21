using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DuRevitTools.Model
{
    public class doc
    {


        public assembly assembly { get; set; }

        public List<member> members { get; set; } = new List<member>();
    }

    public class assembly
    {
        [XmlElement]
        public string name { get; set; }
    }
}
