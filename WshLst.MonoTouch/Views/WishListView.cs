
using System;
using System.Collections.Generic;
using System.Drawing;
using Cirrious.MvvmCross.Binding.Touch.ExtensionMethods;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class WishListView : BaseBindingTableViewController<WishListViewModel>
	{
		public WishListView(MvxShowViewModelRequest request) : base (request)
		{
		}

		UIBarButtonItem buttonEdit = new UIBarButtonItem(UIBarButtonSystemItem.Edit);
		UIBarButtonItem buttonAdd = new UIBarButtonItem(UIBarButtonSystemItem.Add);
		UIBarButtonItem buttonShare;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Loading...";
            			
			var source = new MvxDeleteBindableTableViewSource(this.TableView, UITableViewCellStyle.Subtitle,
				new NSString("ListView"), "{'TitleText':{'Path':'Name'}, 'DetailText':{'Path':'Notes'}}",
				UITableViewCellAccessory.DisclosureIndicator);

			this.AddBindings(new Dictionary<object, string>() { { source, "{'ItemsSource':{'Path':'Entries'}}" } });

			source.SelectionChanged += (sender, e) => 
			{
				var path = TableView.IndexPathForSelectedRow;
				var entry = ViewModel.Entries [path.Row];

				this.InvokeOnMainThread(() => TableView.DeselectRow(path, true));

				this.ViewModel.Select(entry);
			};

			source.OnShouldCommitEditingStyle += (UITableView tv, UITableViewCellEditingStyle style, NSIndexPath indexPath) => 
			{
				var entry = ViewModel.Entries [indexPath.Row];

				ViewModel.Delete(entry);
			};

			buttonEdit.Clicked += (sender, e) => ViewModel.EditWishList();
			buttonAdd.Clicked += (sender, e) => ViewModel.Add();
			buttonShare = new UIBarButtonItem("Share", UIBarButtonItemStyle.Bordered, (s, e) => ViewModel.Share());

			NavigationItem.RightBarButtonItem = buttonEdit;

			SetToolbarItems(new UIBarButtonItem[]
	            {
	                buttonShare,
	                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
	                buttonAdd
	            }, false);

			TableView.Source = source;
			TableView.ReloadData();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		   
			ViewModel.LoadListAndItems();
		}

		public override void HandlePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("WishList"))
				Title = ViewModel.WishList.Name;
			else if (e.PropertyName.Equals("Entries"))
				TableView.ReloadData();
		}
	}
}

