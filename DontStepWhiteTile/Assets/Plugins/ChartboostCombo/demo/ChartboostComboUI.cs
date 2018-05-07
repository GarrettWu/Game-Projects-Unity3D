using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class ChartboostComboUI : MonoBehaviourGUI
{
#if UNITY_IPHONE || UNITY_ANDROID
	void OnGUI()
	{
		beginColumn();


		if( GUILayout.Button( "Init" ) )
		{
			Chartboost.init( "ANDROID_APP_ID", "ANDROID_APP_SIGNATURE", "IOS_APP_ID", "IOS_APP_SIGNATURE" );
		}


		if( GUILayout.Button( "Cache Interstitial" ) )
		{
			Chartboost.cacheInterstitial( "default" );
		}


		if( GUILayout.Button( "Is Interstitial Cached?" ) )
		{
			Debug.Log( "is cached: " + Chartboost.hasCachedInterstitial( "default" ) );
		}


		if( GUILayout.Button( "Show Interstitial" ) )
		{
			Chartboost.showInterstitial( "default" );
		}


		if( GUILayout.Button( "Cache More Apps" ) )
		{
			Chartboost.cacheMoreApps();
		}


		if( GUILayout.Button( "Show More Apps" ) )
		{
			Chartboost.showMoreApps();
		}


		if( GUILayout.Button( "Track Event with Value and Metadata" ) )
		{
			var dict = new Dictionary<string,object>();
			dict.Add( "key", "theValue" );
			Chartboost.trackEvent( "event_with_value_and_data", 9809823, dict );
		}

		endColumn();
	}



	#region Optional: Example of Subscribing to All Events

	void OnEnable()
	{
		Chartboost.didCacheInterstitialEvent += didCacheInterstitialEvent;
		Chartboost.didFailToCacheInterstitialEvent += didFailToCacheInterstitialEvent;
		Chartboost.didFinishInterstitialEvent += didFinishInterstitialEvent;
		Chartboost.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
		Chartboost.didFailToCacheMoreAppsEvent += didFailToCacheMoreAppsEvent;
		Chartboost.didFinishMoreAppsEvent += didFinishMoreAppsEvent;
	}


	void OnDisable()
	{
		Chartboost.didCacheInterstitialEvent += didCacheInterstitialEvent;
		Chartboost.didFailToCacheInterstitialEvent += didFailToCacheInterstitialEvent;
		Chartboost.didFinishInterstitialEvent += didFinishInterstitialEvent;
		Chartboost.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
		Chartboost.didFailToCacheMoreAppsEvent += didFailToCacheMoreAppsEvent;
		Chartboost.didFinishMoreAppsEvent += didFinishMoreAppsEvent;
	}


	void didCacheInterstitialEvent( string location )
	{
		Debug.Log( "didCacheInterstitialEvent: " + location );
	}


	void didFailToCacheInterstitialEvent( string location )
	{
		Debug.Log( "didFailToCacheInterstitialEvent: " + location );
	}


	void didFinishInterstitialEvent( string reason )
	{
		Debug.Log( "didFinishInterstitialEvent: " + reason );
	}


	void didCacheMoreAppsEvent()
	{
		Debug.Log( "didCacheMoreAppsEvent" );
	}


	void didFailToCacheMoreAppsEvent()
	{
		Debug.Log( "didFailToCacheMoreAppsEvent" );
	}


	void didFinishMoreAppsEvent( string reason )
	{
		Debug.Log( "didFinishMoreAppsEvent: " + reason );
	}

	#endregion

#endif
}
