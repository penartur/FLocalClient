using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace FLocal.Client.Common {
	static class Connector {

		private static string UrlEncode(this string value) {
			return HttpUtility.UrlEncode(value, Encoding.UTF8);
		}

		private static byte[] ToByteArray(this NameValueCollection nvc) {
			string query = (
				from string key in nvc.Keys
				from value in nvc.GetValues(key)
				select string.Format("{0}={1}", key.UrlEncode(), value.UrlEncode())
			).Join("&");
			return Encoding.ASCII.GetBytes(query);
		}

		/// <summary>
		/// Code taken from http://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data
		/// </summary>
		private static string HttpUploadFile(string url, string fileName, Stream fileStream, string paramName, string contentType, NameValueCollection postData, CookieContainer cookies) {

			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

			HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
			wr.ContentType = "multipart/form-data; boundary=" + boundary;
			wr.Method = "POST";
			wr.KeepAlive = true;
			wr.CookieContainer = cookies;
			wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
			wr.Referer = url;

			Stream rs = wr.GetRequestStream();

			string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
			foreach (string key in postData.Keys)
			{
				rs.Write(boundarybytes, 0, boundarybytes.Length);
				string formitem = string.Format(formdataTemplate, key, postData[key]);
				byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
				rs.Write(formitembytes, 0, formitembytes.Length);
			}
			rs.Write(boundarybytes, 0, boundarybytes.Length);

			string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			string header = string.Format(headerTemplate, paramName, fileName, contentType);
			byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
			rs.Write(headerbytes, 0, headerbytes.Length);

			byte[] buffer = new byte[4096];
			int bytesRead = 0;
			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
				rs.Write(buffer, 0, bytesRead);
			}

			byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
			rs.Write(trailer, 0, trailer.Length);
			rs.Close();

			WebResponse wresp = null;
			try {
				wresp = wr.GetResponse();
				Stream stream2 = wresp.GetResponseStream();
				StreamReader reader2 = new StreamReader(stream2);
			} catch(Exception) {
				if(wresp != null) {
					wresp.Close();
					wresp = null;
				}
				throw;
			} finally {
				wr = null;
			}

			using(Stream responseStream = wresp.GetResponseStream()) {
				using(StreamReader reader = new StreamReader(responseStream)) {
					return reader.ReadToEnd();
				}
			}
		}
		
		private static string HttpPostRequest(string fullRequestUrl, NameValueCollection postData, CookieContainer cookies) {
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			try {
				request = (HttpWebRequest)WebRequest.Create(fullRequestUrl);
				request.KeepAlive = true;
				request.CookieContainer = cookies;
				request.ReadWriteTimeout = 3*1000;
				request.Timeout = 3*1000;
				request.Accept = "*";
				request.Referer = fullRequestUrl;
				if((postData == null) || (postData.Count < 1)) {
					request.Method = "GET";
				} else {

					byte[] postBytes = postData.ToByteArray();

					request.Method = "POST";
					request.ContentType = "application/x-www-form-urlencoded";
					request.ContentLength = postBytes.Length;

					Stream stream = request.GetRequestStream();
					stream.Write(postBytes, 0, postBytes.Length);
					stream.Close();
				}
				using(response = (HttpWebResponse)request.GetResponse()) {
					cookies.Add(response.Cookies);

					string result;
					using(Stream responseStream = response.GetResponseStream()) {
						using(StreamReader reader = new StreamReader(responseStream)) {
							result = reader.ReadToEnd();
						}
					}

					return result;
				}
			} finally {
				if(response != null) {
					response.Close();
				}
			}
		}

		private static XDocument ProcessResult(string result) {
			XDocument data;
			try {
				data = XDocument.Parse(result);
			} catch(Exception) {
				throw new CriticalRemoteException(result);
			}
			
			XElement exceptionInfo = data.Root.Element("exception");
			if(exceptionInfo != null) {
				throw new RemoteException(exceptionInfo);
			}

			return data;
		}

		public static XDocument UploadFile(string url, string fileName, Stream fileStream, string paramName, string contentType, NameValueCollection postData, CookieContainer cookies) {
			return ProcessResult(HttpUploadFile(url, fileName, fileStream, paramName, contentType, postData, cookies));
		}

		public static XDocument PostRequest(string fullRequestUrl, NameValueCollection postData, CookieContainer cookies) {
			return ProcessResult(HttpPostRequest(fullRequestUrl, postData, cookies));
		}

	}
}
