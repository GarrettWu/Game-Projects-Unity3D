using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ChartboostEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		ChartboostManager.didFailToCacheInterstitialEvent += didFailToLoadInterstitialEvent;
		ChartboostManager.didFinishInterstitialEvent += didFinishInterstitialEvent;
		ChartboostManager.didCacheInterstitialEvent += didCacheInterstitialEvent;
		ChartboostManager.didFailToCacheMoreAppsEvent += didFailToLoadMoreAppsEvent;
		ChartboostManager.didFinishMoreAppsEvent += didFinishMoreAppsEvent;
		ChartboostManager.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		ChartboostManager.didFailToCacheInterstitialEvent -= didFailToLoadInterstitialEvent;
		ChartboostManager.didFinishInterstitialEvent -= didFinishInterstitialEvent;
		ChartboostManager.didCacheInterstitialEvent -= didCacheInterstitialEvent;
		ChartboostManager.didFailToCacheMoreAppsEvent -= didFailToLoadMoreAppsEvent;
		ChartboostManager.didFinishMoreAppsEvent -= didFinishMoreAppsEvent;
		ChartboostManager.didCacheMoreAppsEvent -= didCacheMoreAppsEvent;
	}



	void didFailToLoadInterstitialEvent( string location )
	{
		Debug.Log( "didFailToLoadInterstitialEvent: " + location );
	}


	void didFinishInterstitialEvent( string reason )
	{
		Debug.Log( "didFinishInterstitialEvent: " + reason );
	}

	
	void didCacheInterstitialEvent( string location )
	{
		Debug.Log( "didCacheInterstitialEvent: " + location );
	}

	
	void didFailToLoadMoreAppsEvent()
	{
		Debug.Log( "didFailToLoadMoreAppsEvent" );
	}


	void didFinishMoreAppsEvent( string param )
	{
		Debug.Log( "didFinishMoreAppsEvent: " + param );
	}
	
	
	void didCacheMoreAppsEvent()
	{
		Debug.Log( "didCacheMoreAppsEvent" );
	}

#endif
}


