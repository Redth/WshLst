using System;
using Cirrious.MvvmCross.ViewModels;
using WshLst.Core.Interfaces;

namespace WshLst.Core
{
	public class ErrorApplicationObject : MvxApplicationObject, IErrorReporter, IErrorSource
	{
		public void ReportError(string error)
		{
			if (ErrorReported == null)
				return;

			InvokeOnMainThread(() =>
				{
					var handler = ErrorReported;
					if (handler != null)
						handler(this, new ErrorEventArgs(error));
				});
		}

		public event EventHandler<ErrorEventArgs> ErrorReported;
	}
}