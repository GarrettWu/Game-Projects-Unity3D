using UnityEngine;
using System.Collections;

public class PluginManager : Singleton<PluginManager> {

	public bool isQuiting = false;

	//admob
	private const string MY_BANNERS_AD_UNIT_ID		 = "a1533527e686561"; 
	private const string MY_INTERSTISIALS_AD_UNIT_ID = "a1533527e686561";
	private GoogleMobileAdBanner banner;
	private bool IsAdmobInterstitialReady = false;

	public void Awake(){
		AdmobInit();
		CBInit();
		NativeXInit();
		GAInit();
	}

	public void CacheInterstitials(){
		AdmobLoadInterstitial();
		CBCacheInterstitial();
	}

	public void ShowBanner(){
		AdmobShowBanner();
	}

	public void ShowInterstitialInGame(){
		if (CBIsInterstitialCached())
			CBShowInterstitial();
		else
			AdmobShowInterstitial();
	}

	public void ShowInterstitialOnQuit(){
		isQuiting = true;

		if (CBIsInterstitialCached())
			CBShowInterstitial();
		else if (IsAdmobInterstitialReady)
			AdmobShowInterstitial();
		else 
			Application.Quit();
	}

	public void AdmobInit(){
		AndroidAdMobController.instance.Init(MY_BANNERS_AD_UNIT_ID);
		
		//I whant to use Interstisial ad also, so I have to set additional id for it
		AndroidAdMobController.instance.SetInterstisialsUnitID(MY_INTERSTISIALS_AD_UNIT_ID);
		
		
		//See also AddKeyword and AddTestDevice functions
		AndroidAdMobController.instance.SetGender(GoogleGenger.Unknown);
		
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_LOADED, AdmobOnInterstisialsLoaded);
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_OPENED, AdmobOnInterstisialsOpen);
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_CLOSED, AdmobOnInterstisialsClosed);

	}

	public void AdmobShowBanner(){
		if (banner == null)
			banner = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.SMART_BANNER);
	}

	public void AdmobLoadInterstitial(){
		if (!IsAdmobInterstitialReady)
			AndroidAdMobController.instance.LoadInterstitialAd ();
	}
	
	public void AdmobShowInterstitial(){
		
		AndroidAdMobController.instance.ShowInterstitialAd();
	}

	private void AdmobOnInterstisialsLoaded() {
		IsAdmobInterstitialReady = true;
	}
	
	private void AdmobOnInterstisialsOpen() {
		IsAdmobInterstitialReady = false;
	}

	private void AdmobOnInterstisialsClosed() {
		if (isQuiting){
			Application.Quit();
		}
	}

	public void CBInit(){
		Chartboost.init( "50e7ef8716ba478429000022", "5ec09f1f64212f8945cdc6653c193ce0519dedf2", "IOS_APP_ID", "IOS_APP_SIGNATURE" );
	}

	public void CBCacheInterstitial(){
		if (CBIsInterstitialCached())
			return;
		
		Chartboost.cacheInterstitial( "default" );
	}
	
	public bool CBIsInterstitialCached()
	{
		return Chartboost.hasCachedInterstitial( "default" );
	}
	
	public void CBShowInterstitial () {
		Chartboost.showInterstitial( "default" );
	}
	
	void OnEnable()
	{
		Chartboost.didFinishInterstitialEvent += CBdidFinishInterstitialEvent;
	}
	
	
	void OnDisable()
	{
		Chartboost.didFinishInterstitialEvent += CBdidFinishInterstitialEvent;
	}
	
	void CBdidFinishInterstitialEvent( string reason )
	{
		if (isQuiting){
			Application.Quit();
		}
	}

	public void NativeXInit(){

	}

	public void GAInit(){
		AndroidGoogleAnalytics.instance.StartTracking();

		AndroidGoogleAnalytics.instance.SetTrackerID("UA-49515262-1");
	}
}
