using UnityEngine;
using System.Collections;

public class GameScenePause : MonoBehaviour {
	
	GameObject mainCamera;
	GameObject gameSceneUI;
	GameObject pauseMenu;
	PlayerController playerController;
	// Use this for initialization
	
	void Awake()
	{
		mainCamera = GameObject.Find("MainCamera");
		gameSceneUI = GameObject.Find("GameSceneUI");
		pauseMenu = GameObject.Find("PauseMenu");
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	
	void Start () {
		
		pauseMenu.SetActive(false);
	}
	

	public void Pause()
	{
//		Time.timeScale = 0;
		
		playerController.Pause();
		pauseMenu.SetActive(true);
		mainCamera.SetActive(false);
		gameSceneUI.SetActive(false);
		
	}
}
