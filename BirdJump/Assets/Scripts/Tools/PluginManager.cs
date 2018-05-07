using UnityEngine;
using System.Collections;

public class PluginManager : Singleton<PluginManager> {

	public bool isQuiting = false;

	//IDs
	private const string MY_BANNERS_AD_UNIT_ID = "a1536899d63f243"; 
	private const string MY_CHARTBOOST_ID = "53689a56c26ee42518c9a90a";
	private const string MY_CHARTBOOST_SIGNATURE = "9762d889bf427540512e2ce8d33aca243cf4e239";
	private const string MY_GOOGLE_ANALYTICS_ID = "UA-50708560-1";

	private GoogleMobileAdBanner banner;
	private bool IsAdmobInterstitialReady = false;
	private int tryInterstitialCount = 4;

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

	public void CreateBanner(){
		AdmobCreateBanner();
	}

	public void ShowBanner(){
		if (banner != null)
			banner.Show ();
		else
			CreateBanner();
	}

	public void HideBanner(){
		if (banner != null)
			banner.Hide();
	}

	public void TryInterstitial(){
		tryInterstitialCount--;
		if (tryInterstitialCount <= 0){
			ShowInterstitialInGame();
			tryInterstitialCount = 4;
		}
	}

	public void OnEscapeKey(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			ShowInterstitialOnQuit();
		}
	}

	private void ShowInterstitialInGame(){
		if (CBIsInterstitialCached())
			CBShowInterstitial();
		else
			AdmobShowInterstitial();
	}

	private void ShowInterstitialOnQuit(){
		isQuiting = true;

		if (CBIsInterstitialCached())
			CBShowInterstitial();
		else if (IsAdmobInterstitialReady)
			AdmobShowInterstitial();
		else 
			Application.Quit();
	}
	
	private void AdmobInit(){
		AndroidAdMobController.instance.Init(MY_BANNERS_AD_UNIT_ID);
		
		//I whant to use Interstisial ad also, so I have to set additional id for it
		AndroidAdMobController.instance.SetInterstisialsUnitID(MY_BANNERS_AD_UNIT_ID);
		
		
		//See also AddKeyword and AddTestDevice functions
		AndroidAdMobController.instance.SetGender(GoogleGenger.Unknown);
		
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_LOADED, AdmobOnInterstisialsLoaded);
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_OPENED, AdmobOnInterstisialsOpen);
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_CLOSED, AdmobOnInterstisialsClosed);

	}

	private void AdmobCreateBanner(){
		if (banner == null)
			banner = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.SMART_BANNER);
	}
	
	private void AdmobLoadInterstitial(){
		if (!IsAdmobInterstitialReady)
			AndroidAdMobController.instance.LoadInterstitialAd ();
	}
	
	private void AdmobShowInterstitial(){
		
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

	private void CBInit(){
		Chartboost.init( MY_CHARTBOOST_ID, MY_CHARTBOOST_SIGNATURE, "IOS_APP_ID", "IOS_APP_SIGNATURE" );
	}

	private void CBCacheInterstitial(){
		if (CBIsInterstitialCached())
			return;
		
		Chartboost.cacheInterstitial( "default" );
	}
	
	private bool CBIsInterstitialCached()
	{
		return Chartboost.hasCachedInterstitial( "default" );
	}
	
	private void CBShowInterstitial () {
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

	private void NativeXInit(){

	}

	private void GAInit(){
		AndroidGoogleAnalytics.instance.StartTracking();
		AndroidGoogleAnalytics.instance.SetTrackerID(MY_GOOGLE_ANALYTICS_ID);
	}
}
