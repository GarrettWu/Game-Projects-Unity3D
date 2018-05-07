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

public class AndroidPopUpExamples : MonoBehaviour {


	private string rateText = "If you enjoy using Google Earth, please take a moment to rate it. Thanks for your support!";


	//example link to your app on android market
	//private string rateUrl = "market://details?id=com.example.android";

	private string rateUrl = "LINK_TO_YOUR_APP";



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	void OnGUI() {
		if(GUI.Button(new Rect(10, 10, 150, 50), "Rate Dialog PopUp")) {
			AndroidRateUsPopUp rate = AndroidRateUsPopUp.Create("Rate Us", rateText, rateUrl);
			rate.addEventListener(BaseEvent.COMPLETE, OnRatePopUpClose);
		}
		
		
		if(GUI.Button(new Rect(10, 70, 150, 50), "Dialog PopUp")) {
			AndroidDialog dialog = AndroidDialog.Create("Dialog Titile", "Dialog message");
			dialog.addEventListener(BaseEvent.COMPLETE, OnDialogClose);
		}
		
		if(GUI.Button(new Rect(10, 130, 150, 50), "Message PopUp")) {
			AndroidMessage msg = AndroidMessage.Create("Message Titile", "Message message");
			msg.addEventListener(BaseEvent.COMPLETE, OnMessageClose);
		}
		
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private void OnRatePopUpClose(CEvent e) {
		(e.dispatcher as AndroidRateUsPopUp).removeEventListener(BaseEvent.COMPLETE, OnRatePopUpClose);
		string result = e.data.ToString();
		AndroidNative.showMessage("Result", result + " button pressed");
	}



	private void OnDialogClose(CEvent e) {

		//removing listner
		(e.dispatcher as AndroidDialog).removeEventListener(BaseEvent.COMPLETE, OnDialogClose);

		//parsing result
		switch((AndroidDialogResult)e.data) {
		case AndroidDialogResult.YES:
			Debug.Log ("Yes button pressed");
			break;
		case AndroidDialogResult.NO:
			Debug.Log ("Yes button pressed");
			break;

		}
			
		AndroidNative.showMessage("Result", e.data.ToString() + " button pressed");
	}



	private void OnMessageClose(CEvent e) {
		(e.dispatcher as AndroidMessage).removeEventListener(BaseEvent.COMPLETE,  OnMessageClose);
		AndroidNative.showMessage("Result", "Message Closed");
	}
	

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
