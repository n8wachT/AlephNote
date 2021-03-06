﻿using System;

namespace AlephNote.PluginInterface.Exceptions
{
	public class RemoteException : Exception
	{
		public RemoteException() { }
		public RemoteException(string message) : base(message) { }
		public RemoteException(string message, Exception inner) : base(message, inner) { }
	}
}
