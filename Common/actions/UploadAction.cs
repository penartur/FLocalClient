using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace FLocal.Client.Common.actions {
	public class UploadAction {

		private readonly string filePath;

		public UploadAction(string filePath) {
			this.filePath = filePath;
		}

		public int Process(RemoteSession session) {
			XDocument response = session.UploadFile(
				"/do/Upload/",
				this.filePath,
				new FileStream(this.filePath, FileMode.Open, FileAccess.Read),
				"file",
				Util.getMimeByExtension(Path.GetExtension(this.filePath)),
				new System.Collections.Specialized.NameValueCollection()
			);
			return int.Parse(response.Root.GetTextValue("uploadedId"));
		}

	}
}
