#UPDATE: December 17, 2012#
##Wsh Lst Wins Xamarin Developer Showdown - Best Windows Phone Platform##
Huge thanks to Xamarin for choosing Wsh Lst as a winner!  Congrats to Stuart Lodge for taking the grand prize!
Read the Full Blog Post at Xamarin: http://blog.xamarin.com/2012/12/17/xamarin-developer-showdown-winning-entries-showcase-xamarin-mobile/

# WshLst #
#### a Wish List app for Xamarin’s Developer Showdown ####

Wsh Lst is an open source Wish List app, specifically designed to enter the first ever Xamarin Developer Showdown.  It makes use of the *Xamarin.Mobile library*, *ZXing.Net.Mobile*, *MvvmCross*, and *Azure Mobile Services*.  The app allows you to make wish lists of things you want, add items to those lists, and share the lists over the web with your contacts.

**IMPORTANT:** Before you attempt to compile and use the code, please read the ***GETTING STARTED*** section first!!!
 
![Wshlst Banner](https://raw.github.com/Redth/WshLst/master/WshLst-Banner.png)

#[Watch an intro video about the project!](http://www.youtube.com/watch?v=HfaoAVTbwL8&feature=youtube_gdata "Watch an Intro Video about the Project!")#

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
Since this app uses prebuilt binaries of MvvmCross, it should build for you as is, however if you have porblems getting the Android app to build, we need to trick Visual Studio Mono for Android projects to be able to reference Portable Class Libraries:

1. Open the folder: *C:\Program Files (x86)\Referenced Assemblies\Microsoft\Framework\.NETPortable\v4.0\Profile\Profile104\SupportedFrameworks\\*
2. Create a new file named *MonoAndroid,Version=v1.6+.xml* with the following contents:

	```
	<?xml version="1.0" encoding="utf-8"?>
	<Framework DisplayName="Mono for Android"
	  Identifier="MonoAndroid"
	  Profile="*"
	  MinimumVersion="1.6"
	  MaximumVersion="*" />
	```
3. If you had Visual Studio open, you'll need to restart it



#####Mac Setup:######
At this time, you should have no problems opening the MonoTouch and Mono for Android projects on the mac.

If you do have issues, try the following:

1. Edit the file */Library/Frameworks/Mono.framework/Versions/Current/lib/mono/xbuild/Microsoft/Portable/v4.0/Microsoft.Portable.CSharp.targets*
2. Find the PropertyGroup that sets *<TargetFrameworkIdentifier>MonoTouch</TargetFrameworkIdentifier>*
3. Ensure the following lines exist in this PropertyGroup:

	```
	<CscToolExe>smcs</CscToolExe>
	<CscToolPath>/Developer/MonoTouch/usr/bin</CscToolPath>
	```


#####Azure Setup:#####
1. Create a new Azure Mobile Service and open its dashboard
2. Note the *MOBILE SERVICE URL* (eg: https://wshlst.azure-mobile.net) on the right hand side
3. Click the *Manage Keys* button at the bottom.  Note the Application Key.
4. Edit the *WshLst.Core\Config.cs* file
	1. Set the AZURE\_MOBILE\_SERVICE\_URL constant value to the URL from step 2 (make sure you do NOT have a trailing slash)
	2. Set the AZURE\_MOBILE\_SERVICE\_APPKEY constant value to the Application key from step 3
5. In your Azure Mobile Service, Create the following Data tables (their columns will be dynamically created at runtime):
	1. WishList
	2. Entry
	3. EntryImage
6. Open the *Azure-Table-Scripts.js* file from this repository and copy/paste the corresponding scripts for each table's read/insert/update/delete operations in the azure portal
7. In your Azure Mobile Service's *Identity* configuration tab, setup the correct keys/secrets for all the authentication providers.  You will need to follow the Azure help section to setup applications on each authentication provider (eg: Twitter, Facebook, Google, Microsoft).
8. Create a new Azure Website and open its dashboard
9. Download the new website's publishing profile
10. Open the *WshLst.Web.sln* solution, build the web site, and publish it using your new website's publishing profile.
11. Edit the *WshLst.Core\Config.cs* file again
	1. Set the AZURE\_WEBSITE\_URL to the url of the website you just made (be sure you do NOT include the trailing slash)

#####OPTIONAL#####
Without these optional steps you will be unable to utilize the Google Places or Barcode Scanning features:

1. Signup for a Google Places API, note your API Key
2. Signup for a Scandit API key, note the key
3. Edit the *WshLst.Core\Config.cs* file again
	1. Set the GOOGLE\_PLACES\_API\_KEY to the key you just created
	2. Set the SCANDIT\_API\_KEY to the key you just created

----------

###TROUBLESHOOTING###

1. iOS App Crashes - Try increasing the number of trampolines: [http://docs.xamarin.com/ios/troubleshooting#Ran_out_of_trampolines_of_type_2](http://docs.xamarin.com/ios/troubleshooting#Ran_out_of_trampolines_of_type_2)


----------

###OTHER PROJECTS###
-	Xamarin.Mobile - http://xamarin.com/mobileapi
-	ZXing.Net.Mobile - https://github.com/Redth/ZXing.Net.Mobile
-	MvvmCross - https://github.com/slodge/MvvmCross
-	Azure Mobile Services - https://github.com/xamarin/azure-mobile-services


----------

###THANK YOU'S###
- Xamarin - Thanks to all the folks at Xamarin who make coding for mobile in C# a dream :)
- Stuart Lodge (@slodge) - Thanks for your work on MvvmCross, and awesome library, and all the one on one help and effort you put into it!
- @micjahn - Thanks for your ZXing.Net C# port of the ZXing project! 


----------

##LICENSE##
Apache WshLst Copyright 2012 The Apache Software Foundation

This product includes software developed at The Apache Software Foundation (http://www.apache.org/).
