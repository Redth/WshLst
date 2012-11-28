
using System;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.Views;
using Cirrious.MvvmCross.Touch.Interfaces;
using CrossUI.Touch.Dialog.Elements;

using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class EditWishListView : MvxTouchDialogViewController<EditWishListViewModel>
	{
		public EditWishListView (MvxShowViewModelRequest request) : base (request)
		{
		}
	
		UIBarButtonItem buttonSave = new UIBarButtonItem(UIBarButtonSystemItem.Done);
		UIBarButtonItem buttonCancel = new UIBarButtonItem(UIBarButtonSystemItem.Cancel);

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		
			this.NavigationItem.RightBarButtonItem = buttonSave;
			this.NavigationItem.LeftBarButtonItem = buttonCancel;
		
			this.buttonSave.Clicked += (sender, e) => 
			{
				this.ViewModel.Save();
			};

			this.buttonCancel.Clicked += (sender, e) => 
			{
				this.ViewModel.Cancel();
			};
		
			this.Root = new RootElement ("New Wish List")
			{
				new Section()
				{
					new EntryElement("Name", "Wish List Name").Bind(this, "{'Value':{'Path':'Name'}}"),
					new EntryElement("Description", "Description").Bind(this, "{'Value':{'Path':'Description'}}")
				}
			};
		}

		LoadingHUDView loadingView;

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.ViewModel.PropertyChanged += HandlePropertyChanged;
		}

		public override void ViewDidDisappear (bool animated)
		{
			this.ViewModel.PropertyChanged -= HandlePropertyChanged;
			base.ViewDidDisappear (animated);
		}

		void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals ("IsLoading")) 
			{
				if (this.ViewModel.IsLoading && loadingView == null)
				{
					this.InvokeOnMainThread(() => {
						loadingView = new LoadingHUDView("Loading...", "");
						this.View.AddSubview(loadingView);
						loadingView.StartAnimating();
					});
				}
				else if (!this.ViewModel.IsLoading && this.loadingView != null)
				{
					this.InvokeOnMainThread(() => {
						loadingView.StopAnimating();
						loadingView.RemoveFromSuperview();
						loadingView = null;
					});
				}
			}
		}
	}
}

