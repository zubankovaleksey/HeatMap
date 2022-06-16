using System.Runtime.Serialization;

namespace BrowserController.DB
{
    [DataContract]
    public class urlDto
    {
        [DataMember]
        public int siteId { get; set; }
        [DataMember]
        public string url {get;set;}
    }
}
 