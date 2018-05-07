using UnityEngine;
using UnionAssets.FLE;
using System.Collections;

public class PlayServiceCustomLBExample : MonoBehaviour {

	//example
	private const string LEADERBOARD_ID = "CgkIipfs2qcGEAIQAA";
	//private const string LEADERBOARD_ID = "REPLACE_WITH_YOUR_ID";


	public GameObject avatar;
	private Texture defaulttexture;

	
	public DefaultPreviewButton connectButton;
	public SA_Label playerLabel;

	public DefaultPreviewButton GlobalButton;
	public DefaultPreviewButton LocalButton;


	public DefaultPreviewButton AllTimeButton;
	public DefaultPreviewButton WeekButton;
	public DefaultPreviewButton TodayButton;




	public DefaultPreviewButton SubmitScoreButton;


	public DefaultPreviewButton[] ConnectionDependedntButtons;
	public CustomLeaderboardFiledsHolder[] lines;


	private GPLeaderBoard loadedLeaderBoard = null;
	private GPCollectionType displayCollection = GPCollectionType.FRIENDS;
	private GPBoardTimeSpan displayTime = GPBoardTimeSpan.ALL_TIME;

	private int score = 100;


	//--------------------------------------
	// INITIALIZATION
	//--------------------------------------

	void Start() {



		
		playerLabel.text = "Player Diconnected";
		defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;

		foreach(CustomLeaderboardFiledsHolder line in lines) {
			line.Disable();
		}

		
		//listen for GooglePlayConnection events
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
		GooglePlayConnection.instance.addEventListener(GooglePlayConnection.CONNECTION_RESULT_RECEIVED, OnConnectionResult);

		GooglePlayManager.instance.addEventListener(GooglePlayManager.SCORE_SUBMITED, OnScoreSubmited);

		SA_StatusBar.text = "Custom Leader-board example scene loaded";

		GooglePlayManager.instance.addEventListener (GooglePlayManager.SCORE_REQUEST_RECEIVED, OnScoreListLoaded);

		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 
		
	}


	
	//--------------------------------------
	// METHODS
	//--------------------------------------



	public void LoadScore() {
		GPBoardTimeSpan displayTime = GPBoardTimeSpan.ALL_TIME;
		GPCollectionType displayCollection = GPCollectionType.FRIENDS;
		GooglePlayManager.instance.LoadPlayerCenteredScores(LEADERBOARD_ID, displayTime, displayCollection, 10);
	}

	public void OpenUI() {
		GooglePlayManager.instance.ShowLeaderBoardById(LEADERBOARD_ID);
	}
	


	public void ShowGlobal() {
		displayCollection = GPCollectionType.GLOBAL;
	}

	public void ShowLocal() {
		displayCollection = GPCollectionType.FRIENDS;
	}


	public void ShowAllTime() {
		displayTime = GPBoardTimeSpan.ALL_TIME;
	}
	
	public void ShowWeek() {
		displayTime = GPBoardTimeSpan.WEEK;
	}

	public void ShowDay() {
		displayTime = GPBoardTimeSpan.TODAY;
	}


