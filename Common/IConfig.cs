using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLocal.Client.Common {
	public interface IConfig {

		string ServerUrl { get; }

		string Username { get; }

		string Password { get; }

	}
}
