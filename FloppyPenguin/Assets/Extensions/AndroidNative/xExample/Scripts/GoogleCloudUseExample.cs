////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GoogleCloudUseExample : MonoBehaviour {

	void Awake() {
		GoogleCloudManager.instance.addEventListener (GoogleCloudManager.ALL_STATES_LOADED, OnAllLoaded);

		GoogleCloudManager.instance.addEventListener (GoogleCloudManager.STATE_LOADED,   OnStateUpdated);
		GoogleCloudManager.instance.addEventListener (GoogleCloudManager.STATE_RESOLVED, OnStateUpdated);
		GoogleCloudManager.instance.addEventListener (GoogleCloudManager.STATE_UPDATED,  OnStateUpdated);

		GoogleCloudManager.instance.addEventListener (GoogleCloudManager.STATE_CONFLICT,  OnStateConflict);
	}

	
	void OnGUI() {
		if(GUI.Button(new Rect(490, 70, 150, 50), "Load All States")) {
			GoogleCloudManager.instance.loadAllStates ();
		}

		if(GUI.Button(new Rect(490, 130, 150, 50), "Load Sate")) {
			GoogleCloudManager.instance.loadState (GoogleCloudSlot.SLOT_0);
		}

		
		if(GUI.Button(new Rect(490, 190, 150, 50), "Update state")) {
			string msg = "Hello bytes data";
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
			byte[] data = encoding.GetBytes(msg);
			GoogleCloudManager.instance.updateState (GoogleCloudSlot.SLOT_0, data);
		}

		if(GUI.Button(new Rect(490, 250, 150, 50), "Delete state")) {
			GoogleCloudManager.instance.deleteState(GoogleCloudSlot.SLOT_0);
			GoogleCloudManager.instance.addEventListener (GoogleCloudManager.STATE_DELETED, OnStateDeleted);
		}

	}



	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnStateConflict(CEvent e) {
		GoogleCloudResult result = e.data as GoogleCloudResult;
		AndroidNative.showMessage ("OnStateUpdated", result.message 
		                           + "\n State ID: " + result.stateKey 
		                           + "\n State Data: " + result.stateData
		                           + "\n State Conflict: " + result.serverConflictData
		                           + "\n State resolve: " + result.resolvedVersion);

		//Resolving conflict with our local data
		//you should create your own resolve logic for your game. Read more about resolving conflict on Android developer website

		GoogleCloudManager.instance.resolveState (result.stateKey, result.stateData, result.resolvedVersion);
	}



	private void OnStateUpdated(CEvent e) {
		GoogleCloudResult result = e.data as GoogleCloudResult;
		AndroidNative.showMessage ("OnStateUpdated", result.message + "\n State ID: " + result.stateKey + "\n State Data: " + result.stateDataString);
	}


	private void OnAllLoaded(CEvent e) {
		GoogleCloudResult result = e.data as GoogleCloudResult;
		AndroidNative.showMessage ("OnAllLoaded", result.message + "\n" + "Total states: " + GoogleCloudManager.instance.states.Count);
	}

	private void OnStateDeleted(CEvent e) {
		GoogleCloudManager.instance.removeEventListener (GoogleCloudManager.STATE_DELETED, OnStateDeleted);
		GoogleCloudResult result = e.data as GoogleCloudResult;


		AndroidNative.showMessage ("OnKeyDeleted", result.message + "\n for state key: " + result.stateKey.ToString());
	}

	
}
