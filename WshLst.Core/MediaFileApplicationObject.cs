using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Core;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json.Linq;
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
			if (takeNew)
                return picker.TakePhotoAsync(new StoreCameraMediaOptions());
            else
                return picker.PickPhotoAsync();
        }
    }
}