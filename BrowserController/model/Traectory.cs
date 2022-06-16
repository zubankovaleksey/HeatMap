using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace BrowserController.model
{
    [DataContract]
    public class Traectory
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public List<Point> points { get; set; } 
        [DataMember]
        public long sessionFrameId { get; set; }
    }
}
