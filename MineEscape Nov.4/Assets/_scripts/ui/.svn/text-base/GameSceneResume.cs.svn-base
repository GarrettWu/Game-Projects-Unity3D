using UnityEngine;
using System.Collections;

public class GameSceneResume : MonoBehaviour {

	GameObject mainCamera;
	GameObject gameSceneUI;
	GameObject pauseMenu;
	PlayerController playerController;
	
	// Use this for initialization
	void Awake () {
		mainCamera = GameObject.Find("MainCamera");
		gameSceneUI = GameObject.Find("GameSceneUI");
		pauseMenu = GameObject.Find("PauseMenu");
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	
	
	public void Resume()
	{
//		Time.timeScale = 1;
		
		playerController.Resume();
		mainCamera.SetActive(true);
		gameSceneUI.SetActive(true);
		pauseMenu.SetActive(false);
	}
}
