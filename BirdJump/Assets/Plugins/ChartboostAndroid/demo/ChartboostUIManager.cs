using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class ChartboostUIManager : MonoBehaviourGUI
{
#if UNITY_ANDROID
	void OnGUI()
	{
		beginColumn();

		if( GUILayout.Button( "Init" ) )
		{
			ChartboostAndroid.init( "YOUR_APP_ID", "YOUR_APP_SIGNATURE" );
		}


		if( GUILayout.Button( "Cache Interstitial" ) )
		{
			ChartboostAndroid.cacheInterstitial( null );
		}


		if( GUILayout.Button( "Check for Cached Interstitial" ) )
		{
			Debug.Log( "has cached interstitial: " + ChartboostAndroid.hasCachedInterstitial( null ) );
		}


		if( GUILayout.Button( "Show Interstitial" ) )
		{
			ChartboostAndroid.showInterstitial( null );
		}


		if( GUILayout.Button( "Cache More Apps" ) )
		{
			ChartboostAndroid.cacheMoreApps();
		}


		if( GUILayout.Button( "Has Cached More Apps" ) )
		{
			Debug.Log( "has cached more apps: " + ChartboostAndroid.hasCachedMoreApps() );
		}


		if( GUILayout.Button( "Show More Apps" ) )
		{
			ChartboostAndroid.showMoreApps();
		}


		if( GUILayout.Button( "Track Event" ) )
		{
			ChartboostAndroid.trackEvent( "event-did-something", 0, null );
		}

		endColumn();
	}
#endif
}
