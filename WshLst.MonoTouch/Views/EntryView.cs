
using System;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.Views;
using Cirrious.MvvmCross.Touch.Interfaces;
using CrossUI.Touch.Dialog.Elements;

using Xamarin.Media;

using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class EntryView : MvxTouchDialogViewController<EntryViewModel>
	{
		public EntryView (MvxShowViewModelRequest request) : base (request, UITableViewStyle.Grouped, new RootElement(), true)
		{
		}

		UIBarButtonItem buttonEdit;
        UIBarButtonItem buttonDelete;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			buttonEdit = new UIBarButtonItem("Edit", UIBarButtonItemStyle.Bordered, (s, e) => {
				this.ViewModel.Edit();
			});

            buttonDelete = new UIBarButtonItem("Delete", UIBarButtonItemStyle.Bordered, (s, e) => {
                var av = new UIAlertView("Delete?", "Are you sure you want to delete this item?", null, "No", "Yes");
                av.Clicked += (s2, e2) => {
                    if (e2.ButtonIndex == 1)
                        this.ViewModel.Delete();
                };
            });

            this.SetToolbarItems(new UIBarButtonItem[]
            {
                buttonDelete,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                buttonEdit
            }, false);
		
			this.Root = new RootElement ("View Item")
			{
				new Section("What is it called?")
				{
					new StyledStringElement("Name:", "").Bind(this, "{'Value':{'Path':'Entry.Name'}}"),
				},
				new Section("Where to buy")
				{
					new StyledStringElement("Store:", "").Bind(this, "{'Value':{'Path':'Entry.Store'}}"),
				},
				new Section("How much is it?")
				{
					new StyledStringElement("Price: $", "99.99").Bind(this, "{'Value':{'Path':'Entry.Price'}}")
				},
				new Section("Additional Notes")
				{
					new MultilineElement("Notes").Bind(this, "{'Value':{'Path':'Entry.Notes'}}"),
				},
				new Section("Photo") { }
			};
		}

		LoadingHUDView loadingView;

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.ViewModel.PropertyChanged += HandlePropertyChanged;

			this.ViewModel.LoadEntry ();
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
						this.View.AddSubview (loadingView);
						loadingView.StartAnimating ();
					});
				} else if (!this.ViewModel.IsLoading && this.loadingView != null) {
					this.InvokeOnMainThread (() => {
						loadingView.StopAnimating ();
						loadingView.RemoveFromSuperview ();
						loadingView = null;
					});
				}
			} else if (e.PropertyName.Equals ("HasImage")) {
				if (this.ViewModel.HasImage && this.ViewModel.EntryImage != null && !string.IsNullOrEmpty(this.ViewModel.EntryImage.ImageBase64))
				{
					var converter = new Base64ToUIImageConverter();

					var img = (UIImage)converter.Convert(this.ViewModel.EntryImage.ImageBase64, typeof(UIImage), null, null);

					this.Root[4].Clear();
					this.Root[4].Add(new LargeImageElement() { Image = img });
				}
				else
				{
					this.Root[4].Clear();
				}
			} else {
				this.TableView.ReloadData();
			}
		}
	}
}

