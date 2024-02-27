using System.Diagnostics;
using System.IO;

namespace UMServer
{
	public class Logger
	{
		static Logger()
		{
			if (File.Exists("log.txt"))
			{
				File.Delete("log.txt");
			}
		}

		public static void WriteLine(string log)
		{
			Debug.WriteLine(log);
			//Debug.WriteLine("log.txt", new string[] { log });
		}
	}
}
