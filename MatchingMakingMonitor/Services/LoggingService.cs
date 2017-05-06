using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.Services
{
	public class LoggingService
	{
		public void Log(string message)
		{
			try
			{
				using (var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Log.txt", true))
				{
					sw.WriteLine(DateTime.Now + " - " + message);
				} //end using
			} //end try
			catch (Exception ex)
			{
				//ignore
			} //end catch
		} //end Log
	}
}
