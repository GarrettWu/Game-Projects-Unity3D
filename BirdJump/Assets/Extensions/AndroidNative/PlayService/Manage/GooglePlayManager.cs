////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GooglePlayManager : Singletone<GooglePlayManager> {



	public const string SCORE_SUBMITED          = "score_submited";
	public const string LEADERBOARDS_LOEADED    = "leaderboards_loeaded";
	public const string PLAYER_LOADED           = "player_loaded";
	public const string ACHIEVEMENT_UPDATED       = "achievement_updated";
	public const string ACHIEVEMENTS_LOADED       = "achievements_loaded";


	private GooglePlayerTemplate _player = null ;

	private Dictionary<string, GPLeaderBoard> _leaderBoards =  new Dictionary<string, GPLeaderBoard>();

	private Dictionary<string, GPAchievement> _achievements = new Dictionary<string, GPAchievement>();


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void Create() {
		Debug.Log ("GooglePlayManager was created");
	}


	//--------------------------------------
	// PUBLIC API CALL METHODS
	//--------------------------------------



	public void showAchivmentsUI() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.showAchivmentsUI ();
	}

	public void showLeaderBoardsUI() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.showLeaderBoardsUI ();
	}

	public void showLeaderBoard(string leaderboardName) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.showLeaderBoard (leaderboardName);
	}

	public void showLeaderBoardsUI(string leaderboardId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.showLeaderBoardById (leaderboardId);
	}

	public void loadPlayer() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.loadPlayer ();
	}

	public void submitScore(string leaderboardName, int score) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.submitScore (leaderboardName, score);
	}

	public void submitScoreById(string leaderboardId, int score) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.submitScoreById (leaderboardId, score);
	}

	public void loadLeaderBoards() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.loadLeaderBoards ();
	}


	public void reportAchievement(string achievementName) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.reportAchievement (achievementName);
	}

	public void reportAchievementById(string achievementId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.reportAchievementById (achievementId);
	}


	public void revealAchievement(string achievementName) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.revealAchievement (achievementName);
	}

	public void revealAchievementById(string achievementId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.revealAchievementById (achievementId);
	}

	public void incrementAchievement(string achievementName, int numsteps) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.incrementAchievement (achievementName, numsteps.ToString());
	}

	public void incrementAchievementById(string achievementId, int numsteps) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.incrementAchievementById (achievementId, numsteps.ToString());
	}

	public void loadAchivments() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.loadAchivments ();
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public GPLeaderBoard GetLeaderBoard(string leaderboardId) {
		if(_leaderBoards.ContainsKey(leaderboardId)) {
			return _leaderBoards[leaderboardId];
		} else {
			return null;
		}
	}
	

	public GPAchievement GetAchievementd(string achievementId) {
		if(_achievements.ContainsKey(achievementId)) {
			return _achievements[achievementId];
		} else {
			return null;
		}
	}


	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public GooglePlayerTemplate player {
		get {
			return _player;
		}
	}

	public Dictionary<string, GPLeaderBoard> leaderBoards {
		get {
			return _leaderBoards;
		}
	}

	public Dictionary<string, GPAchievement> achievements {
		get {
			return _achievements;
		}
	}


	//--------------------------------------
	// EVENTS
	//--------------------------------------


	private void OnAchievementsLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.isSuccess) {

			_achievements.Clear ();

			for(int i = 1; i < storeData.Length; i+=7) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}

				GPAchievement ach = new GPAchievement (storeData[i], 
				                                       storeData[i + 1],
				                                       storeData[i + 2],
				                                       storeData[i + 3],
				                                       storeData[i + 4],
				                                       storeData[i + 5],
				                                       storeData[i + 6]
				                                       );

				Debug.Log (ach.name);
				Debug.Log (ach.type);


				_achievements.Add (ach.id, ach);

			}

			Debug.Log ("Loaded: " + _achievements.Count + " Achievements");
		}

		dispatch (ACHIEVEMENTS_LOADED, result);

	}

	private void OnAchievementUpdated(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		result.achievementId = storeData [1];

		dispatch (ACHIEVEMENT_UPDATED, result);

	}

	private void OnLeaderboardDataLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);


		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.isSuccess) {

			_leaderBoards.Clear ();

			for(int i = 1; i < storeData.Length; i+=26) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}
				GPLeaderBoard lb = new GPLeaderBoard (storeData[i], storeData [i + 1]);

				int start = i + 2;
				for(int j = 0; j < 6; j++) {
					LeaderBoardScoreVariant variant =  new LeaderBoardScoreVariant(storeData[start], storeData[start + 1], storeData[start + 2], storeData[start + 3]);
					start = start + 4;

					//Debug.Log (lb.name + ", " + variant.timeSpan.ToString() + ": " + variant.score.ToString()); 
					lb.addScoreVarian (variant);

				}

				_leaderBoards.Add (lb.id, lb);


			}

			Debug.Log ("Loaded: " + _leaderBoards.Count + " Leaderboards");
		}

		dispatch (LEADERBOARDS_LOEADED, result);

	}

	private void OnScoreSubmitted(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("GooglePlayManager OnScoreSubmitted, no data avaiable");
			return;
		}

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		result.leaderboardId = storeData [1];

		dispatch (SCORE_SUBMITED, result);

	}

	private void OnPlayerLoaded(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("GooglePlayManager OnPlayerLoaded, no data avaiable");
			return;
		}

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayResult result = new GooglePlayResult (storeData [0]);

		_player = new GooglePlayerTemplate (storeData [1], storeData [2]);

		dispatch (PLAYER_LOADED, result);
		Debug.Log ("GooglePlayManager -> PLAYER_LOADED");


	}

}
