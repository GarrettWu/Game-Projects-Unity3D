using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static int score = 0;
	public static int best = 0;

	GameUI gameUI;

	void Awake(){
		gameUI = GameObject.Find("UI Root").GetComponent<GameUI>();
	}

	void Start(){
		LoadGame();
	}


	public void SaveGame(){
		if (PlayerPrefs.GetInt("best") < best){
			PlayerPrefs.SetInt("best", best);
			PlayerPrefs.Save();
		}
	}
	
	public void LoadGame(){
		best = PlayerPrefs.GetInt("best");
	}
}
