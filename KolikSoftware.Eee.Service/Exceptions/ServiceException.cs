using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KolikSoftware.Eee.Service.Exceptions
{
    public class ServiceException : Exception
    {
        public enum ExceptionType
        {
            NoMessages,
            Other
        }

        public ExceptionType Type { get; private set; }
        public string OtherExceptionText { get; private set; }

        public ServiceException(string exceptionText)
        {
            try
            {
                this.Type = (ExceptionType)Enum.Parse(typeof(ExceptionType), exceptionText, true);
            }
            catch
            {
                this.Type = ExceptionType.Other;
                this.OtherExceptionText = exceptionText;
            }
        }

        public override string ToString()
        {
            if (this.Type != ExceptionType.Other)
                return this.Type.ToString();
            else
                return this.OtherExceptionText;
        }
    }
}
