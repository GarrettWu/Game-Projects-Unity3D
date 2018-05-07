using UnityEngine;
using System.Collections;

public class PluginManager : Singleton<PluginManager> {

	public bool isQuiting = false;

	//IDs
	private const string MY_BANNERS_AD_UNIT_ID		 = "ca-app-pub-2252922171315937/1890442006"; 
	private const string MY_INTERSTISIALS_AD_UNIT_ID = "ca-app-pub-2252922171315937/3367175204"; 
	private const string MY_GOOGLE_ANALYTICS_ID = "UA-57673931-1";

	private GoogleMobileAdBanner banner;
	private bool IsAdmobInterstitialReady = false;
	private int tryInterstitialCount = 4;

	public void Awake(){
		AdmobInit();
		GAInit();
	}

	public void CacheInterstitials(){
		AdmobLoadInterstitial();
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
		AdmobShowInterstitial();
	}

	private void ShowInterstitialOnQuit(){
		isQuiting = true;


		if (IsAdmobInterstitialReady)
			AdmobShowInterstitial();
		else 
			Application.Quit();
	}
	
	private void AdmobInit(){
		AndroidAdMobController.instance.Init(MY_BANNERS_AD_UNIT_ID);

		AndroidAdMobController.instance.SetInterstisialsUnitID(MY_INTERSTISIALS_AD_UNIT_ID);

		
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

	private void GAInit(){
		AndroidGoogleAnalytics.instance.StartTracking();
		AndroidGoogleAnalytics.instance.SetTrackerID(MY_GOOGLE_ANALYTICS_ID);
		AndroidGoogleAnalytics.instance.SendView("Home Screen");
	}
}
