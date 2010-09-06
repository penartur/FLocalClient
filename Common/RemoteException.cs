using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FLocal.Client.Common {
	public class RemoteException : ApplicationException {

		public readonly string remoteType;
		public readonly string remoteMessage;
		public readonly string remoteGuid;
		public readonly string remoteTrace;

		public RemoteException(XElement info)
		: base("Remote exception: " + info.GetTextValue("message")) {
			this.remoteType = info.GetTextValue("type");
			this.remoteMessage = info.GetTextValue("message");
			this.remoteGuid = info.GetTextValue("guid");
			this.remoteTrace = info.GetTextValue("trace");
		}

	}
}
