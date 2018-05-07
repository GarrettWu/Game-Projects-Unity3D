using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_IPHONE
public class ChartboostManager : AbstractManager
{
	// Fired when an interstitial fails to load
	public static event Action<string> didFailToCacheInterstitialEvent;

	// Fired when an interstitial is finished. Possible reasons are 'dismiss', 'close' and 'click'
	public static event Action<string> didFinishInterstitialEvent;
	
	// Fired when an interstitial is cached
	public static event Action<string> didCacheInterstitialEvent;

	// Fired when the more apps screen fails to load
	public static event Action didFailToCacheMoreAppsEvent;

	// Fired when the more apps screen is finished. Possible reasons are 'dismiss', 'close' and 'click'
	public static event Action<string> didFinishMoreAppsEvent;
	
	// Fired when the more apps screen is cached
	public static event Action didCacheMoreAppsEvent;

	


	static ChartboostManager()
	{
		AbstractManager.initialize( typeof( ChartboostManager ) );
	}


	public void didFailToLoadInterstitial( string location )
	{
		if( didFailToCacheInterstitialEvent != null )
			didFailToCacheInterstitialEvent( location );
	}


	public void didDismissInterstitial( string location )
	{
		if( didFinishInterstitialEvent != null )
			didFinishInterstitialEvent( "dismiss" );
	}
	
	
	public void didClickInterstitial( string location )
	{
		if( didFinishInterstitialEvent != null )
			didFinishInterstitialEvent( "click" );
	}

	
	public void didCloseInterstitial( string location )
	{
		if( didFinishInterstitialEvent != null )
			didFinishInterstitialEvent( "close" );
	}


	public void didFailToLoadMoreApps( string empty )
	{
		if( didFailToCacheMoreAppsEvent != null )
			didFailToCacheMoreAppsEvent();
	}


	public void didFinishMoreApps( string param )
	{
		if( didFinishMoreAppsEvent != null )
			didFinishMoreAppsEvent( param );
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

}
#endif
