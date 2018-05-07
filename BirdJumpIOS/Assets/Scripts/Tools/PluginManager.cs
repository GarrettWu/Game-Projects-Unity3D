using System;
using UnityEngine;
using UnionAssets.FLE;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds;
using GoogleMobileAds.Api;


public class PluginManager : Singleton<PluginManager> {

	
	private string leaderBoardId = "JumpyBirdieHighScores";
	private string bannerId = "ca-app-pub-2252922171315937/1796575600";
	private string interstitialId = "ca-app-pub-2252922171315937/3273308805";

	private BannerView bannerView = null;
	private InterstitialAd interstitial = null;	

	private int tryInterstitialCount = 4;

	void Awake() {
		GameCenterManager.init();

		AdmobInit();

	}

	public void ShowLeaderBoard(){
		GameCenterManager.showLeaderBoard(leaderBoardId);
	}

	public void ReportScore(int score){
		GameCenterManager.reportScore(score, leaderBoardId);
	}

	private void AdmobInit(){
		CreateBanner();
		CreateInterstitial();

	}

	public void CreateBanner()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "INSERT_ANDROID_BANNER_AD_UNIT_ID_HERE";
		#elif UNITY_IPHONE
		string adUnitId = bannerId;
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

		bannerView.AdFailedToLoad += HandleAdFailedToLoad;
		bannerView.AdLoaded += HandleAdLoaded;
		// Load a banner ad.
		bannerView.LoadAd(createAdRequest());
	}



	public void ShowBanner(){
		if (bannerView != null)
			bannerView.Show ();
		else
			CreateBanner();
	}
	
	public void HideBanner(){
		if (bannerView != null)
			bannerView.Hide();
	}

	public void CreateInterstitial()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "INSERT_ANDROID_INTERSTITIAL_AD_UNIT_ID_HERE";
		#elif UNITY_IPHONE
		string adUnitId = interstitialId;
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create an interstitial.
		interstitial = new InterstitialAd(adUnitId);

		interstitial.AdFailedToLoad += HandleInterstitialFailedToLoad;
		interstitial.AdClosed += HandleAdClosed;

		// Load an interstitial ad.
		interstitial.LoadAd(createAdRequest());
	}



	public AdRequest createAdRequest()
	{
		return new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator)
				.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
				.AddKeyword("game")
				.SetGender(Gender.Male)
				.SetBirthday(new DateTime(1985, 1, 1))
				.TagForChildDirectedTreatment(false)
				.AddExtra("color_bg", "9B30FF")
				.Build();
		
	}
	
	public void ShowInterstitial()
	{
		if (interstitial != null && interstitial.IsLoaded())
		{
			interstitial.Show();
		} else {
			CreateInterstitial();
		}
	}

	public void TryInterstitial(){
		tryInterstitialCount--;
		if (tryInterstitialCount <= 0){
			ShowInterstitial();
			tryInterstitialCount = 4;
		}
	}

	public void HandleAdLoaded (object sender, EventArgs e)
	{
		bannerView.Hide();
	}

	public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		bannerView.LoadAd(createAdRequest());
	}

	public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		interstitial.LoadAd(createAdRequest());
	}

	public void HandleAdClosed (object sender, EventArgs e)
	{
		CreateInterstitial();
	}
	
}
