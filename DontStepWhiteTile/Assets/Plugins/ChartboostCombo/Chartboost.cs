using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_IPHONE || UNITY_ANDROID

#if UNITY_ANDROID
using Manager = ChartboostAndroidManager;
#elif UNITY_IPHONE
using Manager = ChartboostManager;
#endif

public class Chartboost
{
	// Fired when an interstitial is cached
	public static event Action<string> didCacheInterstitialEvent;

	// Fired when an interstitial fails to load
	public static event Action<string> didFailToCacheInterstitialEvent;

	// Fired when an interstitial is finished. Possible reasons are 'dismiss', 'close' and 'click'.
	public static event Action<string> didFinishInterstitialEvent;

	// Fired when the more apps screen is cached
	public static event Action didCacheMoreAppsEvent;

	// Fired when the more apps screen fails to load
	public static event Action didFailToCacheMoreAppsEvent;

	// Fired when the more apps screen is finished. Includes the reason the screen was closed.
	// Possible reasons are 'dismiss', 'close' and 'click'
	public static event Action<string> didFinishMoreAppsEvent;


	#region Events


	#endregion

	static Chartboost()
	{
		Manager.didCacheInterstitialEvent += ( location ) => { didCacheInterstitialEvent.fire( location ); };
		Manager.didFailToCacheInterstitialEvent += ( location ) => { didFailToCacheInterstitialEvent.fire( location ); };
		Manager.didFinishInterstitialEvent += ( reason ) => { didFinishInterstitialEvent.fire( reason ); };
		Manager.didCacheMoreAppsEvent += () => { didCacheMoreAppsEvent.fire(); };
		Manager.didFailToCacheMoreAppsEvent += () => { didFailToCacheMoreAppsEvent.fire(); };
		Manager.didFinishMoreAppsEvent += ( reason ) => { didFinishMoreAppsEvent.fire( reason ); };
	}


	// Starts up Chartboost and records an app install
	public static void init( string androidAppId, string androidAppSignature, string iosAppId, string iosAppSignature, bool shouldRequestInterstitialsInFirstSession = true )
	{
#if UNITY_IPHONE
		ChartboostBinding.init( iosAppId, iosAppSignature, shouldRequestInterstitialsInFirstSession );
#elif UNITY_ANDROID
		ChartboostAndroid.init( androidAppId, androidAppSignature, shouldRequestInterstitialsInFirstSession );
#endif
	}


	// Caches an interstitial. Location is optional. Pass in null if you do not want to specify the location.
	public static void cacheInterstitial( string location )
	{
#if UNITY_IPHONE
		ChartboostBinding.cacheInterstitial( location );
#elif UNITY_ANDROID
		ChartboostAndroid.cacheInterstitial( location );
#endif
	}


	// Checks for a cached an interstitial. Location is optional. Pass in null if you do not want to specify the location.
	public static bool hasCachedInterstitial( string location )
	{
#if UNITY_IPHONE
		return ChartboostBinding.hasCachedInterstitial( location );
#elif UNITY_ANDROID
		return ChartboostAndroid.hasCachedInterstitial( location );
#else
		return false;
#endif
	}


	// Loads an interstitial. Location is optional. Pass in null if you do not want to specify the location.
	public static void showInterstitial( string location )
	{
#if UNITY_IPHONE
		ChartboostBinding.showInterstitial( location );
#elif UNITY_ANDROID
		ChartboostAndroid.showInterstitial( location );
#endif
	}


	// Caches the more apps screen
	public static void cacheMoreApps()
	{
#if UNITY_IPHONE
		ChartboostBinding.cacheMoreApps();
#elif UNITY_ANDROID
		ChartboostAndroid.cacheMoreApps();
#endif
	}


	// Shows the more apps screen
	public static void showMoreApps()
	{
#if UNITY_IPHONE
		ChartboostBinding.showMoreApps();
#elif UNITY_ANDROID
		ChartboostAndroid.showMoreApps();
#endif
	}


	// Tracks an event with optional meta data
	public static void trackEvent( string eventIdentifier, double value, Dictionary<string,object> metaData )
	{
		metaData = metaData ?? new Dictionary<string,object>();

#if UNITY_IPHONE
		ChartboostBinding.trackEventWithValueAndMetadata( eventIdentifier, (float)value, metaData );
#elif UNITY_ANDROID
		ChartboostAndroid.trackEvent( eventIdentifier, value, metaData );
#endif
	}

}
#endif


#if UNITY_IPHONE

#elif UNITY_ANDROID

#endif