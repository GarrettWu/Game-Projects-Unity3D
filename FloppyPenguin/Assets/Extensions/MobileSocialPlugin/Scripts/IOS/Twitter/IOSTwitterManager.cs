////////////////////////////////////////////////////////////////////////////////
//  
// @module MobileSocialPlugin 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class IOSTwitterManager : Singletone<IOSTwitterManager>, TwitterManagerInterface {


	//error codes for failed auth
	private const int NO_TWITTER_ACCOUNTS_ON_DEVICE = 0;
	private const int TWITTER_PERMISSION_DENIED = 1;


	[DllImport ("__Internal")]
	private static extern void _twitterInit();

	[DllImport ("__Internal")]
	private static extern void _twitterLoadUserData();

	[DllImport ("__Internal")]
	private static extern void _twitterAuthificateUser();



	[DllImport ("__Internal")]
	private static extern void _twitterPost(string text);

	[DllImport ("__Internal")]
	private static extern void _twitterPostWithMedia(string text, string encodedMedia);


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
		Init();
	}


	public void Init() {
		if(_IsInited) {
			return;
		}
		
		_IsInited = true;

		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			_twitterInit();
		}	
	}
	
	
	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------
	
	
	public void AuthificateUser() {
		Debug.Log("Unity AuthificateUser");
		if(_IsAuthed) {
			OnAuthSuccess();
		} else {
			if(Application.platform == RuntimePlatform.IPhonePlayer) {
				_twitterAuthificateUser();
			}
		}
	}
	
	public void LoadUserData() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			_twitterLoadUserData();
		}
	}


	public void Post(string status) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			_twitterPost(status);
		}
	}
	
	public void Post(string status, Texture2D texture) {
		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);

		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			_twitterPostWithMedia(status, bytesString);
		}
	}
	
	public void LogOut() {

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
	
	
	private void OnAuthFailed(string data) {
		dispatch(TwitterEvents.AUTHENTICATION_FAILED, System.Convert.ToInt32(data));
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
