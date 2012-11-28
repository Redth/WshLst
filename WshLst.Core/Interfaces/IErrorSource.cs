using System;

namespace WshLst.Core.Interfaces
{
	public class ErrorEventArgs : EventArgs
	{
		public string Message { get; private set; }

		public ErrorEventArgs(string message)
		{
			Message = message;
		}
	}

	public interface IErrorSource
	{
		event EventHandler<ErrorEventArgs> ErrorReported;
	}
}