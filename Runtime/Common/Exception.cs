using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	public class TimeoutException : System.Exception
	{
		public TimeoutException(string msg) : base(msg) { }
	}

	public class AbortException : System.Exception
	{
		public AbortException(string msg) : base(msg) { }
	}

	public class NotPoolException : System.Exception
	{
		public NotPoolException(string msg) : base(msg) { }
	}
}
