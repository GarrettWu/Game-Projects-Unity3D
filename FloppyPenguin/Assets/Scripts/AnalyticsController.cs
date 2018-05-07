using UnityEngine;
using System.Collections;

public class AnalyticsController : Singleton<AnalyticsController> {


	public void Awake () {

		//Staring Analytics
		//It will be started with ID spesifayed in xml
		AndroidGoogleAnalytics.instance.StartTracking();
		
		//If you want ot change default tracking id use this function after StartTracking:
		AndroidGoogleAnalytics.instance.SetTrackerID("UA-49021875-1");

	}

	public void Init(){

	}
	
	
//	void Start() {
//		//Tracking firest screen
//		AndroidGoogleAnalytics.instance.SendView("Home Screen");
//		
//		//Send event example + 1 more implementation
//		AndroidGoogleAnalytics.instance.SendEvent("Category", "Action", "label");
//		//Send event example with addition values + 1 more implementation
//		AndroidGoogleAnalytics.instance.SendEvent("Category", "Action", "label", 100, "screen", "main");
//		
//		//Send timing event + 2 more implementation 
//		AndroidGoogleAnalytics.instance.SendTiming("App Started", (long) Time.time);
//		
//		//Set session key
//		AndroidGoogleAnalytics.instance.SetKey("SCREEN", "MAIN");
//		
//		
//		//To remove session key use
//		//AndroidGoogleAnalytics.instance.ClearKey("SCREEN");
//		
//		
//		//To Chnage login level use
//		//AndroidGoogleAnalytics.instance.SetLogLevel(AndroidLogLevel.VERBOSE);
//		
//		//To disable data sending use
//		//AndroidGoogleAnalytics.instance.SetDryRun(true);
//		
//		
//		
//		
//		PurchaseTackingExample();
//	}
	
	
}
