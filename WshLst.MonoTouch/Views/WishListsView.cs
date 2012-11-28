
using System;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Cirrious.MvvmCross.Binding.Touch.ExtensionMethods;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Views;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class WishListsView : MvxBindingTouchTableViewController<WishListsViewModel>
	{
		public WishListsView (MvxShowViewModelRequest request) : base (request)
		{
		}

		UIBarButtonItem buttonAdd = new UIBarButtonItem(UIBarButtonSystemItem.Add);
		UIBarButtonItem buttonLogout;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			buttonLogout = new UIBarButtonItem ("Logout", UIBarButtonItemStyle.Bordered, (s, e) => {
				this.ViewModel.Logout();
			});

			this.Title = "Wish Lists";

            this.NavigationItem.LeftBarButtonItem = buttonLogout;
			
            this.SetToolbarItems(new UIBarButtonItem[]
                {
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                    buttonAdd
                }, false);

			var source = new MvxDeleteBindableTableViewSource (
				this.TableView,
				UITableViewCellStyle.Subtitle,
				new NSString ("LoginView"),
                "{'TitleText':{'Path':'Name'}, 'DetailText':{'Path':'Description'}}",
				UITableViewCellAccessory.DisclosureIndicator);


			this.AddBindings (new Dictionary<object, string> () { { source, "{'ItemsSource':{'Path':'Lists'}}" } });

			source.SelectionChanged += (sender, e) => 
			{
				var path = this.TableView.IndexPathForSelectedRow;
				var list = this.ViewModel.Lists[path.Row];

				this.InvokeOnMainThread(() => this.TableView.DeselectRow(path, true));

				this.ViewModel.Select(list);
			};

			source.OnShouldCommitEditingStyle += (UITableView tv, UITableViewCellEditingStyle style, NSIndexPath indexPath) => {

				var list = this.ViewModel.Lists[indexPath.Row];

				this.ViewModel.Delete(list);
			};


			this.buttonAdd.Clicked += (sender, e) => 
			{
				this.ViewModel.Add();
			};

			this.TableView.Source = source;
			this.TableView.ReloadData ();



		}

		LoadingHUDView loadingView;

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.ViewModel.PropertyChanged += HandlePropertyChanged;

			this.ViewModel.LoadLists ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			this.ViewModel.PropertyChanged -= HandlePropertyChanged;
			base.ViewDidDisappear (animated);
		}

		void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals ("IsLoading")) {
				if (this.ViewModel.IsLoading && loadingView == null) {
					this.InvokeOnMainThread (() => {
						loadingView = new LoadingHUDView ("Loading...", "");
						this.TableView.AddSubview (loadingView);
						loadingView.StartAnimating ();
					});
				} else if (!this.ViewModel.IsLoading && this.loadingView != null) {
					this.InvokeOnMainThread (() => {
						loadingView.StopAnimating ();
						loadingView.RemoveFromSuperview ();
						loadingView = null;
					});
				}
			} else if (e.PropertyName.Equals ("Lists")) {

				this.BeginInvokeOnMainThread(() => this.TableView.ReloadData());
			}
		}
	}
}

