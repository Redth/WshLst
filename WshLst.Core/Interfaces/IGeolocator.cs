using System;
using Xamarin.Geolocation;

namespace WshLst.Core.Interfaces
{
	public interface IGeolocator
	{
		Position CurrentPosition { get; }

		void StartTracking();
		void StopTracking();

		void LookupNearbyPlaces(Action<string> onComplete);
	}
}