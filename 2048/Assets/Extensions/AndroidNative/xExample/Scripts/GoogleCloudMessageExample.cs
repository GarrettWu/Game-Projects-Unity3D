////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class GoogleCloudMessageExample : MonoBehaviour {
	
	
	public const string SENDER_ID = "868698578526";
	
	public static bool IsRegistred = false;
	
	
	void Awake() {
		if(!IsRegistred) {
			IsRegistred = true;
			GoogleCloudMessageService.instance.addEventListener(GoogleCloudMessageService.CLOUD_MESSAGE_SERVICE_REGISTRATION_RECIVED, OnRegistration);
		}
		
	}
	
	void OnGUI() {
		if(GUI.Button(new Rect(330, 130, 150, 50), "Init Cloud Message")) {
			GoogleCloudMessageService.instance.init(SENDER_ID);
		}
	}
	
	
	private void OnRegistration(CEvent e) {
		string regId = GoogleCloudMessageService.instance.registrationId;
		AndroidNative.showMessage("REGISTRATION RECIVED", "ID : " + regId);
	}
}
