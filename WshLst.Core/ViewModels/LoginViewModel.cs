using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ExtensionMethods;
using Microsoft.WindowsAzure.MobileServices;
using WshLst.Core.Interfaces;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		public LoginViewModel()
		{
			var settings = this.GetService<ISettingsProvider>();
			settings.Load();

			Platforms = new List<LoginPlatform>
				{
					new LoginPlatform {Name = "Twitter", Provider = MobileServiceAuthenticationProvider.Twitter},
					new LoginPlatform {Name = "Facebook", Provider = MobileServiceAuthenticationProvider.Facebook},
					new LoginPlatform {Name = "Google", Provider = MobileServiceAuthenticationProvider.Google},
					new LoginPlatform {Name = "Microsoft", Provider = MobileServiceAuthenticationProvider.MicrosoftAccount},
				};

			var geo = this.GetService<IGeolocator>();
			geo.StartTracking();
		}

		private List<LoginPlatform> _platforms;

		public List<LoginPlatform> Platforms
		{
			get { return _platforms; }
			set
			{
				_platforms = value;
				RaisePropertyChanged(() => Platforms);
			}
		}

#if MONOTOUCH
		public MonoTouch.UIKit.UIViewController ViewController { get;set; }
#endif

		public ICommand LoginCommand
		{
			get { return new MvxRelayCommand<LoginPlatform>(Login); }
		}

		public void Login(LoginPlatform platform)
		{
			IsLoading = true;

			App.Logout();

#if MONOANDROID
			var activity = this.GetService<Cirrious.MvvmCross.Droid.Interfaces.IMvxAndroidCurrentTopActivity>().Activity;

            App.Azure.LoginAsync(activity, platform.Provider).ContinueWith((t) => HandleLoginResult(t, platform));
#elif MONOTOUCH
            App.Azure.LoginAsync(ViewController, platform.Provider).ContinueWith((t) => HandleLoginResult(t, platform));
#else
            App.Azure.LoginAsync(platform.Provider).ContinueWith((t) => HandleLoginResult(t, platform));
#endif

		}

		void HandleLoginResult(Task<MobileServiceUser> t, LoginPlatform platform = null)
		{
			IsLoading = false;

			if (t.Status == TaskStatus.RanToCompletion && t.Result != null && !string.IsNullOrEmpty(t.Result.UserId))
			{
				//Save our app settings for next launch
				var settings = this.GetService<ISettingsProvider>();

				settings.UserId = t.Result.UserId;

				if (platform != null)
					settings.AuthenticationProvider = (int)platform.Provider;

				settings.Save();

				//Navigate to the Lists view
				RequestNavigate<WishListsViewModel>();
			}
			else
			{
				//Show Error
				ReportError("Login Failed!");
			}
		}

		public void CheckLogin()
		{
			var settings = this.GetService<ISettingsProvider>();

			if (settings.AuthenticationProvider < 0) return;
			
			var provider =
				(MobileServiceAuthenticationProvider)Enum.Parse(typeof (MobileServiceAuthenticationProvider), 
                                                                settings.AuthenticationProvider.ToString());

            Login(new LoginPlatform {Provider = provider, Name = string.Empty});
		}
	}
}