using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KolikSoftware.Eee.Service
{
    public interface IServiceConfiguration
    {
        string ServiceUrl { get; set; }
        int MessageGetInterval { get; set; }
        int MessageGetInstantRetryCount { get; set; }
        int MessageGetInstantRetryDelay { get; set; }
        int MessageGetRetryDelay { get; set; }
    }
}
