using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLocal.Client.Common {
	public class CriticalRemoteException : ApplicationException {

		public readonly string inner;

		public CriticalRemoteException(string inner)
		: base("Critical remote exception: " + inner) {
			this.inner = inner;
		}

	}
}
