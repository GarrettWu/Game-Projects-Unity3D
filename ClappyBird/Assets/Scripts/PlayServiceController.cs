using UnityEngine;
using System.Collections;

public class PlayServiceController : Singleton<PlayServiceController> {

	private string playerLabel = "Player disconnected";
	
	
	private const string LEADERBOARD_NAME = "leaderboard";

	private const string LEADERBOARD_ID = "CgkItcnx8YIFEAIQBg";
	

	
	public void Awake() {
		
		
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 
		

		GooglePlayConnection.instance.start (GooglePlayConnection.CLIENT_GAMES | GooglePlayConnection.CLIENT_APPSTATE );
	}
	

	public void SubmitScore(){
		GooglePlayManager.instance.submitScore (LEADERBOARD_NAME, GameController.highScore);
	}

	public void OpenLeaderBoard(){
		GooglePlayManager.instance.showLeaderBoard (LEADERBOARD_NAME);
	}
	

	private void OnPlayerConnected() {
		GooglePlayManager.instance.loadPlayer ();
	}
	

}
