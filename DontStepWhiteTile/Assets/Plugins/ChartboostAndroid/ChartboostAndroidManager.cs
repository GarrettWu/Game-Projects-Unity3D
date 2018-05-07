using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class ChartboostAndroidManager : AbstractManager
{
#if UNITY_ANDROID
	// Fired when the more apps screen is cached
	public static event Action didCacheMoreAppsEvent;
	
	// Fired when the more apps screen fails to load
	public static event Action didFailToCacheMoreAppsEvent;

	// Fired when an interstitial is cached
	public static event Action<string> didCacheInterstitialEvent;
	
	// Fired when an interstitial fails to load
	public static event Action<string> didFailToCacheInterstitialEvent;

	// Fired when an interstitial is finished. Possible reasons are 'dismiss', 'close' and 'click'
	public static event Action<string> didFinishInterstitialEvent;

	// Fired when the more apps screen is finished. Possible reasons are 'dismiss', 'close' and 'click'
	public static event Action<string> didFinishMoreAppsEvent;

	// Fired whent he more apps screen is closed
	public static event Action didCloseMoreAppsEvent;

	// Fired when an interstitial is shown
	public static event Action<string> didShowInterstitialEvent;

	// Fired when the more app screen is shown
	public static event Action didShowMoreAppsEvent;

	
	
	static ChartboostAndroidManager()
	{
		AbstractManager.initialize( typeof( ChartboostAndroidManager ) );
	}


	public void didFailToLoadMoreApps( string empty )
	{
		if( didFailToCacheMoreAppsEvent != null )
			didFailToCacheMoreAppsEvent();
	}


	public void didCacheInterstitial( string location )
	{
		if( didCacheInterstitialEvent != null )
			didCacheInterstitialEvent( location );
	}


	public void didCacheMoreApps( string empty )
	{
		if( didCacheMoreAppsEvent != null )
			didCacheMoreAppsEvent();
	}


	public void didFinishInterstitial( string param )
	{
		if( didFinishInterstitialEvent != null )
			didFinishInterstitialEvent( param );
	}


	public void didFinishMoreApps( string param )
	{
		if( didFinishMoreAppsEvent != null )
			didFinishMoreAppsEvent( param );
	}


	public void didCloseMoreApps( string empty )
	{
		if( didCloseMoreAppsEvent != null )
			didCloseMoreAppsEvent();
	}


	public void didFailToLoadInterstitial( string location )
	{
		if( didFailToCacheInterstitialEvent != null )
			didFailToCacheInterstitialEvent( location );
	}


	public void didShowInterstitial( string location )
	{
		if( didShowInterstitialEvent != null )
			didShowInterstitialEvent( location );
	}


	public void didShowMoreApps( string empty )
	{
		if( didShowMoreAppsEvent != null )
			didShowMoreAppsEvent();
	}
	
#endif
}

