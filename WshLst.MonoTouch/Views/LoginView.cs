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
	public class LoginView : BaseBindingTableViewController<LoginViewModel>
	{
		public LoginView(MvxShowViewModelRequest request) : base (request)
		{
		}
				
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
						
			NavigationItem.SetHidesBackButton(true, false);
			Title = "Wsh Lst Login";

			var source = new MvxBindableTableViewSource(TableView, UITableViewCellStyle.Default, new NSString("LoginView"),
				"{'TitleText':{'Path':'Name'}}", UITableViewCellAccessory.DisclosureIndicator);

			this.AddBindings(new Dictionary<object, string>() { { source, "{'ItemsSource':{'Path':'Platforms'}}" } });

			source.SelectionChanged += (sender, e) => 
				{
					var path = TableView.IndexPathForSelectedRow;
					var platform = ViewModel.Platforms [path.Row];

					InvokeOnMainThread(() => TableView.DeselectRow(path, true));

					ViewModel.Login(platform);
				};

			NavigationController.ToolbarHidden = false;

			TableView.Source = source;
			TableView.ReloadData();

			ViewModel.ViewController = this;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (WshLst.Core.App.IsLaunch)
			{
				WshLst.Core.App.IsLaunch = false;

				ViewModel.CheckLogin();
			}
		}
	}
}

