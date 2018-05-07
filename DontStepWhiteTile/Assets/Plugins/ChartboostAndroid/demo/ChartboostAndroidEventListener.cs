using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ChartboostAndroidEventListener : MonoBehaviour
{
#if UNITY_ANDROID

	void OnEnable()
	{
		// Listen to all events for illustration purposes
		ChartboostAndroidManager.didFailToCacheMoreAppsEvent += didFailToLoadMoreAppsEvent;
		ChartboostAndroidManager.didCacheInterstitialEvent += didCacheInterstitialEvent;
		ChartboostAndroidManager.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
		ChartboostAndroidManager.didFinishInterstitialEvent += didFinishInterstitialEvent;
		ChartboostAndroidManager.didFinishMoreAppsEvent += didFinishMoreAppsEvent;
		ChartboostAndroidManager.didCloseMoreAppsEvent += didCloseMoreAppsEvent;
		ChartboostAndroidManager.didFailToCacheInterstitialEvent += didFailToLoadInterstitialEvent;
		ChartboostAndroidManager.didShowInterstitialEvent += didShowInterstitialEvent;
		ChartboostAndroidManager.didShowMoreAppsEvent += didShowMoreAppsEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		ChartboostAndroidManager.didFailToCacheMoreAppsEvent -= didFailToLoadMoreAppsEvent;
		ChartboostAndroidManager.didCacheInterstitialEvent -= didCacheInterstitialEvent;
		ChartboostAndroidManager.didCacheMoreAppsEvent -= didCacheMoreAppsEvent;
		ChartboostAndroidManager.didFinishInterstitialEvent -= didFinishInterstitialEvent;
		ChartboostAndroidManager.didFinishMoreAppsEvent -= didFinishMoreAppsEvent;
		ChartboostAndroidManager.didCloseMoreAppsEvent -= didCloseMoreAppsEvent;
		ChartboostAndroidManager.didFailToCacheInterstitialEvent -= didFailToLoadInterstitialEvent;
		ChartboostAndroidManager.didShowInterstitialEvent -= didShowInterstitialEvent;
		ChartboostAndroidManager.didShowMoreAppsEvent -= didShowMoreAppsEvent;
	}



	void didFailToLoadMoreAppsEvent()
	{
		Debug.Log( "didFailToLoadMoreAppsEvent" );
	}


	void didCacheInterstitialEvent( string location )
	{
		Debug.Log( "didCacheInterstitialEvent: " + location );
	}


	void didCacheMoreAppsEvent()
	{
		Debug.Log( "didCacheMoreAppsEvent" );
	}


	void didFinishInterstitialEvent( string param )
	{
		Debug.Log( "didFinishInterstitialEvent: " + param );
	}


	void didFinishMoreAppsEvent( string param )
	{
		Debug.Log( "didFinishMoreAppsEvent: " + param );
	}


	void didCloseMoreAppsEvent()
	{
		Debug.Log( "didCloseMoreAppsEvent" );
	}


	void didFailToLoadInterstitialEvent( string location )
	{
		Debug.Log( "didFailToLoadInterstitialEvent: " + location );
	}


	void didShowInterstitialEvent( string location )
	{
		Debug.Log( "didShowInterstitialEvent: " + location );
	}


	void didShowMoreAppsEvent()
	{
		Debug.Log( "didShowMoreAppsEvent" );
	}
			
#endif
}


