using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FLocal.Client.Common {
	static class Extensions {

		public static string Join(this IEnumerable<string> value, string separator) {
			return string.Join(separator, value.ToArray());
		}

		public static string GetTextValue(this IEnumerable<XElement> elements) {
			return (from element in elements select element.Value).Join("");
		}

		public static string GetTextValue(this XElement element, XName name) {
			return element.Elements(name).GetTextValue();
		}

	}
}
