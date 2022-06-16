using System.Runtime.Serialization;

namespace BrowserController.model
{
    [DataContract]
    public class SessionFrame
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public long pageId { get; set; } 
        [DataMember]
        public long sessionId { get; set; }

        [DataMember]
        public long? time { get; set; }

        [DataMember]
        public double? first { get; set; }

        [DataMember]
        public double? second { get; set; }

        [DataMember]
        public double? third { get; set; }

        [DataMember]
        public double? firstCursor { get; set; }

        [DataMember]
        public double? secondCursor { get; set; }

        [DataMember]
        public double? thirdCursor { get; set; }
    }
}
