using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace FLocal.Client.Common {
	public class RemoteSession : IDisposable {

		private readonly IConfig config;
		private readonly CookieContainer cookies;
		private bool isOpened = false;
		private string sessionKey {
			get {
				return this.cookies.GetCookies(new Uri(this.config.ServerUrl))["session"].Value;
			}
		}

		public RemoteSession(IConfig config) {
			this.config = config;
			this.cookies = new CookieContainer();
			this.PostRequest(
				"/do/Login/",
				new NameValueCollection {
					{ "name", this.config.Username },
					{ "password", this.config.Password },
				}
			);
			this.isOpened = true;
		}

		internal XDocument PostRequest(string path, NameValueCollection postData) {
			return Connector.PostRequest(this.config.ServerUrl + path, postData, this.cookies);
		}

		internal XDocument UploadFile(string path, string fileName, System.IO.Stream fileStream, string paramName, string contentType, NameValueCollection postData) {
			return Connector.UploadFile(this.config.ServerUrl + path, fileName, fileStream, paramName, contentType, postData, this.cookies);
		}

		public void Close() {
			if(this.isOpened) {
				this.PostRequest("/do/Logout/?sessionKey=" + this.sessionKey, null);
				this.isOpened = false;
			}
		}
		
		void IDisposable.Dispose() {
			this.Close();
		}

	}
}