	private void ConncetButtonPress() {
		Debug.Log("GooglePlayManager State  -> " + GooglePlayConnection.state.ToString());
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			SA_StatusBar.text = "Disconnecting from Play Service...";
			GooglePlayConnection.instance.disconnect ();
		} else {
			SA_StatusBar.text = "Connecting to Play Service...";
			GooglePlayConnection.instance.connect ();
		}
	}


	//--------------------------------------
	// UNITY
	//--------------------------------------

	void Update() {
		

		
		if(loadedLeaderBoard != null) {


			//Getting current player score
			int displayRank;

			GPScore currentPlayerScore = loadedLeaderBoard.GetCurrentPlayerScore(displayTime, displayCollection);
			if(currentPlayerScore == null) {
				//Player does not have rank at this collection / time
				//so let's show the top score
				//since we used loadPlayerCenteredScores function. we should have top scores loaded if player have no scores at this collection / time
				//https://developer.android.com/reference/com/google/android/gms/games/leaderboard/Leaderboards.html#loadPlayerCenteredScores(com.google.android.gms.common.api.GoogleApiClient, java.lang.String, int, int, int)
				//Asynchronously load the player-centered page of scores for a given leaderboard. If the player does not have a score on this leaderboard, this call will return the top page instead.
				displayRank = 1;
			} else {
				//Let's show 5 results before curent player Rank
				displayRank = Mathf.Clamp(currentPlayerScore.rank - 5, 1, currentPlayerScore.rank);

				//let's check if dosplayRank we what to display before player score is exists
				while(loadedLeaderBoard.GetScore(displayRank, displayTime, displayCollection) == null) {
					displayRank++;
				}
			}




			int i = 1;
			foreach(CustomLeaderboardFiledsHolder line in lines) {

				line.Disable();

				GPScore score = loadedLeaderBoard.GetScore(i, displayTime, displayCollection);
				if(score != null) {
					line.rank.text 			= i.ToString();
					line.score.text 		= score.score.ToString();
					line.playerId.text 		= score.playerId;

					GooglePlayerTemplate player = GooglePlayManager.instance.GetPlayerById(score.playerId);
					if(player != null) {
						line.playerName.text =  player.name;
						if(player.hasIconImage) {
							line.avatar.GetComponent<Renderer>().material.mainTexture = player.icon;
						} else {
							line.avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
						}

					} else {
						line.playerName.text = "--";
						line.avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
					}
					line.avatar.GetComponent<Renderer>().enabled = true;

				} else {
					line.Disable();
				}

				i++;
			}
		} else {
			foreach(CustomLeaderboardFiledsHolder line in lines) {
				line.Disable();
			}
		}
		
		
		
	}



	void FixedUpdate() {


		SubmitScoreButton.text = "Submit Score: " + score;
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			if(GooglePlayManager.instance.player.icon != null) {
				avatar.GetComponent<Renderer>().material.mainTexture = GooglePlayManager.instance.player.icon;
			}
		}  else {
			avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
		}





		
		
		
		
		
		
		string title = "Connect";
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			title = "Disconnect";
			
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.EnabledButton();
			}


			AllTimeButton.Unselect();
			WeekButton.Unselect();
			TodayButton.Unselect();
			
			
			switch(displayTime) {
			case GPBoardTimeSpan.ALL_TIME:
				AllTimeButton.Select();
				break;
			case GPBoardTimeSpan.WEEK:
				WeekButton.Select();
				break;
			case GPBoardTimeSpan.TODAY:
				TodayButton.Select();
				break;
			}
			
			
			
			GlobalButton.Unselect();
			LocalButton.Unselect();
			switch(displayCollection) {
			case GPCollectionType.GLOBAL:
				GlobalButton.Select();
				break;
			case GPCollectionType.FRIENDS:
				LocalButton.Select();
				break;
			}


		} else {
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.DisabledButton();
				
			}
			if(GooglePlayConnection.state == GPConnectionState.STATE_DISCONNECTED || GooglePlayConnection.state == GPConnectionState.STATE_UNCONFIGURED) {
				
				title = "Connect";
			} else {
				title = "Connecting..";
			}
		}
		
		connectButton.text = title;
	}


	//--------------------------------------
	// EVENTS
	//--------------------------------------


	private void OnScoreListLoaded() {
		
		SA_StatusBar.text = "Scores Load Finished";
		
	
		loadedLeaderBoard = GooglePlayManager.instance.GetLeaderBoard(LEADERBOARD_ID);

	}



	private void SubmitScore() {
		GooglePlayManager.instance.SubmitScoreById(LEADERBOARD_ID, score);
		SA_StatusBar.text = "Submitiong score: " + (score +1).ToString();
		score ++;
	}


	private void OnPlayerDisconnected() {
		SA_StatusBar.text = "Player Diconnected";
		playerLabel.text = "Player Diconnected";

	}
	
	private void OnPlayerConnected() {
		SA_StatusBar.text = "Player Connected";
		playerLabel.text = GooglePlayManager.instance.player.name;

	}
	
	private void OnConnectionResult(CEvent e) {
		
		GooglePlayConnectionResult result = e.data as GooglePlayConnectionResult;
		SA_StatusBar.text = "Connection Resul:  " + result.code.ToString();
		Debug.Log(result.code.ToString());
	}

	private void OnScoreSubmited(CEvent e) {
		GooglePlayResult result = e.data as GooglePlayResult;
		SA_StatusBar.text = "Score Submit Resul:  " + result.message;

		//Reloading scores list:
		GooglePlayManager.instance.LoadTopScores(LEADERBOARD_ID, GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL, 10);
		GooglePlayManager.instance.LoadTopScores(LEADERBOARD_ID, GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS, 10);

	}
}
