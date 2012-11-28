using Microsoft.WindowsAzure.MobileServices;

namespace WshLst.Core.Models
{
	public class LoginPlatform
	{
		public string Name { get; set; }
		public MobileServiceAuthenticationProvider Provider { get; set; }
	}
}