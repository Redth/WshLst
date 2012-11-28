using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ExtensionMethods;
using Microsoft.WindowsAzure.MobileServices;
using WshLst.Core.Interfaces;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class EditEntryViewModel : BaseViewModel
	{
		public string EntryId = string.Empty;
		public string ListId = string.Empty;
		private Entry _entry;
		private EntryImage _entryImage;

		private WishList _wishList;

		public EditEntryViewModel(string listId, string entryId)
		{
			UseLocation = true;

			ListId = listId;
			EntryId = entryId;

			if (_entry == null)
				_entry = new Entry();

			if (_entryImage == null)
				_entryImage = new EntryImage();

			if (string.IsNullOrEmpty(entryId))
			{
				//Load the nearest place name, if possible
				var geo = this.GetService<IGeolocator>();

				if (geo.CurrentPosition != null)
				{
					geo.LookupNearbyPlaces(nearestPlaceName =>
						{
							if (!string.IsNullOrEmpty(nearestPlaceName) && string.IsNullOrEmpty(_entry.Store))
							{
								_entry.Store = nearestPlaceName;
								RaisePropertyChanged(() => Entry);
							}
						});
				}
			}
		}

		public WishList WishList
		{
			get { return _wishList; }
			set
			{
				_wishList = value;
				RaisePropertyChanged(() => WishList);
			}
		}

		public Entry Entry
		{
			get { return _entry; }
			set
			{
				_entry = value;
				RaisePropertyChanged(() => Entry);
			}
		}

		public EntryImage EntryImage
		{
			get { return _entryImage; }
			set
			{
				_entryImage = value;
				RaisePropertyChanged(() => EntryImage);
			}
		}

		public bool HasImage
		{
			get { return !string.IsNullOrEmpty(_entryImage.ImageBase64); }
		}

		public bool UseLocation { get; set; }

		public void AddPhoto(string base64Image)
		{
			_entryImage.ImageBase64 = base64Image;
			RaisePropertyChanged(() => EntryImage);
			RaisePropertyChanged(() => HasImage);
		}

		public void RemovePhoto()
		{
			_entryImage.ImageBase64 = string.Empty;
			RaisePropertyChanged(() => EntryImage);
			RaisePropertyChanged(() => HasImage);
		}

		public ICommand ScanCommand
		{
			get { return new MvxRelayCommand(Scan); }
		}

		public void Scan()
		{
			var scanner = this.GetService<IBarcodeScanner>();
			scanner.Scan().ContinueWith(t =>
				{
					var ex = t.Exception;

					if (t.Status == TaskStatus.RanToCompletion)
					{
						if (t.Result != null && !string.IsNullOrEmpty(t.Result.Text))
						{
							_entry.Upc = t.Result.Text;
							RaisePropertyChanged(() => Entry);

							IsLoading = true;

							scanner.LookupProduct(t.Result.Text, product =>
								{
									if (product != null && !string.IsNullOrEmpty(product.Name) && string.IsNullOrEmpty(_entry.Name))
									{ 
										_entry.Name = product.Name;
										RaisePropertyChanged(() => Entry);
									}

									IsLoading = false;
								});
						}
					}
				});
		}

		public ICommand SaveCommand
		{
			get { return new MvxRelayCommand(Save); }
		}
	
		public void Save()
		{
			IsLoading = true;

			var geo = this.GetService<IGeolocator>();

			double lat = 0.0;
			double lon = 0.0;

			if (UseLocation && geo.CurrentPosition != null)
			{
				lat = geo.CurrentPosition.Latitude;
				lon = geo.CurrentPosition.Longitude;
			}

			Entry.Latitude = lat;
			Entry.Longitude = lon;

			Entry.ListId = int.Parse(ListId);

			if (_entryImage != null && !string.IsNullOrEmpty(_entryImage.ImageBase64))
			{
				Entry.ImageGuid = _entryImage.ImageGuid;

				IMobileServiceTable<EntryImage> entryImageTable = App.Azure.GetTable<EntryImage>();

				//Try and get an existing image for this item
				entryImageTable.Where(ei => ei.ImageGuid == _entryImage.ImageGuid).ToListAsync().ContinueWith(t =>
					{
						var ex = t.Exception;

						if (t.Status == TaskStatus.RanToCompletion && t.Result != null && t.Result.Count > 0)
						{
							var existingImage = t.Result[0];

							//Update the image since it already exists
							_entryImage.Id = existingImage.Id;

							entryImageTable.UpdateAsync(_entryImage).ContinueWith(t2 => { var ex2 = t2.Exception; });
						}
						else
						{
							//Insert the image since it doesn't exist already
							entryImageTable.InsertAsync(_entryImage).ContinueWith(t2 => { var ex2 = t2.Exception; });
						}
					});
			}
			else
				_entry.ImageGuid = string.Empty;

			if (Entry.ListId <= 0)
			{
				ReportError("Invalid List!");
				return;
			}

			var continuation = new Action<Task>(t =>
				{
					if (t.Exception != null)
						Debug.WriteLine(t.Exception.Flatten());

					if (t.Status != TaskStatus.RanToCompletion)
					{
						IsLoading = false;
						ReportError("Unable to save your new Wishlist Item!");
					}
					else
						RequestClose(this);
				});

			if (Entry.Id <= 0)
				App.Azure.GetTable<Entry>().InsertAsync(Entry).ContinueWith(continuation);
			else
				App.Azure.GetTable<Entry>().UpdateAsync(Entry).ContinueWith(continuation);
		}

		public ICommand CancelCommand
		{
			get { return new MvxRelayCommand(Cancel); }
		}

		public void Cancel()
		{
			RequestClose(this);
		}

		public ICommand DeleteCommand
		{
			get { return new MvxRelayCommand(Delete); }
		}

		public void Delete()
		{
			IsLoading = true;

			App.Azure.GetTable<Entry>().DeleteAsync(_entry).ContinueWith(t =>
				{
					var ex = t.Exception;

					IsLoading = false;

					if (t.Status == TaskStatus.RanToCompletion)
					{
						RequestClose(this);
					}
				});
		}

		public void LoadEntry()
		{
			if (string.IsNullOrEmpty(EntryId))
				return;

			IsLoading = true;

			App.Azure.GetTable<Entry>().LookupAsync(EntryId).ContinueWith(t =>
				{
					var ex = t.Exception;

					if (t.Status == TaskStatus.RanToCompletion)
					{
						_entry = t.Result;
						RaisePropertyChanged("Entry");

						App.Azure.GetTable<EntryImage>().Where(ei => ei.ImageGuid == _entry.ImageGuid).ToListAsync().ContinueWith(t2 =>
							{
								var ex2 = t2.Exception;

								if (t2.Status == TaskStatus.RanToCompletion && t2.Result.Count > 0)
								{
									_entryImage = t2.Result[0];
									RaisePropertyChanged("EntryImage");
									RaisePropertyChanged("HasImage");
								}
							});
					}

					IsLoading = false;
				});

			if (_entry == null)
				_entry = new Entry();

			if (_entryImage == null)
				_entryImage = new EntryImage();
		}
	}
}