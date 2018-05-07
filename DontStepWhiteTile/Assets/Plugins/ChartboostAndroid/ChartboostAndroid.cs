using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_ANDROID
public class ChartboostAndroid
{
	private static AndroidJavaObject _plugin;


	static ChartboostAndroid()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		// find the plugin instance
		using( var pluginClass = new AndroidJavaClass( "com.prime31.ChartboostPlugin" ) )
			_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
	}


	// Starts up Chartboost and records an app install
	public static void init( string appId, string appSignature, bool shouldRequestInterstitialsInFirstSession = true )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "init", appId, appSignature, shouldRequestInterstitialsInFirstSession );
	}


	// Caches an interstitial. Location is optional. Pass in null if you do not want to specify the location.
	public static void cacheInterstitial( string location )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		if( location == null )
			location = string.Empty;

		_plugin.Call( "cacheInterstitial", location );
	}


	// Checks for a cached an interstitial. Location is optional. Pass in null if you do not want to specify the location.
	public static bool hasCachedInterstitial( string location )
	{
		if( Application.platform != RuntimePlatform.Android )
			return false;

		if( location == null )
			location = string.Empty;

		return _plugin.Call<bool>( "hasCachedInterstitial", location );
	}


	// Loads an interstitial. Location is optional. Pass in null if you do not want to specify the location.
	public static void showInterstitial( string location )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		if( location == null )
			location = string.Empty;

		_plugin.Call( "showInterstitial", location );
	}


	// Caches the more apps screen
	public static void cacheMoreApps()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "cacheMoreApps" );
	}


	// Checks to see if the more apps screen is cached
	public static bool hasCachedMoreApps()
	{
		if( Application.platform != RuntimePlatform.Android )
			return false;

		return _plugin.Call<bool>( "hasCachedMoreApps" );
	}


	// Shows the more apps screen
	public static void showMoreApps()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "showMoreApps" );
	}


	// Tracks an event with optional meta data
	public static void trackEvent( string eventIdentifier, double value, Dictionary<string,object> metaData )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		metaData = metaData ?? new Dictionary<string,object>();
		_plugin.Call( "trackEvent", eventIdentifier, value, metaData.toJson() );
	}

}
#endif
