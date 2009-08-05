using System;
using System.Collections.Generic;
using System.Text;

namespace KolikSoftware.Eee.Service
{
    public class MessageAvailableEventArgs : EventArgs
    {
        public EeeDataSet.MessageRow Message { get; private set; }

        public MessageAvailableEventArgs(EeeDataSet.MessageRow message)
        {
            this.Message = message;
        }
    }
}
