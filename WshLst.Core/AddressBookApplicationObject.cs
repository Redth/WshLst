using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.Generic;
using System.Linq;
using WshLst.Core.Models;
using WshLst.Core.Interfaces;
using Xamarin.Contacts;

namespace WshLst.Core
{
	public class AddressBookApplicationObject : MvxApplicationObject, IAddressBookSource
	{
		public IEnumerable<SelectableContact> GetContactsWithEmailAddresses()
		{
#if MONOANDROID
			var androidGlobals = this.GetService<Cirrious.MvvmCross.Droid.Interfaces.IMvxAndroidGlobals>();
			var ab = new AddressBook(androidGlobals.ApplicationContext);
#else
			var ab = new AddressBook();
#endif

			var allContacts = ab.ToList();

			return (from c in allContacts
					orderby c.DisplayName
					where c.Emails != null && c.Emails.Any()
					let email = c.Emails.FirstOrDefault()
					where email != null && !string.IsNullOrEmpty(email.Address)
					select
						new SelectableContact
						{
							DisplayName = c.DisplayName,
							Email = email.Address,
							IsSelected = false
						}).ToList();
		}
	}
}