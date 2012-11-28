using Android.App;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.ViewModels;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "New Wish List", Icon = "@drawable/icontransparent")]
	public class EditWishListView : MvxBindingActivityView<EditWishListViewModel>
	{
		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_EditWishListView);

			ViewModel.PropertyChanged += (s, e) =>
				{
					switch (e.PropertyName)
					{
						case "ViewTitle":
							Title = ViewModel.ViewTitle;
							break;
					}
				};
		}
	}
}