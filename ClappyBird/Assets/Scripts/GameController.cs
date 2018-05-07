using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour{

	public static int gameState = 0; //0 for Starting, 1 for Gaming, 2 for Ending
	public static int score = 0;
	public static int highScore = 0;

	private UIController uiController;
	private BirdController birdController;
	

	void Awake(){
		uiController = GameObject.Find("UIController").GetComponent<UIController>();
		birdController = GameObject.Find("Bird").GetComponent<BirdController>();


	}

	// Use this for initialization
	void Start () {
		Starting();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Starting(){
		LoadGame();

		PluginManager.Instance.Init();
		PlayServiceController.Instance.Init();

		PluginManager.Instance.CacheInterstitials();

		score = 0;
		uiController.Starting();
		gameState = 0;

	}

	public void Gaming(){
		uiController.Gaming ();

		PluginManager.Instance.HideBanner();
		gameState = 1;
	}

	public void Ending(){
		if (score > highScore){
			highScore = score;
			SaveGame();

		}

		uiController.Ending();

		PluginManager.Instance.CreateBanner();
		PluginManager.Instance.ShowBanner();


		PluginManager.Instance.TryInterstitial();

		gameState = 2;
	}

	public void SaveGame(){
		PlayerPrefs.SetInt("HighScore", highScore);
	}

	public void LoadGame(){
		highScore = PlayerPrefs.GetInt("HighScore");
	}
}
