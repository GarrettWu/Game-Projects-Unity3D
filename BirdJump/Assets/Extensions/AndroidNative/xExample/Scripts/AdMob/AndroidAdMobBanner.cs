////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//Attach the script to the empty gameobject on your sceneS
public class AndroidAdMobBanner : MonoBehaviour {


	public string BannersUnityId;
	public GADBannerSize size = GADBannerSize.SMART_BANNER;
	public TextAnchor anchor = TextAnchor.LowerCenter;


	private static Dictionary<string, GoogleMobileAdBanner> _refisterdBanners = null;


	// --------------------------------------
	// Unity Events
	// --------------------------------------
	
	void Awake() {
		if(AndroidAdMobController.instance.IsInited) {
			if(!AndroidAdMobController.instance.BannersUunitId.Equals(BannersUnityId)) {
				AndroidAdMobController.instance.SetBannersUnitID(BannersUnityId);
			} 
		} else {
			AndroidAdMobController.instance.Init(BannersUnityId);
		}
	}

	void Start() {
		ShowBanner();
	}

	void OnDestroy() {
		HideBanner();
	}


	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------

	public void ShowBanner() {
		GoogleMobileAdBanner banner;
		if(refisterdBanners.ContainsKey(sceneBannerId)) {
			banner = refisterdBanners[sceneBannerId];
		}  else {
			banner = AndroidAdMobController.instance.CreateAdBanner(anchor, size);
			refisterdBanners.Add(sceneBannerId, banner);
		}

		if(banner.IsLoaded && !banner.IsOnScreen) {
			banner.Show();
		}
	}

	public void HideBanner() {
		if(refisterdBanners.ContainsKey(sceneBannerId)) {
			GoogleMobileAdBanner banner = refisterdBanners[sceneBannerId];
			if(banner.IsLoaded) {
				if(banner.IsOnScreen) {
					banner.Hide();
				}
			} else {
				banner.ShowOnLoad = false;
			}
		}
	}

	// --------------------------------------
	// GET / SET
	// --------------------------------------


	public static Dictionary<string, GoogleMobileAdBanner> refisterdBanners {
		get {
			if(_refisterdBanners == null) {
				_refisterdBanners = new Dictionary<string, GoogleMobileAdBanner>();
			}

			return _refisterdBanners;
		}
	}

	public string sceneBannerId {
		get {
			return Application.loadedLevelName + "_" + this.gameObject.name;
		}
	}

	
}
