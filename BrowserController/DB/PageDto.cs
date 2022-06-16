using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BrowserController.DB
{
    [DataContract]
    public class PageDto
    {
        [DataMember]
        public int id {get;set;}
        [DataMember]
        public string name {get;set;}
        [DataMember]
        public string url {get;set;}  
        [DataMember]
        public int siteId {get;set;}
        [DataMember]
        public bool isStartPage {get;set;}
        [DataMember]
        public List<List<string>> priority { get; set; }
    }
}