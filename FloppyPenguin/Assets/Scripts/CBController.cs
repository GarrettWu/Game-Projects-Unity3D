using UnityEngine;
using System.Collections;

public class CBController : Singleton<CBController> {

	// Use this for initialization
	public void Awake () {
		Chartboost.init( "5329739ef8975c5d965c9fe0", "cc753ae734affc66f3dc7cd3e19eb3bff9cc6241", "IOS_APP_ID", "IOS_APP_SIGNATURE" );
	}

	public void Init(){

	}
	
	public void CacheInterstitial(){
		if (isInterstitialCached())
			return;

		Chartboost.cacheInterstitial( "default" );
	}

	public bool isInterstitialCached()
	{
		return Chartboost.hasCachedInterstitial( "default" );
	}
	// Update is called once per frame
	public void ShowInterstisial () {
		Chartboost.showInterstitial( "default" );
	}

	void OnEnable()
	{
		Chartboost.didFinishInterstitialEvent += didFinishInterstitialEvent;
	}
	
	
	void OnDisable()
	{
		Chartboost.didFinishInterstitialEvent += didFinishInterstitialEvent;
	}

	void didFinishInterstitialEvent( string reason )
	{
		Application.Quit();
	}

}
