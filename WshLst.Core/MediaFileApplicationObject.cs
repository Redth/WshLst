using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using WshLst.Core.Interfaces;
using Xamarin.Media;

namespace WshLst.Core
{
	public class MediaFileApplicationObject : MvxApplicationObject, IMediaFileSource
	{
		public MediaFileApplicationObject()
		{
		}

		public Task<MediaFile> GetPhoto(bool takeNew)
		{
#if MONOANDROID
			var activity = this.GetService<Cirrious.MvvmCross.Droid.Interfaces.IMvxAndroidCurrentTopActivity>();
            var picker = new MediaPicker(activity.Activity);
#else
			var picker = new MediaPicker();
#endif
			return takeNew ? picker.TakePhotoAsync(new StoreCameraMediaOptions()) : picker.PickPhotoAsync();
		}
	}
}