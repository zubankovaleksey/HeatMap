using System.Runtime.Serialization;

namespace BrowserController.model
{
    [DataContract]
    public class Session
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public long pageShowTime { get; set; }
        [DataMember] 
        public long siteId { get; set; }
        [DataMember]
        public long userId { get; set; }
    }
}
