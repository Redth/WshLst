
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Views;
using Cirrious.MvvmCross.Touch.Interfaces;
using CrossUI.Touch.Dialog.Elements;

using Xamarin.Media;

using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class EditItemView : MvxTouchDialogViewController<EditEntryViewModel>
	{
		public EditItemView (MvxShowViewModelRequest request) : base (request, UITableViewStyle.Grouped, new RootElement(), true)
		{
		}
		
		UIBarButtonItem buttonSave = new UIBarButtonItem(UIBarButtonSystemItem.Done);
		UIBarButtonItem buttonCancel = new UIBarButtonItem(UIBarButtonSystemItem.Cancel);

		StyledStringElement elemPickPhoto;
		StyledStringElement elemTakePhoto;
		StyledStringElement elemRemPhoto;
		LargeImageElement elemImage;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.NavigationItem.RightBarButtonItem = buttonSave;
			this.NavigationItem.LeftBarButtonItem = buttonCancel;
			
			this.buttonSave.Clicked += (sender, e) => 
			{
				this.ViewModel.Save();
			};

			this.buttonCancel.Clicked += (sender, e) => 
			{
				this.ViewModel.Cancel();
			};

		
			elemPickPhoto = new StyledStringElement ("Pick Existing Photo", () => 
			{
				addPhoto(false);
			});

			elemTakePhoto = new StyledStringElement("Take New Photo", () => 
			{
				addPhoto(true);
			});
            
			elemRemPhoto = new StyledStringElement ("Remove Photo", () => {
				this.ViewModel.RemovePhoto();
			});

			elemImage = new LargeImageElement();

			this.Root = new RootElement ("New Item")
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
					new CheckboxElement("include current location"),
				},
				new Section("How much is it?")
				{
					new EntryElement("Price: $", "99.99") {
						KeyboardType = UIKeyboardType.DecimalPad
					}.Bind(this, "{'Value':{'Path':'Entry.Price'}}")
				},
				new Section("Additional Notes")
				{
					new EntryElement("Notes", "Notes..."){

					}.Bind(this, "{'Value':{'Path':'Entry.Notes'}}")
				},
				new Section("Photo")
				{
					elemTakePhoto,
					elemPickPhoto,
					elemImage
				}
			};
            this.Root.UnevenRows = true;

			this.Root [5].Remove(elemImage);
		}

		void addPhoto(bool takeNew)
		{
			var mediaFileSource = this.GetService<WshLst.Core.Interfaces.IMediaFileSource>();

			this.InvokeOnMainThread(() => 
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


		LoadingHUDView loadingView;

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			this.ViewModel.PropertyChanged += HandlePropertyChanged;

			this.ViewModel.LoadEntry ();
		}

		public override void ViewDidUnload()
		{
			this.ViewModel.PropertyChanged -= HandlePropertyChanged;
			base.ViewDidUnload();
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
			} 
			else if (e.PropertyName.Equals ("HasImage")) 
			{
				if (this.ViewModel.HasImage) 
				{			
					this.BeginInvokeOnMainThread (() =>               
					{
						var converter = new Base64ToUIImageConverter ();
						var img = (UIImage)converter.Convert (this.ViewModel.EntryImage.ImageBase64, typeof(UIImage), null, null);
									
						this.Root [5].Clear ();
						this.Root [5].Add (elemRemPhoto);
						this.Root [5].Add (new LargeImageElement () { Image = img });
						this.TableView.ReloadData();
					});
				} 
				else 
				{
					this.BeginInvokeOnMainThread (() => 
					{
						this.Root [5].Clear ();
						this.Root [5].Add (elemTakePhoto);
						this.Root [5].Add (elemPickPhoto);
						this.TableView.ReloadData();
					});
				}
			}
		}
	}
}

