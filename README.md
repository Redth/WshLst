# WshLst #
#### a Wish List app for Xamarin’s Developer Showdown ####

Wsh Lst is an open source Wish List app, specifically designed to enter the first ever Xamarin Developer Showdown.  It makes use of the *Xamarin.Mobile library*, *ZXing.Net.Mobile*, *MvvmCross*, and *Azure Mobile Services*.  The app allows you to make wish lists of things you want, add items to those lists, and share the lists over the web with your contacts.

**IMPORTANT:** Before you attempt to compile and use the code, please read the ***GETTING STARTED*** section first!!!
 
![Wshlst Banner](https://raw.github.com/Redth/WshLst/master/WshLst-Banner.png)

----------


###PROJECT GOALS###
The goal of this application was an entry into the contest, specifically showing off the use of Xamarin.Mobile, however it was also a chance for me to showcase several other cross platform mobile libraries and technologies which was a great learning opportunity for myself, and I hope also a great example for others to follow, which is why I’ve open sourced the project.  Below is an outline of the various components I’ve incorporated into this application:

- Xamarin.Mobile
	- Contacts
		- Choose contacts with emails addresses from your address book
		- Share your wish list web link with them
	- Geolocation
		- Get your current location when adding an item 
		- Auto-populate ‘Where to buy’ with nearest Google place (Places API)
	- Media Picker
		- Attach a photo to a wish list item
		- Pick an Existing photo, or take a new one

- ZXing.Net.Mobile
	- Mobile Barcode Scanning
		- Optionally scan the barcode when adding an item to a wish list
		- Auto-populate the product name if Scandit API has data for the scanned barcode
	- Spirit of amarin.Mobile
		- Common API for scanning across each platform
		- Supports MonoTouch, Mono for Android, and Windows Phone

- Azure Mobile Services
	- Authentication
		- Twitter, Google, Facebook & Microsoft authentication support
		- Very easy authentication implementation
	- Data Storage
		- Stores all Wish List, Items, and Images
		- Common API across all platforms using Xamarin’s port

- MvvmCross
	- Code Sharing
		- Share as much code as possible across platforms
		- Model View View Model pattern
		- Common Model, ViewModel code layer



----------

###GETTING STARTED###
At the time of creating this project, Wsh Lst makes heavy use of the latest version of MvvmCross which in turn uses Portable Class libraries (PCL’s) extensively.  At this time, there are a few tweaks you must make to your system(s) before you may be able to compile the project.  The main issue is that the Mono for Android and MonoTouch profiles do not recognize Portable Class Libraries (PCL’s) as valid profile types to reference.  We need to ‘trick’ visual studio into allowing us to reference these PCL’s.

#####Windows Setup:#####
ALTER profile file

#####Mac Setup:######
asdfsadf


#####Azure Setup:#####
asdfsadf


----------

###OTHER PROJECTS###
-	Xamarin.Mobile - http://xamarin.com/mobileapi
-	ZXing.Net.Mobile - https://github.com/Redth/ZXing.Net.Mobile
-	MvvmCross - https://github.com/slodge/MvvmCross
-	Azure Mobile Services - https://github.com/xamarin/azure-mobile-services


###OTHER RESOURCES###
-	Youtube video
-	Powerpoint


###THANK YOU'S###
- Xamarin - Thanks to all the folks at Xamarin who make coding for mobile in C# a dream :)
- Stuart Lodge (@slodge) - Thanks for your work on MvvmCross, and awesome library, and all the one on one help and effort you put into it!
- @micjahn - Thanks for your ZXing.Net C# port of the ZXing project! 
