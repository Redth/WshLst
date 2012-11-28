using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.ExtensionMethods;
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
	public class EditItemView : BaseDialogViewController<EditEntryViewModel>
	{
		public EditItemView(MvxShowViewModelRequest request) 
			: base (request, UITableViewStyle.Grouped, new RootElement(), true)
		{
		}
		
		UIBarButtonItem _buttonSave = new UIBarButtonItem(UIBarButtonSystemItem.Done);
		UIBarButtonItem _buttonCancel = new UIBarButtonItem(UIBarButtonSystemItem.Cancel);
		StyledStringElement _elemPickPhoto;
		StyledStringElement _elemTakePhoto;
		StyledStringElement _elemRemPhoto;
		LargeImageElement _elemImage;
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			NavigationItem.RightBarButtonItem = _buttonSave;
			NavigationItem.LeftBarButtonItem = _buttonCancel;
			
			_buttonSave.Clicked += (sender, e) => ViewModel.Save();
			_buttonCancel.Clicked += (sender, e) => ViewModel.Cancel();
					
			_elemPickPhoto = new StyledStringElement("Pick Existing Photo", () => addPhoto(false));
			_elemTakePhoto = new StyledStringElement("Take New Photo", () => addPhoto(true));
			_elemRemPhoto = new StyledStringElement("Remove Photo", () => ViewModel.RemovePhoto());

			_elemImage = new LargeImageElement();

			this.Root = new RootElement(string.IsNullOrEmpty(ViewModel.EntryId) ? "New Item" : "Edit Item")
			{
				new Section()
				{
					new StyledStringElement("Scan Barcode", () => { this.ViewModel.Scan(); })
				},
				new Section("What is it called?")
				{
					new EntryElement("Name:", "Name of product / item").Bind(this, "{'Value':{'Path':'Entry.Name'}}"),
				},
				new Section("Where to buy")
				{
					new EntryElement("Store:", "Store / place").Bind(this, "{'Value':{'Path':'Entry.Store'}}"),
					new CheckboxElement("include current location").Bind(this, "{'Value':{'Path':'UseLocation'}}"),
				},
				new Section("How much is it?")
				{
					new EntryElement("Price: $", "99.99") {	KeyboardType = UIKeyboardType.DecimalPad }
						.Bind(this, "{'Value':{'Path':'Entry.Price'}}")
				},
				new Section("Additional Notes")
				{
					new EntryElement("Notes", "Notes...").Bind(this, "{'Value':{'Path':'Entry.Notes'}}")
				},
				new Section("Photo") { _elemTakePhoto, _elemPickPhoto, _elemImage }
			};

			Root.UnevenRows = true;
			Root [5].Remove(_elemImage);
		}

		void addPhoto(bool takeNew)
		{
			var mediaFileSource = this.GetService<WshLst.Core.Interfaces.IMediaFileSource>();

			InvokeOnMainThread(() => 
			{
				mediaFileSource.GetPhoto(takeNew).ContinueWith((t) => 
				{
					var ex = t.Exception;
					if (t.Status != TaskStatus.RanToCompletion || t.Result == null)
						return;
						
					using (var mediaFile = t.Result)
					{
						ViewModel.AddPhoto(mediaFile.GetStream());
					}
				});
			});
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
				if (this.ViewModel.HasImage)
				{			
					var converter = new Base64ToUIImageConverter();
					var img = (UIImage)converter.Convert(this.ViewModel.EntryImage.ImageBase64, typeof(UIImage), null, null);
					
					Root [5].Clear();
					Root [5].Add(_elemRemPhoto);
					Root [5].Add(new LargeImageElement() { Image = img });
					TableView.ReloadData();
				} 
				else
				{
					Root [5].Clear();
					Root [5].Add(_elemTakePhoto);
					Root [5].Add(_elemPickPhoto);
					TableView.ReloadData();
				}
			}
		}
	}
}

