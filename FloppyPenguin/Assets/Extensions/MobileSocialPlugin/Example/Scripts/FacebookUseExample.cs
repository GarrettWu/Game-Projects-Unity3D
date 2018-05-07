using UnityEngine;
using System.Collections;

public class FacebookUseExample : MonoBehaviour {


	private static bool IsUserInfoLoaded = false;
	private static bool IsFrindsInfoLoaded = false;
	private static bool IsAuntifivated = false;

	
	private GUIStyle style;
	private GUIStyle style2;
	private GUIStyle statusStyle;


	private string statusMessage = "";


	void Awake() {


		SPFacebook.instance.addEventListener(FacebookEvents.FACEBOOK_INITED, 			 OnInit);
		SPFacebook.instance.addEventListener(FacebookEvents.AUTHENTICATION_SUCCEEDED,  	 OnAuth);
		SPFacebook.instance.addEventListener(FacebookEvents.AUTHENTICATION_FAILED,  	 OnAuthFailed);
		

		SPFacebook.instance.addEventListener(FacebookEvents.USER_DATA_LOADED,  			OnUserDataLoaded);
		SPFacebook.instance.addEventListener(FacebookEvents.USER_DATA_FAILED_TO_LOAD,   OnUserDataLoadFailed);

		SPFacebook.instance.addEventListener(FacebookEvents.FRIENDS_DATA_LOADED,  			OnFriendsDataLoaded);
		SPFacebook.instance.addEventListener(FacebookEvents.FRIENDS_FAILED_TO_LOAD,   		OnFriendDataLoadFailed);

		SPFacebook.instance.addEventListener(FacebookEvents.POST_FAILED,  			OnPostFailed);
		SPFacebook.instance.addEventListener(FacebookEvents.POST_SUCCEEDED,   		OnPost);


		SPFacebook.instance.addEventListener(FacebookEvents.GAME_FOCUS_CHANGED,   OnFocusChanged);

		SPFacebook.instance.Init();

		statusMessage = "initializing Facebook";

		InitGUI();


		/*FB.AppRequest(
				message: "hello",
				title: "title"
				); */


		/*
			FB.Feed(
				toId: "alexandr.volga",
				link: "https://developers.facebook.com/docs/android/getting-started/",
				linkName: "Getting Started ",
				linkCaption: "Getting Started2 ",
				linkDescription: "Getting Started linkDescription"
				);
				*/

	}


