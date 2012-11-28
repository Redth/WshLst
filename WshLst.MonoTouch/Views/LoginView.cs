
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
	public class LoginView : MvxBindingTouchTableViewController<LoginViewModel>
	{
		public LoginView (MvxShowViewModelRequest request) : base (request)
		{
		}
				
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			
			this.Title = "Wsh Lst Login";

			var source = new MvxBindableTableViewSource (
				this.TableView,
				UITableViewCellStyle.Default,
				new NSString ("LoginView"),
				"{'TitleText':{'Path':'Name'}}",
				UITableViewCellAccessory.DisclosureIndicator);


			this.AddBindings (new Dictionary<object, string> () { { source, "{'ItemsSource':{'Path':'Platforms'}}" } });

			source.SelectionChanged += (sender, e) => 
			{
				var path = this.TableView.IndexPathForSelectedRow;
				var platform = this.ViewModel.Platforms[path.Row];

				this.InvokeOnMainThread(() => this.TableView.DeselectRow(path, true));

				this.ViewModel.Login(platform);
			};

            this.NavigationController.ToolbarHidden = false;

			this.TableView.Source = source;
			this.TableView.ReloadData ();

			this.ViewModel.ViewController = this;

			this.ViewModel.CheckLogin ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

            if (WshLst.Core.App.IsLaunch)
            {
                WshLst.Core.App.IsLaunch = false;

                this.ViewModel.CheckLogin();
            }
		}
	}
}

