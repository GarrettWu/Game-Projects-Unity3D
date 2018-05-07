using UnityEngine;
using System.Collections;

public class SPTwitter : MonoBehaviour {


	private static TwitterManagerInterface _twitter = null;



	// --------------------------------------
	// INITIALIZATION
	// --------------------------------------


	public static void Init(string consumer_key, string consumer_secret) {
		switch(Application.platform) {
		case RuntimePlatform.Android:
			_twitter = AndroidTwitterManager.instance;
			break;
		default:
			_twitter = IOSTwitterManager.instance;
			break;
		}

		_twitter.Init(consumer_key, consumer_secret);
	}



	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------


	public static void AuthificateUser() {
		if(_twitter != null) {
			_twitter.AuthificateUser();
		} else {
			Debug.Log("SPTwitter.Init should be called before SPTwitter.AuthificateUser");
		}

	}
	
	public static void LoadUserData() {
		if(_twitter != null) {
			_twitter.LoadUserData();
		} else {
			Debug.Log("SPTwitter.Init should be called before SPTwitter.LoadUserData");
		}
	}

	
	public static void Post(string status) {
		if(_twitter != null) {
			_twitter.Post(status);
		} else {
			Debug.Log("SPTwitter.Init should be called before SPTwitter.Post");
		}
	}

	public static void Post(string status, Texture2D texture ) {
		if(_twitter != null) {
			_twitter.Post(status, texture);
		} else {
			Debug.Log("SPTwitter.Init should be called before SPTwitter.Post");
		}
	}



	public static void LogOut() {
		if(_twitter != null) {
			_twitter.LogOut();
		} else {
			Debug.Log("SPTwitter.Init should be called before SPTwitter.LogOut");
		}
	}



	// --------------------------------------
	// GET / SET
	// --------------------------------------


	public static TwitterManagerInterface twitter {
		get {
			return _twitter;
		}
	}

}
