using System;

namespace KolikSoftware.Eee.Service
{
	public class ErrorEventArgs : EventArgs
	{
		protected string message;

		public string Message
		{
			get
			{
				return this.message;
			}
		}
		
		protected Exception exception;

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public ErrorEventArgs(string message, Exception exception)
		{
			this.message = message;
			this.exception = exception;
		}
	}

	public delegate void ErrorEventHanlder(object sender, ErrorEventArgs e);
}
