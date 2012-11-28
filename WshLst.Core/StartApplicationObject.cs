using Cirrious.MvvmCross.Interfaces.ViewModels;
using Cirrious.MvvmCross.ViewModels;
using WshLst.Core.ViewModels;

namespace WshLst.Core
{
	public class StartApplicationObject : MvxApplicationObject, IMvxStartNavigation
	{
		public void Start()
		{
			RequestNavigate<LoginViewModel>();
		}

		public bool ApplicationCanOpenBookmarks
		{
			get { return true; }
		}
	}
}