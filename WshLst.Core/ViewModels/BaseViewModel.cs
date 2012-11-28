using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using WshLst.Core.Interfaces;

namespace WshLst.Core.ViewModels
{
	public abstract class BaseViewModel : MvxViewModel, IMvxServiceConsumer
	{
		
		bool isLoading = false;
		public bool IsLoading
		{
			get { return isLoading; }
			set { isLoading = value; RaisePropertyChanged("IsLoading"); }
		}

		public void ReportError(string error)
		{
			this.GetService<IErrorReporter>().ReportError(error);
		}
	}
}