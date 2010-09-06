using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using FLocal.Client.Common;

namespace FLocal.Client.PhotoBlog {
	class Config : IConfig {

		private Config() {
		}

		public static readonly Config instance = new Config();

		public string ServerUrl {
			get {
				return ConfigurationManager.AppSettings["ServerUrl"];
			}
		}

		public string Username {
			get {
				return ConfigurationManager.AppSettings["Username"];
			}
		}

		public string Password {
			get {
				return ConfigurationManager.AppSettings["Password"];
			}
		}

		public int mainPostId {
			get {
				return int.Parse(ConfigurationManager.AppSettings["MainPostId"]);
			}
		}

	}
}
