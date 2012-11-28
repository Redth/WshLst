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
	public class WishListsView : BaseBindingTableViewController<WishListsViewModel>
	{
		public WishListsView(MvxShowViewModelRequest request) : base (request)
		{
		}

		UIBarButtonItem buttonAdd = new UIBarButtonItem(UIBarButtonSystemItem.Add);
		UIBarButtonItem buttonLogout;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			buttonLogout = new UIBarButtonItem("Logout", UIBarButtonItemStyle.Bordered, (s, e) => ViewModel.Logout());
			buttonAdd.Clicked += (sender, e) => ViewModel.Add();

			Title = "Wish Lists";
			NavigationItem.LeftBarButtonItem = buttonLogout;
			
			this.SetToolbarItems(new UIBarButtonItem[]
                {
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                    buttonAdd
                }, false);

			var source = new MvxDeleteBindableTableViewSource(TableView, UITableViewCellStyle.Subtitle, new NSString("LoginView"),
				"{'TitleText':{'Path':'Name'}, 'DetailText':{'Path':'Description'}}", UITableViewCellAccessory.DisclosureIndicator);

			this.AddBindings(new Dictionary<object, string>() { { source, "{'ItemsSource':{'Path':'Lists'}}" } });

			source.SelectionChanged += (sender, e) => 
				{
					var path = TableView.IndexPathForSelectedRow;
					var list = ViewModel.Lists [path.Row];

					InvokeOnMainThread(() => TableView.DeselectRow(path, true));

					ViewModel.Select(list);
				};

			source.OnShouldCommitEditingStyle += (UITableView tv, UITableViewCellEditingStyle style, NSIndexPath indexPath) => 
				{
					var list = ViewModel.Lists [indexPath.Row];

					ViewModel.Delete(list);
				};

			TableView.Source = source;
			TableView.ReloadData();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		
			ViewModel.LoadLists();
		}

		public override void HandlePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("Lists")) 
				TableView.ReloadData();
		}
	}
}

