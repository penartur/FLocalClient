using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Specialized;

namespace FLocal.Client.Common.actions {
	public class ReplyAction {

		private readonly int parentPostId;
		private readonly int layerId;
		private readonly string title;
		private readonly string bodyUbb;

		public ReplyAction(int parentPostId, int layerId, string title, string bodyUbb) {
			this.parentPostId = parentPostId;
			this.layerId = layerId;
			this.title = title;
			this.bodyUbb = bodyUbb;
		}

		public int Process(RemoteSession session) {
			XDocument response = session.PostRequest(
				"/do/Reply/",
				new NameValueCollection {
					{ "parent", this.parentPostId.ToString() },
					{ "title", this.title },
					{ "layerId", this.layerId.ToString() },
					{ "Body", this.bodyUbb },
				}
			);
			return int.Parse(response.Root.Element("post").GetTextValue("id"));
		}

	}
}
