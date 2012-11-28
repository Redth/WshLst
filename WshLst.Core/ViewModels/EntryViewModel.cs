using System;
using Cirrious.MvvmCross.ViewModels;

namespace WshLst.Core.ViewModels
{
	public class EntryViewModel : BaseViewModel
	{
		public EntryViewModel(string listId, string entryId)
		{
			this.ListId = listId;
			this.EntryId = entryId;
		}

		public void Edit()
		{
			RequestNavigate<EditEntryViewModel>(new { listId = this.ListId, entryId = this.EntryId });
		}

		public void Delete()
		{
			IsLoading = true;

			//Delete
			App.Azure.GetTable<Models.Entry>().DeleteAsync(this.Entry).ContinueWith((t) => 
			{
				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
					RequestNavigate<WishListViewModel>(new { listId = this.ListId });
				else
				{
					this.IsLoading = false;
					ReportError("Failed to Delete Item!");
				}				
			});
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
			get { return entryImage != null && !string.IsNullOrEmpty(entryImage.ImageBase64); }
		}

		public void Cancel()
		{
			RequestClose (this);
		}

		public void LoadEntry()
		{
			this.IsLoading = true;

			App.Azure.GetTable<Models.Entry>().LookupAsync(this.EntryId).ContinueWith((t) =>
			{
				var ex = t.Exception;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					this.entry = t.Result;
					RaisePropertyChanged("Entry");

					if (!string.IsNullOrEmpty(this.entry.ImageGuid))
					{
						App.Azure.GetTable<Models.EntryImage>().Where(ei => ei.ImageGuid == this.entry.ImageGuid).ToListAsync().ContinueWith((t2) =>
						{
							var ex2 = t2.Exception;

							if (t2.Status == System.Threading.Tasks.TaskStatus.RanToCompletion
								&& t2.Result.Count > 0)
							{
								this.entryImage = t2.Result[0];
								RaisePropertyChanged("EntryImage");
								RaisePropertyChanged("HasImage");
							}

							this.IsLoading = false;
						});
					}
					else
						this.IsLoading = false;
				}
				else
				{
					this.IsLoading = false;
					this.ReportError("Could not load Item!");
				}
			});
		}
	}
}