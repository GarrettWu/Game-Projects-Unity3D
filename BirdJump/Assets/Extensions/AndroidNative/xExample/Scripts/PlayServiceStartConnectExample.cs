////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class PlayServiceStartConnectExample : MonoBehaviour {

	// Use this for initialization
	private string playerLabel = "Player disconnected";
	
	private int score = 100;
	
	
	
	private const string LEADERBOARD_NAME = "leaderboard_easy";
	
	
	//example
	//private const string LEADERBOARD_ID = "CgkI3rzhk6QZEAIQBw";
	private const string LEADERBOARD_ID = "REPLACE_WITH_YOUR_ID";
	
	
	
	void Start() {
		//listen for GooglePlayConnection events
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);

		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.CONNECTION_INITIALIZED, OnConnect);
		
		
		//listen for GooglePlayManager events
		GooglePlayManager.instance.addEventListener (GooglePlayManager.ACHIEVEMENT_UPDATED, OnAchivmentUpdated);
		GooglePlayManager.instance.addEventListener (GooglePlayManager.PLAYER_LOADED, OnPlayerInfoLoaded);
		GooglePlayManager.instance.addEventListener (GooglePlayManager.SCORE_SUBMITED, OnScoreSubmited);

		
		
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 
		
		
		//should be called on your application start 
		//best practice to call it only once. Any way other calls will be ignored by the plugin.
		//if you want to use only Game service (Leader-boards, achievements) use GooglePlayConnection.CLIENT_GAMES
		//if you want to use only Google Cloud service use GooglePlayConnection.CLIENT_APPSTATE
		//if you want both: GooglePlayConnection.CLIENT_GAMES | GooglePlayConnection.CLIENT_APPSTATE
		//and if you whant to get avaliable permissions for your app use: GooglePlayConnection.CLIENT_ALL
		GooglePlayConnection.instance.start (GooglePlayConnection.CLIENT_GAMES | GooglePlayConnection.CLIENT_APPSTATE );


	}

	private void OnConnect() {
		Debug.Log("OnConnect");
		GooglePlayConnection.instance.connect ();
	}
	
	
	void OnGUI() {
		
		GUI.Label (new Rect(20, 25, 150, 50), playerLabel);
		
		
		string title = "GooglePlay Connect";
		
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			title = "GooglePlay Disconnect";
		}
		
		
		if(GUI.Button(new Rect(10, 70, 150, 50), title)) {
			Debug.Log("GooglePlayManager State  -> " + GooglePlayConnection.state.ToString());
			if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
				GooglePlayConnection.instance.disconnect ();
			} else {
				GooglePlayConnection.instance.connect ();
			}
			
		}
		
		if(GUI.Button(new Rect(10, 130, 150, 50), "Achivments UI")) {
			GooglePlayManager.instance.showAchivmentsUI ();
		}
		
		if(GUI.Button(new Rect(10, 190, 150, 50), "Leader Boards UI")) {
			GooglePlayManager.instance.showLeaderBoardsUI ();
		}
		
		
		
		/*************************/
		
		
		if(GUI.Button(new Rect(170, 190, 150, 50), "L-Board UI by Name")) {
			GooglePlayManager.instance.showLeaderBoard (LEADERBOARD_NAME);
		}
		
		
		if(GUI.Button(new Rect(170, 70, 150, 50), "Load LeaderBoards")) {
			GooglePlayManager.instance.addEventListener (GooglePlayManager.LEADERBOARDS_LOEADED, OnLeaderBoardsLoaded);
			GooglePlayManager.instance.loadLeaderBoards ();
			
		}
		
		if(GUI.Button(new Rect(170, 130, 150, 50), "SubmitScore")) {
			score++;
			GooglePlayManager.instance.submitScore (LEADERBOARD_NAME, score);
			
		}
		
		
		
		/*************************/
		
		if(GUI.Button(new Rect(330, 70, 150, 50), "Report Achivment")) {
			GooglePlayManager.instance.reportAchievement ("achievement_prime");
		}
		
		if(GUI.Button(new Rect(330, 130, 150, 50), "Increment Achivment")) {
			GooglePlayManager.instance.incrementAchievement ("achievement_bored", 2);
		}
		
		if(GUI.Button(new Rect(330, 190, 150, 50), "Reveal Achivment")) {
			GooglePlayManager.instance.revealAchievement ("achievement_humble");
		}
		
		if(GUI.Button(new Rect(330, 250, 150, 50), "Load Achivments")) {
			GooglePlayManager.instance.addEventListener (GooglePlayManager.ACHIEVEMENTS_LOADED, OnAchivmentsLoaded);
			GooglePlayManager.instance.loadAchivments ();
		}
		
		
		/*************************/
		
		
		Color c = GUI.color;
		GUI.color = Color.green;
		if(GUI.Button(new Rect(10, 310, 150, 50), "Billing Example")) {
			
			Application.LoadLevel ("BillingExample");
			
		}
		
		
		if(GUI.Button(new Rect(170, 310, 150, 50), "Other Examples")) {
			
			Application.LoadLevel ("OtherFeaturesExample");
			
		}
		
		GUI.color = Color.red;
		
		if(GUI.Button(new Rect(330, 310, 150, 50), "ExitApp")) {
			Application.Quit ();
		}
		
		GUI.color = c;
		
		
	}
	
	
	//--------------------------------------
	// EVENTS
	//--------------------------------------
	
	private void OnAchivmentsLoaded(CEvent e) {
		GooglePlayManager.instance.removeEventListener (GooglePlayManager.ACHIEVEMENTS_LOADED, OnAchivmentsLoaded);
		GooglePlayResult result = e.data as GooglePlayResult;
		if(result.isSuccess) {
			AndroidNative.showMessage ("OnAchivmentsLoaded", "Total Achivments: " + GooglePlayManager.instance.achievements.Count.ToString());
		} else {
			AndroidNative.showMessage ("OnAchivmentsLoaded error: ", result.message);
		}
		
	}
	
	private void OnAchivmentUpdated(CEvent e) {
		GooglePlayResult result = e.data as GooglePlayResult;
		AndroidNative.showMessage ("OnAchivmentUpdated ", "Id: " + result.achievementId + "\n status: " + result.message);
	}
	
	
	private void OnLeaderBoardsLoaded(CEvent e) {
		GooglePlayManager.instance.removeEventListener (GooglePlayManager.LEADERBOARDS_LOEADED, OnLeaderBoardsLoaded);
		
		GooglePlayResult result = e.data as GooglePlayResult;
		if(result.isSuccess) {
			if( GooglePlayManager.instance.GetLeaderBoard(LEADERBOARD_ID) == null) {
				AndroidNative.showMessage("Leader boards loaded", LEADERBOARD_ID + " not found in leader boards list");
				return;
			}
			
			
			AndroidNative.showMessage (LEADERBOARD_NAME + "  score",  GooglePlayManager.instance.GetLeaderBoard(LEADERBOARD_ID).GetScore(GPCollectionType.COLLECTION_PUBLIC, GPBoardTimeSpan.TIME_SPAN_ALL_TIME).ToString());
		} else {
			AndroidNative.showMessage ("OnLeaderBoardsLoaded error: ", result.message);
		}
	}
	
	private void OnScoreSubmited(CEvent e) {
		GooglePlayResult result = e.data as GooglePlayResult;
		AndroidNative.showMessage ("OnScoreSubmited", result.message);
	}
	
	private void OnPlayerInfoLoaded(CEvent e) {
		GooglePlayResult result = e.data as GooglePlayResult;
		
		if(result.isSuccess) {
			playerLabel = GooglePlayManager.instance.player.name;
		} else {
			playerLabel = "error: " + result.message;
		}
	}
	
	private void OnPlayerDisconnected() {
		playerLabel = "Player disconnected";
	}
	
	private void OnPlayerConnected() {
		GooglePlayManager.instance.loadPlayer ();
	}
	
	
	void OnDestroy() {
		if(!GooglePlayConnection.IsDestroyed) {
			GooglePlayConnection.instance.removeEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
			GooglePlayConnection.instance.removeEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
			
		}
		
		if(!GooglePlayManager.IsDestroyed) {
			GooglePlayManager.instance.removeEventListener (GooglePlayManager.ACHIEVEMENT_UPDATED, OnAchivmentUpdated);
			GooglePlayManager.instance.removeEventListener (GooglePlayManager.PLAYER_LOADED, OnPlayerInfoLoaded);
			GooglePlayManager.instance.removeEventListener (GooglePlayManager.SCORE_SUBMITED, OnScoreSubmited);
		}
		
	}
}
