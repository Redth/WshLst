using System;
using System.Collections.Generic;
using System.Drawing;
using Cirrious.MvvmCross.Dialog.Touch;
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
	public class EntryView : BaseDialogViewController<EntryViewModel>
	{
		public EntryView(MvxShowViewModelRequest request) 
			: base (request, UITableViewStyle.Grouped, new RootElement(), true)
		{
		}

		UIBarButtonItem buttonEdit;
		UIBarButtonItem buttonDelete;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			buttonEdit = new UIBarButtonItem("Edit", UIBarButtonItemStyle.Bordered, (s, e) => ViewModel.Edit());
			buttonDelete = new UIBarButtonItem("Delete", UIBarButtonItemStyle.Bordered, (s, e) => 
				{
					var av = new UIAlertView("Delete?", "Are you sure you want to delete this item?", null, "No", "Yes");
					av.Clicked += (s2, e2) => 
						{
							if (e2.ButtonIndex == 1)
								ViewModel.Delete();
						};
				});

			this.SetToolbarItems(new UIBarButtonItem[]
	            {
	                buttonDelete,
	                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
	                buttonEdit
	            }, false);
		
			this.Root = new RootElement("View Item")
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
	
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
	
			ViewModel.LoadEntry();
		}

		public override void HandlePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("HasImage"))
			{
				if (ViewModel.HasImage && ViewModel.EntryImage != null && !string.IsNullOrEmpty(ViewModel.EntryImage.ImageBase64))
				{
					var converter = new Base64ToUIImageConverter();
					var img = (UIImage)converter.Convert(this.ViewModel.EntryImage.ImageBase64, typeof(UIImage), null, null);

					Root [4].Clear();
					Root [4].Add(new LargeImageElement() { Image = img });
				} 
				else
					Root [4].Clear();
			} 
			else
				TableView.ReloadData();
		}
	}
}

