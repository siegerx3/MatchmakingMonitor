using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
	public static class Extensions
	{
		public static List<Version> Sort(this IEnumerable<Version> versions)
		{
			var list = versions.ToList();
			list.Sort((x, y) => y.CompareTo(x));
			return list;
		}
	}
}
