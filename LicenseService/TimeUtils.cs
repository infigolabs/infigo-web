using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LicenseService
{
	public class TimeUtils
	{
		/// <summary>
		/// Return current time.
		/// </summary>
		/// <returns></returns>
		public static ulong Now()
		{
			TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
			return (ulong)t.TotalSeconds;
		}

		/// <summary>
		/// Return current time.
		/// </summary>
		/// <returns></returns>
		public static ulong Elapsed(ulong startTime)
		{
			ulong now = Now();
			return now - startTime;
		}
	}
}
