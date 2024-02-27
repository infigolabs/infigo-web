using System;
using System.IO;

namespace LicenseService
{
	public class Configuration
	{
		public static string Manufacturer => "Web Shield";
		public static string ApplicationDataPath
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Manufacturer);
			}
		}

		public static string CurrentDirectory
		{
			get
			{
				return AppDomain.CurrentDomain.BaseDirectory;
			}
		}
	}
}
