using System;
using System.IO;

namespace UMServer
{
	public class Configuration
	{
		public static string DatabaseLocation
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data"); ;
			}
		}
	}
}
