using System;
using System.IO;
using System.Reflection;

namespace KolikSoftware.Eee.Service
{
	public class Notificator
	{
		public event ErrorEventHanlder Error;

		public Notificator()
		{
		}

		public void ReportError(object sender, string message, Exception ex)
		{
			LogError(ex, message);
			OnError(sender, new ErrorEventArgs(message, ex));
		}

		protected virtual void OnError(object sender, ErrorEventArgs args)
		{
			if (Error != null)
				Error(sender, args);
		}

		protected void LogError(Exception ex, string message)
		{
			try 
			{
				string logDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Log");

                if (Directory.Exists(logDir) == false)
                    Directory.CreateDirectory(logDir);

				string fileName = "lasterror.log";
				string path = Path.Combine(logDir, fileName);

				string error = "\n================================\n";

				if (ex == null)
					ex = new Exception("(No details)");

				while (ex != null) 
				{
					error += "Error: " + message + "\n" + ex.Message + "\n";
					error += "\n";
					error += ex.StackTrace + "\n";
					error += "------------------------------------\n\n";

					ex = ex.InnerException;
				}
			
				error += "================================\n\n";

				lock (this) 
				{
					using (StreamWriter w = new StreamWriter(path, false)) 
					{
						w.Write("{0}: {1}", DateTime.Now.ToString(), Convert.ToString(error) );
					}
				}			
			} 
			catch
			{
			}
		}
	}
}
