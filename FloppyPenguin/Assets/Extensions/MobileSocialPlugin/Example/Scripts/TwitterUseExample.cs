using UnityEngine;
using System.Collections;

public class TwitterUseExample : MonoBehaviour {


	//Replace with your key and secret
	private static string TWITTER_CONSUMER_KEY = "wEvDyAUr2QabVAsWPDiGwg";
	private static string TWITTER_CONSUMER_SECRET = "igRxZbOrkLQPNLSvibNC3mdNJ5tOlVOPH3HNNKDY0";



	private static bool IsUserInfoLoaded = false;


	private static bool IsAuntifivated = false;

	private GUIStyle style;
	private GUIStyle style2;

	void Awake() {
		SPTwitter.Init(TWITTER_CONSUMER_KEY, TWITTER_CONSUMER_SECRET);

		SPTwitter.twitter.addEventListener(TwitterEvents.AUTHENTICATION_SUCCEEDED,  OnAuth);
		SPTwitter.twitter.addEventListener(TwitterEvents.TWITTER_INITED,  OnInit);

		SPTwitter.twitter.addEventListener(TwitterEvents.POST_SUCCEEDED,  OnPost);
		SPTwitter.twitter.addEventListener(TwitterEvents.POST_FAILED,  OnPostFailed);

		SPTwitter.twitter.addEventListener(TwitterEvents.USER_DATA_LOADED,  OnUserDataLoaded);
		SPTwitter.twitter.addEventListener(TwitterEvents.USER_DATA_FAILED_TO_LOAD,  OnUserDataLoadFailed);


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

	}

	void OnGUI() {
		if(!IsAuntifivated) {
			GUI.Label(new Rect(10, 10, Screen.width, 100), "App do not have permission to use your twitter account, press the button to auntificate", style);
			if(GUI.Button(new Rect(10, 70, 150, 50), "Twitter Auth")) {
				SPTwitter.AuthificateUser();
			}
		} else {

			if(!IsUserInfoLoaded) {
				GUI.Label(new Rect(10, 10, Screen.width, 100), "Great, app have  permission to use your twitter account, see the avaliable action bellow", style);

				if(GUI.Button(new Rect(10, 70, 150, 50), "Load User Data")) {
					SPTwitter.LoadUserData();
				}
				
				if(GUI.Button(new Rect(10, 130, 150, 50), "Post Message")) {
					SPTwitter.Post("Hello, I'am posting this from my app");
				}

				if(GUI.Button(new Rect(10, 190, 150, 50), "Post ScreehShot")) {
					StartCoroutine(PostScreenshot());
				}

				if(GUI.Button(new Rect(10, 250, 150, 50), "Log out")) {
					LogOut();
				}
			} else {

				if(SPTwitter.twitter.userInfo.profile_background != null) {
					GUI.DrawTexture(new Rect(0, 0, SPTwitter.twitter.userInfo.profile_background.width, SPTwitter.twitter.userInfo.profile_background.height),  SPTwitter.twitter.userInfo.profile_background);
				}


				if(SPTwitter.twitter.userInfo.profile_image != null) {
					GUI.DrawTexture(new Rect(10, 10, 60, 60),  SPTwitter.twitter.userInfo.profile_image);
				}


				GUI.Label(new Rect(150, 10, Screen.width, 100),  SPTwitter.twitter.userInfo.name + " aka " + SPTwitter.twitter.userInfo.screen_name, style2);
				GUI.Label(new Rect(150, 30, Screen.width, 100),  "Location:  " + SPTwitter.twitter.userInfo.location, style2);
				GUI.Label(new Rect(150, 50, Screen.width, 100),  "Language:  " + SPTwitter.twitter.userInfo.lang, style2);

				GUI.Label(new Rect(150, 70, Screen.width, 100),  "Status:  " + SPTwitter.twitter.userInfo.status.text , style2);

				GUI.Label(new Rect(150, 90, Screen.width, 100),  SPTwitter.twitter.userInfo.name + " has  " + SPTwitter.twitter.userInfo.friends_count +  " friends and " + SPTwitter.twitter.userInfo.statuses_count + " twits", style2);


				if(GUI.Button(new Rect(10, 130, 150, 50), "Post Message")) {
					SPTwitter.Post("Hello, I'am posting this from my app");
				}
				
				if(GUI.Button(new Rect(10, 190, 150, 50), "Post ScreehShot")) {
					StartCoroutine(PostScreenshot());
				}
				
				if(GUI.Button(new Rect(10, 250, 150, 50), "Log out")) {
					LogOut();
				}
			}
		}
	}



	// --------------------------------------
	// EVENTS
	// --------------------------------------



	private void OnUserDataLoadFailed() {
		Debug.Log("Opps, user data load failed, something was wrong");
	}


	private void OnUserDataLoaded() {
		IsUserInfoLoaded = true;
		SPTwitter.twitter.userInfo.LoadProfileImage();
		SPTwitter.twitter.userInfo.LoadBackgroundImage();


		style2.normal.textColor 							= SPTwitter.twitter.userInfo.profile_text_color;
		Camera.main.GetComponent<Camera>().backgroundColor  = SPTwitter.twitter.userInfo.profile_background_color;
	}


	private void OnPost() {
		Debug.Log("Congrats, you just postet something to twitter");
	}

	private void OnPostFailed() {
		Debug.Log("Opps, post failed, something was wrong");
	}


	private void OnInit() {
		if(SPTwitter.twitter.IsAuthed) {
			OnAuth();
		}
	}


	private void OnAuth() {
		IsAuntifivated = true;
	}

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
		
		SPTwitter.Post("My app ScreehShot", tex);
		
		Destroy(tex);
		
	}

	private void LogOut() {
		IsUserInfoLoaded = false;
		
		IsAuntifivated = false;

		SPTwitter.LogOut();
	}
}
