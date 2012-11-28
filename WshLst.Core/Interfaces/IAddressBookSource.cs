using System.Collections.Generic;
using WshLst.Core.Models;

namespace WshLst.Core.Interfaces
{
	public interface IAddressBookSource
	{
		IEnumerable<SelectableContact> GetContactsWithEmailAddresses();
	}
}