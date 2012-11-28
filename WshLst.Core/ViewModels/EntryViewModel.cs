using System.Threading.Tasks;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class EntryViewModel : BaseViewModel
	{
		public string EntryId = string.Empty;
		public string ListId = string.Empty;

		private WishList _wishList;

		private Entry _entry;

		private EntryImage _entryImage;

		public EntryViewModel(string listId, string entryId)
		{
			ListId = listId;
			EntryId = entryId;
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
			get { return _entryImage != null && !string.IsNullOrEmpty(_entryImage.ImageBase64); }
		}

		public void Edit()
		{
			RequestNavigate<EditEntryViewModel>(new {listId = ListId, entryId = EntryId});
		}

		public void Delete()
		{
			IsLoading = true;

			//Delete
			App.Azure.GetTable<Entry>().DeleteAsync(Entry).ContinueWith(t =>
				{
					if (t.Status == TaskStatus.RanToCompletion)
						RequestNavigate<WishListViewModel>(new {listId = ListId});
					else
					{
						IsLoading = false;
						ReportError("Failed to Delete Item!");
					}
				});
		}

		public void Cancel()
		{
			RequestClose(this);
		}

		public void LoadEntry()
		{
			IsLoading = true;

			App.Azure.GetTable<Entry>().LookupAsync(EntryId).ContinueWith(t =>
				{
					var ex = t.Exception;

					if (t.Status == TaskStatus.RanToCompletion)
					{
						_entry = t.Result;
						RaisePropertyChanged(() => Entry);

						if (!string.IsNullOrEmpty(_entry.ImageGuid))
						{
							App.Azure.GetTable<EntryImage>()
							   .Where(ei => ei.ImageGuid == _entry.ImageGuid)
							   .ToListAsync()
							   .ContinueWith(t2 =>
								   {
									   var ex2 = t2.Exception;

									   if (t2.Status == TaskStatus.RanToCompletion && t2.Result.Count > 0)
									   {
										   _entryImage = t2.Result[0];
										   RaisePropertyChanged(() => EntryImage);
										   RaisePropertyChanged(() => HasImage);
									   }

									   IsLoading = false;
								   });
						}
						else
							IsLoading = false;
					}
					else
					{
						IsLoading = false;
						ReportError("Could not load Item!");
					}
				});
		}
	}
}