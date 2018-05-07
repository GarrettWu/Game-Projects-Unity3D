////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class AndroidNative {

	//--------------------------------------
	// Constants
	//--------------------------------------

	public const string DATA_SPLITTER = "|";
	public const string DATA_EOF = "endofline";
	
	//--------------------------------------
	// Goole Cloud
	//--------------------------------------

	public static void listStates() {
		CallActivityFunction("listStates");
	}

	
	public static void updateState(int stateKey, string data) {
		CallActivityFunction("updateState", stateKey.ToString(), data);
	}

	public static void resolveState(int stateKey, string resolvedData, string resolvedVersion) {
		CallActivityFunction("resolveState", stateKey.ToString(), resolvedData, resolvedVersion);
	}

	public static void deleteState(int stateKey)  {
		CallActivityFunction("deleteState", stateKey.ToString());
	}

	public static void loadState(int stateKey)  {
		CallActivityFunction("loadState", stateKey.ToString());
	}
	

	// --------------------------------------
	// Google Cloud Message
	// --------------------------------------
	
	public static void GCMRgisterDevice(string senderId) {
		CallActivityFunction("GCMRgisterDevice", senderId);
	}

	public static void GCMLoadLastMessage() {
		CallActivityFunction("GCMLoadLastMessage");
	}

	//--------------------------------------
	// Billing
	//--------------------------------------

	
	public static void connectToBilling(string ids, string base64EncodedPublicKey) {
		CallActivityFunction("connectToBilling", ids, base64EncodedPublicKey);
	}
	

	public static void retrieveProducDetails() {
		CallActivityFunction("retrieveProducDetails");
	}


	public static void consume(string SKU) {
		CallActivityFunction("consume", SKU);
	}



	public static void purchase(string SKU, string developerPayload) {
		CallActivityFunction("purchase", SKU, developerPayload);
	}
	

	public static void subscribe(string SKU, string developerPayload) {
		CallActivityFunction("subscribe", SKU, developerPayload);
	}

	

	

	//--------------------------------------
	// Other
	//--------------------------------------

	public static void LoadContacts() {
		CallActivityFunction("loadAddressBook");
	}
	

	public static void LoadPackageInfo() {
		CallActivityFunction("LoadPackageInfo");
	}

	

	// --------------------------------------
	// Analytics
	// --------------------------------------

	public static void startAnalyticsTracking() {
		CallActivityFunction ("startAnalyticsTracking");
	}

	public static void SetTrackerID(string trackingID) {
		CallActivityFunction("SetTrackerID", trackingID);
	}

	public static void SendView() {
		CallActivityFunction("SendView");
	}

	public static void SendView(string appScreen) {
		CallActivityFunction("SendView", appScreen);
	}

	public static void SendEvent(string category, string action, string label, string value)  {
		CallActivityFunction("SendEvent", category, action, label, value);
	}

	public static void SendEvent(string category, string action, string label, string value, string key, string val)  {
		CallActivityFunction("SendEvent", category, action, label, value, key, val);
	}

	public static void SendTiming(string category, string intervalInMilliseconds, string name, string label) {
		CallActivityFunction("SendTiming", category, intervalInMilliseconds, name, label);
	}

	public static void CreateTransaction(string transactionId, string affiliation, string revenue, string tax, string shipping, string currencyCode) {
		CallActivityFunction("CreateTransaction", transactionId, affiliation, revenue, tax, shipping, currencyCode);
	}
	
	public static void CreateItem(string transactionId, string name, string sku, string category, string price, string quantity, string currencyCode) {
		CallActivityFunction("CreateItem", transactionId, name, sku, category, price, quantity, currencyCode);
	}



	public static void SetKey(string key, string value) {
		CallActivityFunction("SetKey", key, value);
	}

	public static void ClearKey(string key) {
		CallActivityFunction("ClearKey", key);
	}

	public static void SetLogLevel(int lvl) {
		CallActivityFunction("SetLogLevel", lvl.ToString());
	}


	public static void SetDryRun(string mode) {
		CallActivityFunction("SetDryRun", mode);
	}

	public static void EnableAdvertisingIdCollection(string mode) {
		CallActivityFunction("enableAdvertisingIdCollection", mode);
	}
	


	// --------------------------------------
	// Social
	// --------------------------------------


	public static void StartShareIntent(string caption, string message,  string subject, string filters) {
		CallActivityFunction("StartShareIntent", caption, message, subject, filters);
	}

	public static void StartShareIntent(string caption, string message, string subject, string media, string filters) {
		CallActivityFunction("StartShareIntentMedia", caption, message, subject, media, filters);
	}

	public static void SendMailWithImage(string caption, string message,  string subject, string email, string media) {
		CallActivityFunction("SendMailWithImage", caption, message, subject, email, media);
	}
	

	public static void SendMail(string caption, string message,  string subject, string email) {
		CallActivityFunction("SendMail", caption, message, subject, email);
	}

	// --------------------------------------
	// Twitter
	// --------------------------------------

	public static void TwitterInit(string consumer_key, string consumer_secret) {
		CallActivityFunction("TwitterInit", consumer_key, consumer_secret);
	}
	
	public static void AuthificateUser() {
		CallActivityFunction("AuthificateUser");
	}

	public static void LoadUserData() {
		CallActivityFunction("LoadUserData");
	}

	public static void TwitterPost(string status) {
		CallActivityFunction("TwitterPost", status);
	}

	public static void TwitterPostWithImage(string status, string data) {
		CallActivityFunction("TwitterPostWithImage", status, data);
	}

	public static void LogoutFromTwitter() {
		CallActivityFunction("LogoutFromTwitter");
	}

	// --------------------------------------
	// Instagram
	// --------------------------------------



	public static void InstagramPostImage(string data, string cpation) {
		CallActivityFunction("InstagramPostImage", data, cpation);
	}


	// --------------------------------------
	// Utils
	// --------------------------------------
	
	public static void isPackageInstalled(string packagename) {
		CallActivityFunction("isPackageInstalled", packagename);
	}

	public static void runPackage(string packagename) {
		CallActivityFunction("runPackage", packagename);
	}


	
	public static void revokeAccessAndDisconnect() {
		CallActivityFunction("revokeAccessAndDisconnect");
	}
	

	public static void ShowToastNotification(string text, int duration) {
		CallActivityFunction("ShowToastNotification", text, duration.ToString());
	}

	public static void requestCurrentAppLaunchNotificationId() { 
		CallActivityFunction("requestCurrentAppLaunchNotificationId");
	}


	public static void ScheduleLocalNotification(string title, string message, int seconds, int id) {
		CallActivityFunction("ScheduleLocalNotification", title, message, seconds.ToString(), id.ToString());
	}

	public static void CanselLocalNotification(int id) {
		CallActivityFunction("canselLocalNotification", id.ToString());
	}


	public static void InitCameraAPI(string folderName, int maxSize, int mode) {
		CallActivityFunction("InitCameraAPI", folderName, maxSize.ToString(), mode.ToString());
	}

	public static void SaveToGalalry(string ImageData, string name) {
		CallActivityFunction("SaveToGalalry", ImageData, name);
	}


	public static void GetImageFromGallery() {
		Debug.Log("GetImageFromGallery");
		CallActivityFunction("GetImageFromGallery");
	}
	

	
	public static void GetImageFromCamera() {
		Debug.Log("GetImageFromCamera");
		CallActivityFunction("GetImageFromCamera");
	}


	private static void CallActivityFunction(string methodName, params object[] args) {
       #if UNITY_ANDROID

		if(Application.platform != RuntimePlatform.Android) {
			return;
		}

		try {

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 

			jo.Call("runOnUiThread", new AndroidJavaRunnable(() => { jo.Call(methodName, args); }));


		} catch(System.Exception ex) {
			Debug.LogWarning(ex.Message);
		}
		#endif
	}

}
