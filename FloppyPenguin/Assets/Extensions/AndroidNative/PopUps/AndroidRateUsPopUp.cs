////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AndroidRateUsPopUp : BaseAndroidPopup {
	


	public string yes;
	public string laiter;
	public string no;
	public string url;

	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public static AndroidRateUsPopUp Create(string title, string message, string url) {
		return Create(title, message, url, "Rate app", "Later", "No, thanks");
	}
	
	public static AndroidRateUsPopUp Create(string title, string message, string url, string yes, string laiter, string no) {
		AndroidRateUsPopUp rate = new GameObject("AndroidRateUsPopUp").AddComponent<AndroidRateUsPopUp>();
		rate.title = title;
		rate.message = message;
		rate.url = url;

		rate.yes = yes;
		rate.laiter = laiter;
		rate.no = no;

		rate.init();
			
		return rate;
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	public void init() {
		AndroidNative.showRateDialog(title, message, yes, laiter, no, url);
	}
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	public void onPopUpCallBack(string buttonIndex) {
		int index = System.Convert.ToInt16(buttonIndex);
		switch(index) {
			case 0: 
				dispatch(BaseEvent.COMPLETE, AndroidDialogResult.RATED);
				break;
			case 1:
				dispatch(BaseEvent.COMPLETE, AndroidDialogResult.REMIND);
				break;
			case 2:
				dispatch(BaseEvent.COMPLETE, AndroidDialogResult.DECLINED);
				break;
		}
		
		
		
		Destroy(gameObject);
	} 
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