	private void InitGUI() {
		style =  new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 18;
		style.fontStyle = FontStyle.BoldAndItalic;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		
		
		style2 =  new GUIStyle();
		style2.normal.textColor = Color.white;
		style2.fontSize = 12;
		style2.fontStyle = FontStyle.Italic;
		style2.alignment = TextAnchor.UpperLeft;
		style2.wordWrap = true;

		statusStyle =  new GUIStyle(style2);
		statusStyle.fontSize = 16;
	}

	
	void OnGUI() {

		GUI.Label(new Rect(10, Screen.height - 40, Screen.width, 40), statusMessage, statusStyle);
		
		if(!IsAuntifivated) {
			GUI.Label(new Rect(10, 10, Screen.width, 100), "App do not have permission to use your facebook account, press the button to auntificate", style);
			if(GUI.Button(new Rect(10, 70, 150, 50), "Facebook Auth")) {
				SPFacebook.instance.Login("email,publish_actions");
				statusMessage = "Log in...";
			}
		} else {
			
			if(!IsUserInfoLoaded) {
				GUI.Label(new Rect(10, 10, Screen.width, 100), "Great, app have  permission to use your facebook account, see the avaliable action bellow", style);
				
				if(GUI.Button(new Rect(10, 70, 150, 50), "Load User Data")) {
					SPFacebook.instance.LoadUserData();
					statusMessage = "Loadin user data..";
				}
				
				if(GUI.Button(new Rect(10, 130, 150, 50), "Post Message")) {
					SPFacebook.instance.Post (
						link: "http://unity3d.com/",
						linkName: "The Larch",
						linkCaption: "I thought up a witty tagline about larches",
						linkDescription: "There are a lot of larch trees around here, aren't there?",
						picture: "http://unity3d.com/sites/default/files/frontpage/learn.jpg"
						);

					statusMessage = "Positng..";
				}
				
				if(GUI.Button(new Rect(10, 190, 150, 50), "Post ScreehShot")) {
					StartCoroutine(PostScreenshot());
					statusMessage = "Positng..";
				}
				
				if(GUI.Button(new Rect(10, 250, 150, 50), "Log out")) {
					LogOut();
					statusMessage = "Logged out";
				}
			} else {
				
				if(SPFacebook.instance.userInfo.GetProfileImage(FacebookProfileImageSize.large) != null) {
					Texture2D img = SPFacebook.instance.userInfo.GetProfileImage(FacebookProfileImageSize.large);
					GUI.DrawTexture(new Rect(10, 10, img.width, img.height),  img);
				}
				
				

				
				GUI.Label(new Rect(240, 10, Screen.width, 100),  SPFacebook.instance.userInfo.name + " aka " + SPFacebook.instance.userInfo.username, style2);
				GUI.Label(new Rect(240, 30, Screen.width, 100),  "Location:  " + SPFacebook.instance.userInfo.location, style2);
				GUI.Label(new Rect(240, 50, Screen.width, 100),  "Language:  " + SPFacebook.instance.userInfo.locale, style2);
				
				GUI.Label(new Rect(240, 70, Screen.width, 100),  "e-mail:  " + SPFacebook.instance.userInfo.email , style2);


				float x = 260;
				float y = 110;


				if(IsFrindsInfoLoaded) {
					foreach(FacebookUserInfo friend in SPFacebook.instance.firendsList) {
				
						if(friend.GetProfileImage(FacebookProfileImageSize.square) != null) {
							Texture2D img = friend.GetProfileImage(FacebookProfileImageSize.square);
							GUI.DrawTexture(new Rect(x, y, img.width, img.height),  img);

							GUI.Label(new Rect(x - 15, y + 80, Screen.width, 100),  friend.first_name , style2);
						} 

						x+= 100;
					}
				}
				
	
				if(GUI.Button(new Rect(10, 300, 150, 50), "Post Message")) {
					SPFacebook.instance.Post (
						link: "http://unity3d.com/",
						linkName: "The Larch",
						linkCaption: "I thought up a witty tagline about larches",
						linkDescription: "There are a lot of larch trees around here, aren't there?",
						picture: "http://unity3d.com/sites/default/files/frontpage/learn.jpg"
						);
					
					statusMessage = "Positng..";
				}


				
				if(GUI.Button(new Rect(10, 360, 150, 50), "Post ScreehShot")) {
					StartCoroutine(PostScreenshot());
					statusMessage = "Positng..";
				}

				if(GUI.Button(new Rect(10, 420, 150, 50), "LoadFriends")) {
					SPFacebook.instance.LoadFrientdsInfo(5);
					statusMessage = "Loading friends..";
				}
				
				if(GUI.Button(new Rect(10, 480, 150, 50), "Log out")) {
					LogOut();
					statusMessage = "Logged out";
				}
			}
		}
		
		


	}


	// --------------------------------------
	// EVENTS
	// --------------------------------------
	
	
	private void OnFocusChanged(CEvent e) {
		bool focus = (bool) e.data;

		if (!focus)  {                                                                                        
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		} else  {                                                                                        
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}   
	}


	private void OnUserDataLoadFailed() {
		statusMessage ="Opps, user data load failed, something was wrong";
		Debug.Log("Opps, user data load failed, something was wrong");
	}
	
	
	private void OnUserDataLoaded() {
		statusMessage = "User data loaded";
		IsUserInfoLoaded = true;
		SPFacebook.instance.userInfo.LoadProfileImage(FacebookProfileImageSize.large);
	}

	private void OnFriendDataLoadFailed() {
		statusMessage = "Opps, friends data load failed, something was wrong";
		Debug.Log("Opps, friends data load failed, something was wrong");
	}

	private void OnFriendsDataLoaded() {
		statusMessage = "Friends data loaded";
		foreach(FacebookUserInfo friend in SPFacebook.instance.firendsList) {
			friend.LoadProfileImage(FacebookProfileImageSize.square);
		}

		IsFrindsInfoLoaded = true;
	}
	

	
	
	private void OnInit() {

		if(SPFacebook.instance.IsLoggedIn) {
			OnAuth();
		} else {
			statusMessage = "user Login -> fale";
		}
	}
	
	
	private void OnAuth() {
		IsAuntifivated = true;
		statusMessage = "user Login -> true";
	}

	private void OnAuthFailed(CEvent e) {
		FBResult result = e.data as FBResult;
		statusMessage = "Auth Failed: " + result.Error;

	}

	private void OnPost() {
		statusMessage = "Posting complete";
	}
	
	private void OnPostFailed() {
		statusMessage = "Opps, post failed, something was wrong";
		Debug.Log("Opps, post failed, something was wrong");
	}
	


	
	// --------------------------------------
	// PRIVATE METHODS
	// --------------------------------------

	// --------------------------------------
	// PRIVATE METHODS
	// --------------------------------------
	
	private IEnumerator PostScreenshot() {
		
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();

		SPFacebook.instance.PostImage("My app ScreehShot", tex);;
		
		Destroy(tex);
		
	}
	
	private void LogOut() {
		IsUserInfoLoaded = false;
		
		IsAuntifivated = false;
		
		SPFacebook.instance.Logout();
	}



}
