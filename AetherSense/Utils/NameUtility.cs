using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AetherSense.Utils
{
	public static class NameUtility
	{
		public static string ToName(string original)
		{
			return Regex.Replace(original, "([a-z])([A-Z]+$)|([A-Z]+)([A-Z])|([a-z])([A-Z])", "$1$3$5 $2$4$6");
		}
	}
}
