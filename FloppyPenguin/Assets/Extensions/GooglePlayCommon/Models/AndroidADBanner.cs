﻿////////////////////////////////////////////////////////////////////////////////
//  
// @module <module_name>
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class AndroidADBanner : EventDispatcherBase, GoogleMobileAdBanner {


	private int _id;
	private GADBannerSize _size;
	private TextAnchor _anchor;

	private bool _IsLoaded = false;
	private bool _IsOnScreen = false;
	private bool firstLoad = true;

	private bool _ShowOnLoad = true;



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public AndroidADBanner(TextAnchor anchor, GADBannerSize size, int id) {
		_id = id;
		_size = size;
		_anchor = anchor;
	

		AndroidNative.CreateBannerAd (gravity, (int) _size, _id);

	}

	public AndroidADBanner(int x, int y, GADBannerSize size, int id) {
		_id = id;
		_size = size;
		
		
		AndroidNative.CreateBannerAdPos (x, y, (int) _size, _id);
		
	}


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void Hide() { 
		if(!_IsOnScreen) {
			return;
		}

		_IsOnScreen = false;
		AndroidNative.HideAd (_id);
	}


	public void Show() { 

		if(_IsOnScreen) {
			return;
		}

		_IsOnScreen = true;
		AndroidNative.ShowAd (_id);
	}


	public void Refresh() { 

		if(!_IsLoaded) {
			return;
		}

		AndroidNative.RefreshAd (_id);
	}

	

	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public int id {
		get {
			return _id;
		}
	}

	public GADBannerSize size {
		get {
			return _size;
		}
	}


	public bool IsLoaded {
		get {
			return _IsLoaded;
		}
	}

	public bool IsOnScreen {
		get {
			return _IsOnScreen;
		}
	}

	public bool ShowOnLoad {
		get {
			return _ShowOnLoad;
		} 

		set {
			_ShowOnLoad = value;
		}
	}

	public TextAnchor anchor {
		get {
			return _anchor;
		}
	}


	public int gravity {
		get {
			switch(_anchor) {
			case TextAnchor.LowerCenter:
				return GoogleGravity.BOTTOM | GoogleGravity.CENTER;
			case TextAnchor.LowerLeft:
				return GoogleGravity.BOTTOM | GoogleGravity.LEFT;
			case TextAnchor.LowerRight:
				return GoogleGravity.BOTTOM | GoogleGravity.RIGHT;

			case TextAnchor.MiddleCenter:
				return GoogleGravity.CENTER;
			case TextAnchor.MiddleLeft:
				return GoogleGravity.CENTER | GoogleGravity.LEFT;
			case TextAnchor.MiddleRight:
				return GoogleGravity.CENTER | GoogleGravity.RIGHT;

			case TextAnchor.UpperCenter:
				return GoogleGravity.TOP | GoogleGravity.CENTER;
			case TextAnchor.UpperLeft:
				return GoogleGravity.TOP | GoogleGravity.LEFT;
			case TextAnchor.UpperRight:
				return GoogleGravity.TOP | GoogleGravity.RIGHT;
			}

			return GoogleGravity.TOP;
		}
	}




	//--------------------------------------
	//  EVENTS
	//--------------------------------------



	public void OnBannerAdLoaded()  {
		_IsLoaded = true;
		if(ShowOnLoad && firstLoad) {
			Show();
			firstLoad = false;
		}

		dispatch(GoogleMobileAdEvents.ON_BANNER_AD_LOADED);
	}
	
	public void OnBannerAdFailedToLoad() {
		dispatch(GoogleMobileAdEvents.ON_BANNER_AD_FAILED_LOADING);
	}
	
	public void OnBannerAdOpened() {
		dispatch(GoogleMobileAdEvents.ON_BANNER_AD_OPENED);
	}
	
	public void OnBannerAdClosed() {
		dispatch(GoogleMobileAdEvents.ON_BANNER_AD_CLOSED);
	}
	
	public void OnBannerAdLeftApplication() {
		dispatch(GoogleMobileAdEvents.ON_BANNER_AD_LEFT_APPLICATION);
	}

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
