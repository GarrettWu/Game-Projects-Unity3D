using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface GoogleMobileAdInterface  {

	void Init(string ad_unit_id);
	void ChangeBannersUnitID(string ad_unit_id);
	void ChangeInterstisialsUnitID(string ad_unit_id);

	void AddKeyword(string keyword);
	void AddTestDevice(string deviceId);
	void SetGender(GoogleGenger gender);

	GoogleMobileAdBanner CreateAdBanner(TextAnchor anchor, GADBannerSize size);
	GoogleMobileAdBanner CreateAdBanner(int x, int y, GADBannerSize size);


	void DestroyBanner(int id);


	void StartInterstitialAd();
	void LoadInterstitialAd();
	void ShowInterstitialAd();


	GoogleMobileAdBanner GetBanner(int id);
	List<GoogleMobileAdBanner> banners {get;}


	void addEventListener(string eventName, EventHandlerFunction handler);
	void addEventListener(string eventName, DataEventHandlerFunction handler);
}
