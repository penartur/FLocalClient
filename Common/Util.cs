using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLocal.Client.Common {
	class Util {

		private static Dictionary<string, string> extension2mime = new Dictionary<string,string>();
		public static string getMimeByExtension(string extension) {
			if(!extension.StartsWith(".")) extension = "." + extension;
			if(!extension2mime.ContainsKey(extension)) {
				lock(extension2mime) {
					if(!extension2mime.ContainsKey(extension)) {
						Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);
						if (regKey != null && regKey.GetValue("Content Type") != null) {
							extension2mime[extension] = regKey.GetValue("Content Type").ToString();
						} else {
							return null;
						}
					}
				}
			}
			return extension2mime[extension];
		}

	}
}
