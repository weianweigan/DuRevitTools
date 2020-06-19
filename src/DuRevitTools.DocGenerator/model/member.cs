using System.Collections.Generic;
using System.Xml.Serialization;

namespace GStarGenerator
{
    public enum memberType
    {
        /// <summary>
        /// type
        /// </summary>
        T,
        /// <summary>
        /// methds
        /// </summary>
        M,
        /// <summary>
        /// properties
        /// </summary>
        P
       
    }

    public class member
    {
        private readonly memberType _memberType;
        private string _name;

        public member()
        {
        }

        public member(memberType memberType)
        {
            _memberType = memberType;
        }

        public memberType memberType { get => _memberType; }

        ///<summary>命名空间</summary>
        [XmlAttribute]
        public string name { get => _name; set => _name = value; }

        [XmlElement]
        public string summary { get; set; }

        [XmlArray]
        public List<param> @params { get; set; } = new List<param>();

        [XmlArray]
        public List<typeparam> typeparams { get; set; } = new List<typeparam>();

        public string returns { get; set; }

        public string remarks { get; set; }
    }


    public class param
    {
        [XmlAttribute]
        public string name { get; set; }

        [XmlText]
        public string value { get; set; }

        public override string ToString()
        {
            return value;
        }
    }

    public class typeparam
    {
        [XmlAttribute]
        public string name { get; set; }

        public string value { get; set; }
    }
}
