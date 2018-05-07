////////////////////////////////////////////////////////////////////////////////
//  
// @module <module_name>
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class AndroidGoogleAdsExample : MonoBehaviour {



	
	//replace with your id
	private const string MY_AD_UNIT_ID = "a152fcb787efd53"; 


	private GUIStyle style;
	private GUIStyle style2;

	private GoogleMobileAdBanner banner1;
	private GoogleMobileAdBanner banner2;

	private bool IsInterstisialsAdReady = false;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Start() {
		AndroidAdMobController.instance.Init(MY_AD_UNIT_ID);
		
		//See also AddKeyword and AddTestDevice functions
		AndroidAdMobController.instance.SetGender(GoogleGenger.Unknown);
		
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_LOADED, OnInterstisialsLoaded);
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_OPENED, OnInterstisialsOpen);

		InitStyles();
	}


	private void InitStyles () {
		style =  new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 16;
		style.fontStyle = FontStyle.BoldAndItalic;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		
		
		style2 =  new GUIStyle();
		style2.normal.textColor = Color.white;
		style2.fontSize = 12;
		style2.fontStyle = FontStyle.Italic;
		style2.alignment = TextAnchor.UpperLeft;
		style2.wordWrap = true;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {

		float StartY = 20;
		float StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Interstisal Example", style);

		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Start Interstitial Ad")) {
			AndroidAdMobController.instance.StartInterstitialAd ();
		}

		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Load Interstitial Ad")) {
			AndroidAdMobController.instance.LoadInterstitialAd ();
		}


		StartX += 70;
		GUI.enabled = IsInterstisialsAdReady;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Show Interstitial Ad")) {
			AndroidAdMobController.instance.ShowInterstitialAd ();
		}
		GUI.enabled  = true;


		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Banners Example", style);

		GUI.enabled = false;
		if(banner1 == null) {
			GUI.enabled  = true;
		}

		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Banner Custom Pos")) {
			banner1 = AndroidAdMobController.instance.CreateAdBanner(300, 100, GADBannerSize.BANNER);
		}

		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Banner Top Left")) {
			banner1 = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.UpperLeft, GADBannerSize.BANNER);
			banner1 = AndroidAdMobController.instance.CreateAdBanner(100, 100, GADBannerSize.BANNER);
		}

		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Banner Top Center")) {
			banner1 = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.BANNER);
		}

		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Banner Top Right")) {
			banner1 = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.UpperRight, GADBannerSize.BANNER);
		}

		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Banner Bottom Left")) {
			banner1 = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.BANNER);
		}

		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Banner Bottom Center")) {
			banner1 = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.LowerCenter, GADBannerSize.BANNER);
		}

		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Banner Bottom Right")) {
			banner1 = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.LowerRight, GADBannerSize.BANNER);
		}



		GUI.enabled  = false;
		if(banner1 != null) {
			if(banner1.IsLoaded) {
				GUI.enabled  = true;
			}
		}

		StartY+= 80;
		StartX = 10;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Refresh")) {
			banner1.Refresh();
		}


		GUI.enabled  = false;
		if(banner1 != null) {
			if(banner1.IsLoaded && banner1.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Hide")) {
			banner1.Hide();
		}


		GUI.enabled  = false;
		if(banner1 != null) {
			if(banner1.IsLoaded && !banner1.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Show")) {
			banner1.Show();
		}



		GUI.enabled  = false;
		if(banner1 != null) {
			GUI.enabled  = true;
		}
		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Destroy")) {
			AndroidAdMobController.instance.DestroyBanner(banner1.id);
			banner1 = null;

		}
		GUI.enabled  = true;


		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Banner 2", style);

		GUI.enabled = false;
		if(banner2 == null) {
			GUI.enabled  = true;
		}

		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Smart Banner")) {
			banner2 = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.SMART_BANNER);
		}



		GUI.enabled  = false;
		if(banner2 != null) {
			if(banner2.IsLoaded) {
				GUI.enabled  = true;
			}
		}

		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Refresh")) {
			banner2.Refresh();
		}

		GUI.enabled  = false;
		if(banner2 != null) {
			if(banner2.IsLoaded && banner2.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		
		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Hide")) {
			banner2.Hide();
		}


		GUI.enabled  = false;
		if(banner2 != null) {
			if(banner2.IsLoaded && !banner2.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		
		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Show")) {
			banner2.Show();
		}

		GUI.enabled  = false;
		if(banner2 != null) {
			GUI.enabled  = true;
		}
		StartX += 70;
		if(GUI.Button(new Rect(StartX, StartY, 50, 50), "Destroy")) {
			AndroidAdMobController.instance.DestroyBanner(banner2.id);
			banner2 = null;
			
		}

		GUI.enabled  = true;

	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnInterstisialsLoaded() {
		IsInterstisialsAdReady = true;
	}

	private void OnInterstisialsOpen() {
		IsInterstisialsAdReady = false;
	}


	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
