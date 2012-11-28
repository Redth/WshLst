using System;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class EditWishListViewModel : BaseViewModel
	{
		public EditWishListViewModel(string listId)
		{
			this.ListId = listId;
		}

		public string ListId { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }

		public string ViewTitle
		{
			get { return string.IsNullOrEmpty(this.ListId) ? "New Wish List" : "Edit Wish List"; }
		}


		public void Save()
		{
			this.IsLoading = true;
			
			var model = new WishList() { Name = this.Name, Description = this.Description };

			int id = 0;
			if (!string.IsNullOrEmpty(this.ListId) && int.TryParse(this.ListId, out id))
				model.Id = id;

			var onComplete = new Action<Task>((t) =>
			{
				var ex = t.Exception;

				this.IsLoading = false;

				if (t.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
					this.ReportError("Unable to save your new Wish List!");
				else
					RequestClose(this);

			});

			if (string.IsNullOrEmpty(this.ListId))
				App.Azure.GetTable<WishList>().InsertAsync(model).ContinueWith(onComplete);
			else
				App.Azure.GetTable<WishList>().UpdateAsync(model).ContinueWith(onComplete);

		}

		public void LoadList()
		{
			if (string.IsNullOrEmpty(this.ListId))
				return;

			this.IsLoading = true;

			App.Azure.GetTable<WishList>().LookupAsync(this.ListId).ContinueWith((t) =>
			{
				var ex = t.Exception;

				this.IsLoading = false;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion && t.Result != null)
				{
					this.Name = t.Result.Name;
					this.Description = t.Result.Description;
					this.RaisePropertyChanged("Name");
					this.RaisePropertyChanged("Description");
					this.RaisePropertyChanged("ViewTitle");
				}
			});
		}

		public void Cancel()
		{
			RequestClose(this);
		}
	}
}