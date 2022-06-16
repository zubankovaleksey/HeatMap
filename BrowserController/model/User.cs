using System.Runtime.Serialization;

namespace BrowserController.model
{
    [DataContract]
    public class User
    {
        public string fio => surname + " " + name + " " + patronymic;
        [DataMember]
        public long id { get; set; }
        [DataMember] 
        public string name { get; set; }
        [DataMember]
        public string surname { get; set; }
        [DataMember]
        public string patronymic { get; set; }
        [DataMember]
        public string dateOfBirth { get; set; }
        [DataMember]
        public bool gender { get; set; }

        public string sex => gender?"М":"Ж";
    }
}
