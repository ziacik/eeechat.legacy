using System;
using System.Collections.Generic;
using System.Text;

namespace KolikSoftware.Eee.Service
{
    [global::System.Serializable]
    public class DisconnectedException : Exception
    {
        public DisconnectedException() { }
        protected DisconnectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
