////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////
/// 
using UnityEngine;
using System.Collections;

public class AndroidGoogleAnalytics : SA_Singleton<AndroidGoogleAnalytics> {


	private bool IsStarted = false;



	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void StartTracking() {
		if(IsStarted) {
			return;
		}

		IsStarted = true;
		AndroidNative.startAnalyticsTracking();
	}


	public void SetTrackerID(string trackingID)  {
		AndroidNative.SetTrackerID(trackingID);
	}

	public void SendView(string appScreen) {
		AndroidNative.SendView(appScreen);
	}
	
	public void SendView() {
		AndroidNative.SendView();
	}


	public void SendEvent(string category, string action, string label) {
		AndroidNative.SendEvent(category, action, label, "null");
	}

	public void SendEvent(string category, string action, string label, long value) {
		AndroidNative.SendEvent(category, action, label, value.ToString());
	}

	public void SendEvent(string category, string action, string label, string key, string val) {
		AndroidNative.SendEvent(category, action, label, "null", key, val);
	}

	public void SendEvent(string category, string action, string label, long value, string key, string val) {
		AndroidNative.SendEvent(category, action, label, value.ToString(), key, val);
	}


	public void SendTiming(string category, long intervalInMilliseconds) {
		AndroidNative.SendTiming(category, intervalInMilliseconds.ToString(), "null", "null");
	}

	public void SendTiming(string category, long intervalInMilliseconds, string name) {
		AndroidNative.SendTiming(category, intervalInMilliseconds.ToString(), name, "null");
	}
	

	public void SendTiming(string category, long intervalInMilliseconds, string name, string label) {
		AndroidNative.SendTiming(category, intervalInMilliseconds.ToString(), name, label);
	}


	public void CreateTransaction(string transactionId, string affiliation, float revenue, float tax, float shipping, string currencyCode) {
		AndroidNative.CreateTransaction(transactionId, affiliation, revenue.ToString(), tax.ToString(), shipping.ToString(), currencyCode);
	}
	
	public void CreateItem(string transactionId, string name, string sku, string category, float price, int quantity, string currencyCode) {
		AndroidNative.CreateItem(transactionId, name, sku, category, price.ToString(), quantity.ToString(), currencyCode);
	}


	public void SetKey(string key, string value) {
		AndroidNative.SetKey(key, value);
	}


	public  void ClearKey(string key) {
		AndroidNative.ClearKey(key);
	}

	public void SetLogLevel(GPLogLevel logLevel) {
		AndroidNative.SetLogLevel((int) logLevel);
	}

	public void SetDryRun(bool mode) {
		if(mode) {
			AndroidNative.SetDryRun("true");
		} else {
			AndroidNative.SetDryRun("false");
		}
	}

	public void EnableAdvertisingIdCollection(bool mode) {
		if(mode) {
			AndroidNative.EnableAdvertisingIdCollection("true");
		} else {
			AndroidNative.EnableAdvertisingIdCollection("false");
		}
	}

	
	
	



		
}
