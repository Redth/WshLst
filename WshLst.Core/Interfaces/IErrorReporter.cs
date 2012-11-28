using System;

namespace WshLst.Core.Interfaces
{
	public interface IErrorReporter
	{
		void ReportError(string error);
	}
}