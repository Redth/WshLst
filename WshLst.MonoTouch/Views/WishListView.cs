
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
	public class WishListView : MvxBindingTouchTableViewController<WishListViewModel>
	{
		public WishListView (MvxShowViewModelRequest request) : base (request)
		{
		}

		UIBarButtonItem buttonAdd = new UIBarButtonItem(UIBarButtonSystemItem.Add);
        UIBarButtonItem buttonShare;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.Title = "Loading...";
            			
			var source = new MvxDeleteBindableTableViewSource (
				this.TableView,
				UITableViewCellStyle.Subtitle,
				new NSString ("ListView"),
                "{'TitleText':{'Path':'Name'}, 'DetailText':{'Path':'Notes'}}",
				UITableViewCellAccessory.DisclosureIndicator);


			this.AddBindings (new Dictionary<object, string> () { { source, "{'ItemsSource':{'Path':'Entries'}}" } });

			source.SelectionChanged += (sender, e) => 
			{
				var path = this.TableView.IndexPathForSelectedRow;
				var entry = this.ViewModel.Entries[path.Row];

				this.InvokeOnMainThread(() => this.TableView.DeselectRow(path, true));

				this.ViewModel.Select(entry);
			};

			source.OnShouldCommitEditingStyle += (UITableView tv, UITableViewCellEditingStyle style, NSIndexPath indexPath) => {

				var entry = this.ViewModel.Entries[indexPath.Row];

				this.ViewModel.Delete(entry);
			};

			this.buttonAdd.Clicked += (sender, e) => 
			{
				this.ViewModel.Add();
			};

            this.buttonShare = new UIBarButtonItem("Share", UIBarButtonItemStyle.Bordered, (s, e) => {
                this.ViewModel.Share();
            });

            this.SetToolbarItems(new UIBarButtonItem[]
            {
                buttonShare,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                buttonAdd
            }, false);

			this.TableView.Source = source;
			this.TableView.ReloadData ();
		}

		LoadingHUDView loadingView;

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.ViewModel.PropertyChanged += HandlePropertyChanged;
           
			this.ViewModel.LoadListAndItems ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			this.ViewModel.PropertyChanged -= HandlePropertyChanged;

			base.ViewDidDisappear (animated);
		}

		void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsLoading"))
			{
				if (this.ViewModel.IsLoading && loadingView == null)
				{
					this.InvokeOnMainThread(() => {
						loadingView = new LoadingHUDView("Loading...", "");
						this.TableView.AddSubview(loadingView);
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
			else if (e.PropertyName.Equals("WishList"))
			{
				this.InvokeOnMainThread(() => this.Title = this.ViewModel.WishList.Name);
			}
			else if (e.PropertyName.Equals("Entries"))
			{
				this.BeginInvokeOnMainThread(() => this.TableView.ReloadData());
			}
		}
	}
}

