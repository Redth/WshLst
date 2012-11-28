using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.ExtensionMethods;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.Touch.Interfaces;
using Cirrious.MvvmCross.Views;
using CrossUI.Touch.Dialog;
using CrossUI.Touch.Dialog.Elements;
using MonoTouch.Foundation;
using MonoTouch.MessageUI;
using MonoTouch.UIKit;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class ShareView : BaseDialogViewController<ShareViewModel>
	{
		public ShareView(MvxShowViewModelRequest request) : base (request, UITableViewStyle.Plain, new RootElement(), true)
		{
		}

		UIBarButtonItem buttonDone = new UIBarButtonItem(UIBarButtonSystemItem.Done);
		UIBarButtonItem buttonCancel;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			buttonCancel = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered, (s, e) => ViewModel.Cancel());

			Title = "Share Wish List";
			NavigationItem.LeftBarButtonItem = buttonCancel;
			NavigationItem.RightBarButtonItem = buttonDone;           

			buttonDone.Clicked += (s, e) => 
			{
				//Send email
				var to = ViewModel.GetEmailTo();
				var subject = ViewModel.GetEmailSubject();
				var body = ViewModel.GetEmailBody();

				var mailComposer = new MFMailComposeViewController();

				if (!string.IsNullOrEmpty(to))
					mailComposer.SetToRecipients(to.Split(';'));

				mailComposer.SetSubject(subject);
				mailComposer.SetMessageBody(body, false);

				mailComposer.Finished += (s2, e2) => 
					{
						InvokeOnMainThread(() => 
							{
								DismissViewController(true, () => 
									{
										InvokeOnMainThread(() => 
					                		NavigationController.PopViewControllerAnimated(true));
									});
							});
					};

				InvokeOnMainThread(() => NavigationController.PresentViewController(mailComposer, true, null));
			};
			           
			Root = new RootElement("Share Wish List") { new Section("Choose Contacts to Share with") };
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			this.ViewModel.LoadContacts();
		}

		void LoadTable()
		{
			Root.Clear();

			//Group contacts by name
			var gcs = from c in this.ViewModel.Contacts
                      group c by (c.DisplayName ?? "z").ToUpper().Substring(0, 1) into grouping
                      select grouping;

			//Populate the table with sections for each group and all items in each group
			foreach (var grp in gcs.OrderBy(g => g.Key))
			{
				var s = new Section(grp.Key);

				foreach (var c in grp)
					s.Add(new SelectableContactElement(c));

				Root.Add(s);
			}
		}

		public override void HandlePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("Contacts"))
			{
				this.BeginInvokeOnMainThread(() => 
				{
					LoadTable();
					TableView.ReloadData();
				});
			}
		}
	}
}

