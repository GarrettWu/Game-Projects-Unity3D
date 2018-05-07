////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AndroidDialog : BaseAndroidPopup {
	

	public string yes;
	public string no;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static AndroidDialog Create(string title, string message) {
		return Create(title, message, "Yes", "No");
	}
		
	public static AndroidDialog Create(string title, string message, string yes, string no) {
		AndroidDialog dialog;
		dialog  = new GameObject("AndroidPopUp").AddComponent<AndroidDialog>();
		dialog.title = title;
		dialog.message = message;
		dialog.yes = yes;
		dialog.no = no;
		dialog.init();
		
		return dialog;
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public void init() {
		AndroidNative.showDialog(title, message, yes, no);
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
				dispatch(BaseEvent.COMPLETE, AndroidDialogResult.YES);
				break;
			case 1: 
				dispatch(BaseEvent.COMPLETE, AndroidDialogResult.NO);
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
