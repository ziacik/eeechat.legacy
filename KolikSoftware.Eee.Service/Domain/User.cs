using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace KolikSoftware.Eee.Service.Domain
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public UserState State { get; set; }
        [DataMember]
        public int Color { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public string Client { get; set; }
        [DataMember]
        public DateTime ConnectedSince { get; set; }
    }
}
