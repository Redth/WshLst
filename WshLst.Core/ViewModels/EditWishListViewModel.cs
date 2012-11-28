using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class EditWishListViewModel : BaseViewModel
	{
		public EditWishListViewModel(string listId)
		{
			ListId = listId;
		}

		public string ListId { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }

		public string ViewTitle
		{
			get { return string.IsNullOrEmpty(ListId) ? "New Wish List" : "Edit Wish List"; }
		}

		public ICommand SaveCommand
		{
			get { return new MvxRelayCommand(Save); }
		}

		public void Save()
		{
			IsLoading = true;

			var model = new WishList {Name = Name, Description = Description};

			int id = 0;
			if (!string.IsNullOrEmpty(ListId) && int.TryParse(ListId, out id))
				model.Id = id;

			var onComplete = new Action<Task>(t =>
				{
					var ex = t.Exception;

					IsLoading = false;

					if (t.Status != TaskStatus.RanToCompletion)
						ReportError("Unable to save your new Wish List!");
					else
						RequestClose(this);
				});

			if (string.IsNullOrEmpty(ListId))
				App.Azure.GetTable<WishList>().InsertAsync(model).ContinueWith(onComplete);
			else
				App.Azure.GetTable<WishList>().UpdateAsync(model).ContinueWith(onComplete);
		}

		public void LoadList()
		{
			if (string.IsNullOrEmpty(ListId))
				return;

			IsLoading = true;

			App.Azure.GetTable<WishList>().LookupAsync(ListId).ContinueWith(t =>
				{
					var ex = t.Exception;

					IsLoading = false;

					if (t.Status == TaskStatus.RanToCompletion && t.Result != null)
					{
						Name = t.Result.Name;
						Description = t.Result.Description;
						RaisePropertyChanged(() => Name);
						RaisePropertyChanged(() => Description);
						RaisePropertyChanged(() => ViewTitle);
					}
				});
		}

		public ICommand CancelCommand
		{
			get { return new MvxRelayCommand(Cancel); }
		}

		public void Cancel()
		{
			RequestClose(this);
		}
	}
}