using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KolikSoftware.Eee.Service
{
    public class BindServiceConfiguration : IServiceConfiguration
    {
        public BindServiceConfiguration()
        {
            this.MessageGetInstantRetryCount = 4;
            this.MessageGetRetryDelay = 30;
            this.MessageGetSafeInterval = 60;
        }

        public string ServiceUrl { get; set; }

        public int MessageGetInterval
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public int MessageGetInstantRetryDelay
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public int MessageGetSafeInterval { get; set; }
        public int MessageGetInstantRetryCount { get; set; }
        public int MessageGetRetryDelay { get; set; }
    }
}
