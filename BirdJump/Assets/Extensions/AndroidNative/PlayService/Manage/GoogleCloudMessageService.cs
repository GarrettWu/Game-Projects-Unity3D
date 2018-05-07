////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GoogleCloudMessageService : Singletone<GoogleCloudMessageService> {
	
	public const string CLOUD_MESSAGE_SERVICE_REGISTRATION_FAILED = "cloud_message_service_registration_failed";
	public const string CLOUD_MESSAGE_SERVICE_REGISTRATION_RECIVED = "cloud_message_service_registration_recived";
	
	private string _registrationId = string.Empty;
	
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	
	public void init(string senderId) {
		AndroidNative.initCloudMessage(senderId);
	}
	
	
	
	//--------------------------------------
	// GET / SET
	//--------------------------------------
	
	public string registrationId {
		get {
			return _registrationId;
		}
	}
	
	
	//--------------------------------------
	// EVENTS
	//--------------------------------------
	
	private void OnRegistrationReviced(string regId) {
		_registrationId = regId;
		dispatch(CLOUD_MESSAGE_SERVICE_REGISTRATION_RECIVED, regId);
	}
	
	private void OnRegistrationFailed() {
		dispatch(CLOUD_MESSAGE_SERVICE_REGISTRATION_FAILED);
	}
	
	
	
}
