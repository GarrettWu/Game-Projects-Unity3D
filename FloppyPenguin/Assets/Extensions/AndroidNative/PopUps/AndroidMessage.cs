////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AndroidMessage : BaseAndroidPopup {
	
	
	public string ok;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static AndroidMessage Create(string title, string message) {
		return Create(title, message, "Ok");
	}
		
	public static AndroidMessage Create(string title, string message, string ok) {
		AndroidMessage dialog;
		dialog  = new GameObject("AndroidPopUp").AddComponent<AndroidMessage>();
		dialog.title = title;
		dialog.message = message;
		dialog.ok = ok;
		
		dialog.init();
		
		return dialog;
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public void init() {
		AndroidNative.showMessage(title, message, ok);
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	public void onPopUpCallBack(string buttonIndex) {
		dispatch(BaseEvent.COMPLETE);
		Destroy(gameObject);	
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
