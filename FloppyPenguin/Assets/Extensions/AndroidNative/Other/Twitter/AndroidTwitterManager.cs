using UnityEngine;
using System.Collections;

public class AndroidTwitterManager : Singletone<AndroidTwitterManager>, TwitterManagerInterface {


	private bool _IsAuthed = false;
	private bool _IsInited = false;

	private TwitterUserInfo _userInfo;


	// --------------------------------------
	// INITIALIZATION
	// --------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void Init(string consumer_key, string consumer_secret) {
		if(_IsInited) {
			return;
		}

		_IsInited = true;
		AndroidNative.TwitterInit(consumer_key, consumer_secret);
	}


	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------


	public void AuthificateUser() {
		if(_IsAuthed) {
			OnAuthSuccess();
		} else {
			AndroidNative.AuthificateUser();
		}
	}

	public void LoadUserData() {
		if(_IsAuthed) {
			AndroidNative.LoadUserData();
		} else {
			Debug.LogWarning("Auth user before loadin data, fail event generated");
			dispatch(TwitterEvents.USER_DATA_FAILED_TO_LOAD);
		}
	}
	
	public void Post(string status) {
		if(!_IsAuthed) {
			Debug.LogWarning("Auth user before posting data, fail event generated");
			dispatch(TwitterEvents.POST_FAILED);
			return;
		} 

		AndroidNative.TwitterPost(status);
	}

	public void Post(string status, Texture2D texture) {

		if(!_IsAuthed) {
			Debug.LogWarning("Auth user before posting data, fail event generated");
			dispatch(TwitterEvents.POST_FAILED);
			return;
		} 


		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);

		AndroidNative.TwitterPostWithImage(status, bytesString);
	}

	public void LogOut() {
		_IsAuthed = false;
		AndroidNative.LogoutFromTwitter();
	}

	// --------------------------------------
	// GET / SET
	// --------------------------------------

	public bool IsAuthed {
		get {
			return _IsAuthed;
		}
	}

	public TwitterUserInfo userInfo {
		get {
			return _userInfo;
		}
	}


	
	// --------------------------------------
	// EVENTS
	// --------------------------------------



	private void OnInited(string data) {
		if(data.Equals("1")) {
			_IsAuthed = true;
		}

		dispatch(TwitterEvents.TWITTER_INITED);
	}

	private void OnAuthSuccess() {
		_IsAuthed = true;
		dispatch(TwitterEvents.AUTHENTICATION_SUCCEEDED);
	}


	private void OnAuthFailed() {
		dispatch(TwitterEvents.AUTHENTICATION_FAILED);
	}

	private void OnPostSuccess() {
		dispatch(TwitterEvents.POST_SUCCEEDED);
	}
	
	
	private void OnPostFailed() {
		dispatch(TwitterEvents.POST_FAILED);
	}


	private void OnUserDataLoaded(string data) {
		_userInfo =  new TwitterUserInfo(data);
		dispatch(TwitterEvents.USER_DATA_LOADED);

	}


	private void OnUserDataLoadFailed() {
		dispatch(TwitterEvents.USER_DATA_FAILED_TO_LOAD);
	}


}

