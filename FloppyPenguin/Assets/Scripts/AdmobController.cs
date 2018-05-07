using UnityEngine;
using System.Collections;

public class AdmobController : Singleton<AdmobController> {
	//replace with your id
	private const string MY_AD_UNIT_ID = "a152fcb787efd53"; 
	
	private GoogleMobileAdBanner banner;
	
	private bool IsInterstisialsAdReady = false;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	public static int deathCount = 0;

	public void Awake() {

		AndroidAdMobController.instance.Init(MY_AD_UNIT_ID);
		
		//See also AddKeyword and AddTestDevice functions
		AndroidAdMobController.instance.SetGender(GoogleGenger.Unknown);
		
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_LOADED, OnInterstisialsLoaded);
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_OPENED, OnInterstisialsOpen);

		DontDestroyOnLoad(this);

		LoadInterstitial();
	}

	public void Init(){

	}

	public void CreateBanner(){
		if (banner != null)
			return;

		banner = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.SMART_BANNER);
	}

	public void HideBanner(){
		if (banner == null)
			return;
		banner.Hide ();
	}

	public void ShowBanner(){
		if (banner == null)
			return;
		banner.Show();
	}

	public void LoadInterstitial(){
		if (!IsInterstisialsAdReady)
			AndroidAdMobController.instance.LoadInterstitialAd ();
	}

	public void ShowInterstitial(){

		AndroidAdMobController.instance.ShowInterstitialAd();
	}

	public void CheckToShowInterstitial(){
		deathCount ++;
		if (deathCount >= 6){
			ShowInterstitial();
		}
	}


	

	
	private void OnInterstisialsLoaded() {
		IsInterstisialsAdReady = true;
	}
	
	private void OnInterstisialsOpen() {
		IsInterstisialsAdReady = false;
		deathCount = 0;
	}
	

}
