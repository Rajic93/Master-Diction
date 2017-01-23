using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Diction_Master___Library
{
    [XmlInclude(typeof(LeafComponent))]
    [XmlInclude(typeof(CompositeComponent))]
    public abstract class Component
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
    }
}
