using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Core;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json.Linq;
using WshLst.Core.Interfaces;
using Xamarin.Geolocation;

namespace WshLst.Core
{
	public class GeolocatorApplicationObject : MvxApplicationObject, IGeolocator
	{
		const string GOOGLE_PLACES_API_KEY = "AIzaSyBlrLpCUYWCl2qSlTj1Gx7l9OiBgEmKSE8";

		Geolocator geo;

        string[] IGNORED_PLACE_TYPES = new string[] { "administrative_area_level_1", "administrative_area_level_2",
            "administrative_area_level_3", "colloquial_area", "country", "floor", "geocode", "intersection", "locality", "natural_feature",
            "neighborhood", "political", "point_of_interest", "post_box", "postal_code", "postal_code_prefix", "postal_town",
            "premise", "room", "route", "street_address", "street_number", "sublocality", "sublocality_level_4", "sublocality_level_5",
            "sublocality_level_3", "sublocality_level_2", "sublocality_level_1", "subpremise", "transit_station" };

        public GeolocatorApplicationObject()
		{
#if MONOANDROID
			var androidGlobal = this.GetService<Cirrious.MvvmCross.Droid.Interfaces.IMvxAndroidGlobals>();
			geo = new Geolocator(androidGlobal.ApplicationContext);
#else
			geo = new Geolocator();
#endif
			geo.DesiredAccuracy = 250;
			geo.PositionChanged += geo_PositionChanged;
			geo.PositionError += geo_PositionError;	
		}

		void geo_PositionError(object sender, PositionErrorEventArgs e)
		{
			//Position error
		}

		void geo_PositionChanged(object sender, PositionEventArgs e)
		{
			lock (geo)
			{
				currentPosition = e.Position;

				System.Diagnostics.Debug.WriteLine("CURRENT POSITION: " + currentPosition.Latitude.ToString() + ", " + currentPosition.Longitude.ToString());
			}
		}

		public void StartTracking() 
        {
			if (geo.IsGeolocationAvailable && !geo.IsListening)
				geo.StartListening(30000, 0, false);
		}

		public void StopTracking()
		{
			if (geo.IsListening)
				geo.StopListening();
		}
		
		Position currentPosition = null;
		public Position CurrentPosition
		{
			get { lock (geo) return currentPosition; }
		}


		public void LookupNearbyPlaces(Action<string> onComplete)
		{
			var wc = new WebClient();
            var url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&sensor=true&types={3}&key={4}";

            var placeTypes = new string[]
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

			lock(geo)
				currentPos = currentPosition;

			wc.DownloadStringCompleted += (s, e) =>
			{
				string result = null;

				try
				{
					var json = JObject.Parse(e.Result);
                    Console.WriteLine(json);
					
                    result = (json["results"] as JArray)[0]["name"].ToString();
				}
				catch { }

				if (onComplete != null)
					onComplete(result);
			};
		
			if (currentPos != null)
			{
				var uri = new Uri(string.Format(url, currentPos.Latitude, currentPos.Longitude, 2000, string.Join("|", placeTypes), GOOGLE_PLACES_API_KEY));

                Console.WriteLine(uri.ToString());

				wc.DownloadStringAsync(uri);
			}
		}
	}
}