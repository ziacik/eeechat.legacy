using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace KolikSoftware.Eee.Service.Domain
{
    [DataContract]
    public class ActionResult
    {
        [DataMember]
        public string Result { get; set; }
    }
}
