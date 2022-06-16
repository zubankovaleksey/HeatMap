using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BrowserController.model
{
    [DataContract]
    public class Page
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public List<List<string>> priority { get; set; }
        [DataMember] 
        public string name { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public int siteId { get; set; }
        [DataMember]
        public bool startPage { get; set; }
    }
}
