using System;
using System.Diagnostics;
using System.Net;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json.Linq;
using WshLst.Core.Interfaces;
using Xamarin.Geolocation;

namespace WshLst.Core
{
	public class GeolocatorApplicationObject : MvxApplicationObject, IGeolocator
	{
		private readonly Geolocator _geo;

		private Position _currentPosition;

		public GeolocatorApplicationObject()
		{
#if MONOANDROID
			var androidGlobal = this.GetService<Cirrious.MvvmCross.Droid.Interfaces.IMvxAndroidGlobals>();
			geo = new Geolocator(androidGlobal.ApplicationContext);
#else
			_geo = new Geolocator();
#endif
			_geo.DesiredAccuracy = 250;
			_geo.PositionChanged += geo_PositionChanged;
			_geo.PositionError += geo_PositionError;
		}

		public void StartTracking()
		{
			if (_geo.IsGeolocationAvailable && !_geo.IsListening)
				_geo.StartListening(30000, 0, false);
		}

		public void StopTracking()
		{
			if (_geo.IsListening)
				_geo.StopListening();
		}

		public Position CurrentPosition
		{
			get { lock (_geo) return _currentPosition; }
		}


		public void LookupNearbyPlaces(Action<string> onComplete)
		{
			var wc = new WebClient();
			var url =
				"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&sensor=true&types={3}&key={4}";

			var placeTypes = new[]
				{
					"art_gallery",
					"bicycle_store",
					"book_store",
					"bowling_alley",
					"car_dealer",
					"clothing_store",
					"department_store",
					"electronics_store",
					"furniture_store",
					"grocery_or_supermarket",
					"hardware_store",
					"home_goods_store",
					"jewelry_store",
					"liquor_store",
					"movie_rental",
					"pet_store",
					"pharmacy",
					"shoe_store",
					"shopping_mall",
					"store"
				};

			Position currentPos = null;

			lock (_geo)
				currentPos = _currentPosition;

			wc.DownloadStringCompleted += (s, e) =>
				{
					string result = null;

					try
					{
						var json = JObject.Parse(e.Result);
						Console.WriteLine(json);

						result = ((JArray) json["results"])[0]["name"].ToString();
					}
					catch (Exception)
					{
					}

					if (onComplete != null)
						onComplete(result);
				};

			if (currentPos != null)
			{
				var uri =
					new Uri(string.Format(url, currentPos.Latitude, currentPos.Longitude, 2000, string.Join("|", placeTypes),
					                      Config.GOOGLE_PLACES_API_KEY));

				Console.WriteLine(uri.ToString());

				wc.DownloadStringAsync(uri);
			}
		}

		private void geo_PositionError(object sender, PositionErrorEventArgs e)
		{
			//Position error
		}

		private void geo_PositionChanged(object sender, PositionEventArgs e)
		{
			lock (_geo)
			{
				_currentPosition = e.Position;

				Debug.WriteLine("CURRENT POSITION: " + _currentPosition.Latitude + ", " +
				                _currentPosition.Longitude);
			}
		}
	}
}