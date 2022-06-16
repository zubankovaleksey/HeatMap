using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BrowserController.model
{
    [DataContract]
    public class FrameTraectoryes 
    {
        [DataMember]
        public SessionFrame sessionFrame { get; set; }
        [DataMember]
        public List<Traectory> trajectoryList { get; set; }

        [DataMember]
        public long? time { get; set; }

        
    }
}
