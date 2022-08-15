using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class Complaint
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string Complainter { get; set; }
    }
}