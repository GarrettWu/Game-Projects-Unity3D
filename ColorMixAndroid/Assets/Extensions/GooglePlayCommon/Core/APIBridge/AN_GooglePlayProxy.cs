using UnityEngine;
using System.Collections;

public class AN_GooglePlayProxy  {

	private const string CLASS_NAME = "com.androidnative.gms.core.GameClientManager";
	
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}

	
	public static void ShowSavedGamesUI_Bridge(string title, int maxNumberOfSavedGamesToShow) {
		CallActivityFunction("ShowSavedGamesUI_Bridge", title, maxNumberOfSavedGamesToShow);
	}

	public static void CreateNewSpanshot_Bridge(string name, string description, string ImageData, string Data, long PlayedTime) {
		CallActivityFunction("CreateNewSpanshot_Bridge", name, description, ImageData, Data, PlayedTime);
	}

	public static void ResolveSnapshotsConflict_Bridge(int index) {
		CallActivityFunction("ResolveSnapshotsConflict_Bridge", index);
	}

	public static void LoadSpanshots_Bridge() {
		CallActivityFunction("LoadSpanshots_Bridge");
	}

	public static void Test() {
		CallActivityFunction("Test");
	}

	public static void OpenSpanshotByName_Bridge(string name) {
		CallActivityFunction("OpenSpanshotByName_Bridge", name);
	}

}
