using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ExtensionMethods;
using WshLst.Core.Models;
using WshLst.Core.Interfaces;
using Xamarin.Contacts;

namespace WshLst.Core.ViewModels
{
	public class ShareViewModel : BaseViewModel
	{
		private List<SelectableContact> _contacts;

		public ShareViewModel(string listId, string listGuid)
		{
			ListId = listId;
			ListGuid = listGuid;
		}

		public string ListId { get; set; }
		public string ListGuid { get; set; }

		public List<SelectableContact> Contacts
		{
			get { return _contacts; }
			set
			{
				_contacts = value;
				RaisePropertyChanged(() => Contacts);
			}
		}

		public void LoadContacts()
		{
			var addressBook = this.GetService<IAddressBookSource>();
			_contacts = addressBook.GetContactsWithEmailAddresses().ToList();

			RaisePropertyChanged(() => Contacts);
		}

		public ICommand CancelCommand
		{
			get { return new MvxRelayCommand(Cancel); }
		}

		public void Cancel()
		{
			RequestClose(this);
		}
		
		public ICommand FinishedCommand
		{
			get { return new MvxRelayCommand(Finished); }
		}

		public void Finished()
		{
			RequestNavigate<WishListViewModel>(new {listId = ListId});
		}

		public string GetEmailTo()
		{
			if (_contacts == null || _contacts.Count <= 0)
				return string.Empty;

			var selContacts = from c in _contacts where c.IsSelected select c.Email;

			return string.Join("; ", selContacts);
		}

		public string GetEmailSubject()
		{
			return "I've shared a Wish List with you!";
		}

		public string GetEmailBody()
		{
			var url = string.Format(Config.AZURE_WEBSITE_URL + "/?id={0}", ListGuid);

			return "Hi, I'd like to share my wish list with you!  You can see my Wish List online at: \r\n\r\n" + url;
		}
	}
}