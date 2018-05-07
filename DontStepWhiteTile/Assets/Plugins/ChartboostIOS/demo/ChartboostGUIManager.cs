using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class ChartboostGUIManager : MonoBehaviourGUI
{
#if UNITY_IPHONE
	void OnGUI()
	{
		beginColumn();


		if( GUILayout.Button( "Init" ) )
		{
			ChartboostBinding.init( "YOUR_APP_ID", "YOUR_APP_SIGNATURE" );
		}


		if( GUILayout.Button( "Cache Interstitial" ) )
		{
			ChartboostBinding.cacheInterstitial( "default" );
		}


		if( GUILayout.Button( "Is Interstitial Cached?" ) )
		{
			Debug.Log( "is cached: " + ChartboostBinding.hasCachedInterstitial( "default" ) );
		}


		if( GUILayout.Button( "Show Interstitial" ) )
		{
			ChartboostBinding.showInterstitial( "default" );
		}


		if( GUILayout.Button( "Cache More Apps" ) )
		{
			ChartboostBinding.cacheMoreApps();
		}


		if( GUILayout.Button( "Show More Apps" ) )
		{
			ChartboostBinding.showMoreApps();
		}


		endColumn( true );


		if( GUILayout.Button( "Track Event" ) )
		{
			ChartboostBinding.trackEvent( "some_event" );
		}


		if( GUILayout.Button( "Track Event with Metadata" ) )
		{
			var dict = new Dictionary<string,string>();
			dict.Add( "key", "theValue" );
			ChartboostBinding.trackEventWithMetadata( "some_event_with_data", dict );
		}


		if( GUILayout.Button( "Track Event with Value" ) )
		{
			ChartboostBinding.trackEventWithValue( "event_with_value", 123 );
		}


		if( GUILayout.Button( "Track Event with Value and Metadata" ) )
		{
			var dict = new Dictionary<string,object>();
			dict.Add( "key", "theValue" );
			ChartboostBinding.trackEventWithValueAndMetadata( "event_with_value_and_data", 9809823, dict );
		}

		endColumn( false );
	}
#endif
}
