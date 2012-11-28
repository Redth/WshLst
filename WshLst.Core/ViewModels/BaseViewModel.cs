using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.ExtensionMethods;
using WshLst.Core.Interfaces;

namespace WshLst.Core.ViewModels
{
	public abstract class BaseViewModel : MvxViewModel
	{
		private bool _isLoading;

		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				_isLoading = value;
				RaisePropertyChanged(() => IsLoading);
			}
		}

		public void ReportError(string error)
		{
			this.GetService<IErrorReporter>().ReportError(error);
		}
	}
}