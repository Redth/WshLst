using System;
using System.Collections.Generic;
using System.Drawing;
using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.Interfaces.ViewModels;
using Cirrious.MvvmCross.Touch.Interfaces;
using Cirrious.MvvmCross.Views;
using CrossUI.Touch.Dialog.Elements;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Media;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class BaseDialogViewController<TViewModel> : MvxTouchDialogViewController<TViewModel> where TViewModel : BaseViewModel
	{
		public BaseDialogViewController (MvxShowViewModelRequest request, UITableViewStyle style, RootElement root, bool pushing) 
			: base (request, style, root, pushing)
		{
		}

		LoadingHUDView _loadingView;

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			ViewModel.PropertyChanged += handlePropertyChanged;
		}
		public override void ViewDidUnload()
		{
			ViewModel.PropertyChanged -= handlePropertyChanged;
			base.ViewDidUnload();
		}

		public virtual void HandlePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
		}

		void handlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals ("IsLoading"))
			{
				if (ViewModel.IsLoading && _loadingView == null) 
				{
					_loadingView = new LoadingHUDView ("Loading...", "");
					View.AddSubview (_loadingView);
					_loadingView.StartAnimating ();
				} 
				else if (!ViewModel.IsLoading && _loadingView != null) 
				{
					_loadingView.StopAnimating ();
					_loadingView.RemoveFromSuperview ();
					_loadingView = null;
				}
			}

			HandlePropertyChanged(e);
		}
	}
}

