using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.ExtensionMethods;

namespace WshLst.Core.ViewModels
{
	public class EditEntryViewModel : BaseViewModel
	{
		public EditEntryViewModel(string listId, string entryId, string entryImageId)
		{
			this.UseLocation = true;

			this.ListId = listId;
			this.EntryId = entryId;

			if (this.entry == null)
				this.entry = new Models.Entry();

			if (this.entryImage == null)
				this.entryImage = new Models.EntryImage();

            if (string.IsNullOrEmpty(entryId))
            {
                //Load the nearest place name, if possible
                var geo = this.GetService<Interfaces.IGeolocator>();

                if (geo.CurrentPosition != null)
                {
                    geo.LookupNearbyPlaces((nearestPlaceName) =>
                    {
                        if (!string.IsNullOrEmpty(nearestPlaceName) && string.IsNullOrEmpty(this.entry.Store))
                        {
                            this.entry.Store = nearestPlaceName;
                            RaisePropertyChanged("Entry");
                        }
                    });
                }
            }
		}

		public string ListId = string.Empty;
		public string EntryId = string.Empty;	

		Models.WishList wishList;
		public Models.WishList WishList
		{
			get { return wishList; }
			set { wishList = value; RaisePropertyChanged("WishList"); }
		}

		Models.Entry entry;
		public Models.Entry Entry
		{
			get { return entry; }
			set { entry = value; RaisePropertyChanged("Entry"); }
		}

		Models.EntryImage entryImage;
		public Models.EntryImage EntryImage
		{
			get { return entryImage; }
			set { entryImage = value; RaisePropertyChanged("EntryImage"); }
		}
		
		public bool HasImage
		{
			get { return !string.IsNullOrEmpty(entryImage.ImageBase64); }
		}
		
		public bool UseLocation { get; set; }

		public void AddPhoto(string base64Image)
		{
            this.entryImage.ImageBase64 = base64Image;
			RaisePropertyChanged("EntryImage");
			RaisePropertyChanged("HasImage");
		}

		public void RemovePhoto()
		{
			this.entryImage.ImageBase64 = string.Empty;
			RaisePropertyChanged("EntryImage");
			RaisePropertyChanged("HasImage");
		}

		public void Scan()
		{
			var scanner = this.GetService<Interfaces.IBarcodeScanner>();
			scanner.Scan().ContinueWith((t) =>
			{
				var ex = t.Exception;

				var extxt = ex.Flatten().ToString();

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					if (t.Result != null && !string.IsNullOrEmpty(t.Result.Text))
					{
						this.entry.Upc = t.Result.Text;
						RaisePropertyChanged("Entry");

						this.IsLoading = true;

						
						scanner.LookupProduct(t.Result.Text, (product) =>
						{
							if (product != null && !string.IsNullOrEmpty(product.Name) && string.IsNullOrEmpty(this.Entry.Name))
							{
								this.entry.Name = product.Name;
								RaisePropertyChanged("Entry");
							}

							this.IsLoading = false;
						});
					}
				}
			});
		}

		public void Save()
		{
			this.IsLoading = true;

			var geo = this.GetService<Interfaces.IGeolocator>();

			double lat = 0.0;
			double lon = 0.0;

			if (this.UseLocation && geo.CurrentPosition != null)
			{
				lat = geo.CurrentPosition.Latitude;
				lon = geo.CurrentPosition.Longitude;
			}

			this.Entry.Latitude = lat;
			this.Entry.Longitude = lon;
			
			this.Entry.ListId = int.Parse(this.ListId);

			if (this.entryImage != null && !string.IsNullOrEmpty(this.entryImage.ImageBase64))
			{
				this.Entry.ImageGuid = this.entryImage.ImageGuid;

				var entryImageTable = App.Azure.GetTable<Models.EntryImage>();

				//Try and get an existing image for this item
				entryImageTable.Where(ei => ei.ImageGuid == this.entryImage.ImageGuid).ToListAsync().ContinueWith((t) =>
				{
					var ex = t.Exception;

					if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion && t.Result != null && t.Result.Count > 0)
					{
						var existingImage = t.Result[0];

						//Update the image since it already exists
						this.entryImage.Id = existingImage.Id;

						entryImageTable.UpdateAsync(this.entryImage).ContinueWith((t2) =>
						{
							var ex2 = t2.Exception;
						});
					}
					else
					{
						//Insert the image since it doesn't exist already
						entryImageTable.InsertAsync(this.entryImage).ContinueWith((t2) =>
						{
							var ex2 = t2.Exception;
						});

					}
				});

			}
			else
				this.entry.ImageGuid = string.Empty;

			if (this.Entry.ListId <= 0)
			{
				this.ReportError("Invalid List!");
				return;
			}

			var continuation = new Action<System.Threading.Tasks.Task>((t) =>
			{
				if (t.Exception != null)
					System.Diagnostics.Debug.WriteLine(t.Exception.Flatten());
				
				if (t.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					this.IsLoading = false;
					this.ReportError("Unable to save your new Wishlist Item!");
				}
				else
					RequestClose(this);
			});

			if (this.Entry.Id <= 0)
				App.Azure.GetTable<Models.Entry>().InsertAsync(this.Entry).ContinueWith(continuation);
			else
				App.Azure.GetTable<Models.Entry>().UpdateAsync(this.Entry).ContinueWith(continuation);

		}

		public void Cancel()
		{
			RequestClose(this);
		}

		public void Delete()
		{
			this.IsLoading = true;

			App.Azure.GetTable<Models.Entry>().DeleteAsync(entry).ContinueWith((t) =>
			                                                                   {
				var ex = t.Exception;

				this.IsLoading = false;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					this.RequestClose(this);
				}
			});
		}

		public void LoadEntry()
		{
			if (string.IsNullOrEmpty(this.EntryId))
				return;

			this.IsLoading = true;

			App.Azure.GetTable<Models.Entry>().LookupAsync(this.EntryId).ContinueWith((t) =>
			{
				var ex = t.Exception;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					this.entry = t.Result;
					RaisePropertyChanged("Entry");

					App.Azure.GetTable<Models.EntryImage>().Where(ei => ei.ImageGuid == this.entry.ImageGuid).ToListAsync().ContinueWith((t2) =>
					{
						var ex2 = t2.Exception;

						if (t2.Status == System.Threading.Tasks.TaskStatus.RanToCompletion && t2.Result.Count > 0)
						{
							this.entryImage = t2.Result[0];
							RaisePropertyChanged("EntryImage");
							RaisePropertyChanged("HasImage");
						}
					});
				}
				
				this.IsLoading = false;
			});

			if (this.entry == null)
				this.entry = new Models.Entry();

			if (this.entryImage == null)
				this.entryImage = new Models.EntryImage();

		}
	}
}