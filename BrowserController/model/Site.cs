using System.Runtime.Serialization;

namespace BrowserController.DB
{
    [DataContract]
    public class Site
    {
        [DataMember]
        public int id { get; set; } 
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string fileLocation { get; set; }
    }
}