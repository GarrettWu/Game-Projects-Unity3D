using UnityEngine;
using System.Collections;

public class PlayServiceManager: Singleton<PlayServiceManager> {
	
	
	private const string LEADERBOARD_NAME = "BestScores";

	private const string LEADERBOARD_ID = "CgkI88GMzqgcEAIQBQ";
	

	
	public void Awake() {
		GooglePlayConnection.instance.connect ();
	}
	

	public void SubmitScore(){
		GooglePlayManager.instance.SubmitScore (LEADERBOARD_NAME, GameController.highScore);
	}

	public void OpenLeaderBoard(){
//		GooglePlayManager.instance.showLeaderBoard (LEADERBOARD_NAME);
		GooglePlayManager.instance.ShowLeaderBoardById(LEADERBOARD_ID);
	}


}
