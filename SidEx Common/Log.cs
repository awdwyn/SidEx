using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SidEx
{
    public static class Log
    {
		public enum LogFormat {
			Normal,
			Warning,
			Error,
			Screen
		}

		static public void Write(string s, LogFormat logFormat = LogFormat.Normal, bool always = false) {
			string a = Assembly.GetCallingAssembly().GetName().Name;
#if !DEBUG
			if (always)
#endif
				_Write($"[{a}]", s, logFormat);
		}

		static private void _Write(string a, string s, LogFormat logFormat = LogFormat.Normal) {
			switch (logFormat) {
				case LogFormat.Normal:
					Debug.LogFormat($"{a} {s}");
					break;
				case LogFormat.Warning:
					Debug.LogWarningFormat($"{a} {s}");
					break;
				case LogFormat.Error:
					Debug.LogErrorFormat($"{a} {s}");
					break;
				case LogFormat.Screen:
					if (KerbalismAPI.KerbalismInstalled)
						KerbalismAPI.Message(s);
					else
						ScreenMessages.PostScreenMessage(s, 3.0f, ScreenMessageStyle.LOWER_CENTER, false);
					break;
			}
		}

	}
}
