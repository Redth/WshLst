using System;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Commands;
using WshLst.Core.Models;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Microsoft.WindowsAzure.MobileServices;

namespace WshLst.Core.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		public LoginViewModel()
		{
			var settings = this.GetService<Interfaces.ISettingsProvider>();
			settings.Load();

			Platforms = new List<LoginPlatform>()
			{
				new LoginPlatform() { Name = "Twitter", Provider = MobileServiceAuthenticationProvider.Twitter },
				new LoginPlatform() { Name = "Facebook", Provider = MobileServiceAuthenticationProvider.Facebook },
				new LoginPlatform() { Name = "Google", Provider = MobileServiceAuthenticationProvider.Google },
				new LoginPlatform() { Name = "Microsoft", Provider = MobileServiceAuthenticationProvider.MicrosoftAccount },
			};

			var geo = this.GetService<Interfaces.IGeolocator>();
			geo.StartTracking();
		}

		List<LoginPlatform> platforms;
		public List<LoginPlatform> Platforms
		{
			get { return platforms; }
			set { platforms = value; RaisePropertyChanged("Platforms"); }
		}

		//public ICommand LoginCommand { get { return new MvxRelayCommand<LoginPlatform>(Login); } }

#if MONOTOUCH
		public MonoTouch.UIKit.UIViewController ViewController { get;set; }
#endif

		public void Login(LoginPlatform platform)
		{
			this.IsLoading = true;

            App.Logout();

#if MONOANDROID
			var activity = this.GetService<Cirrious.MvvmCross.Droid.Interfaces.IMvxAndroidCurrentTopActivity>().Activity;

			App.Azure.LoginAsync(activity,  platform.Provider).ContinueWith((t) =>
#elif MONOTOUCH
			App.Azure.LoginAsync(ViewController, platform.Provider).ContinueWith((t) => 
#else
			App.Azure.LoginAsync(platform.Provider).ContinueWith((t) =>
#endif
			{
				this.IsLoading = false;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion && t.Result != null && !string.IsNullOrEmpty(t.Result.UserId))
				{
					//Save our app settings for next launch
					var settings = this.GetService<Interfaces.ISettingsProvider>();
										
					settings.UserId = t.Result.UserId;
					settings.AuthenticationProvider = (int)platform.Provider;
					settings.Save();
					
					//Navigate to the Lists view
					RequestNavigate<WishListsViewModel>();
				}
				else
				{
					//Show Error
					this.ReportError("Login Failed!");
				}
			});
		}
			                                                                    
		public void CheckLogin()
		{
			var settings = this.GetService<Interfaces.ISettingsProvider>();
			
			if (settings.AuthenticationProvider >= 0)
			{
				var provider = (MobileServiceAuthenticationProvider)Enum.Parse(typeof(MobileServiceAuthenticationProvider), settings.AuthenticationProvider.ToString());

				Login(new LoginPlatform() { Provider = provider, Name = string.Empty });	
			}
		}
	}
}
