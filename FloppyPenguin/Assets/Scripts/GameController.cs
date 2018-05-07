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

		AdmobController.Instance.Init ();
		AnalyticsController.Instance.Init();
		PlayServiceController.Instance.Init();
		CBController.Instance.Init();

		AdmobController.Instance.LoadInterstitial();
		CBController.Instance.CacheInterstitial();

		score = 0;
		uiController.Starting();
		gameState = 0;

	}

	public void Gaming(){
		uiController.Gaming ();

		AdmobController.Instance.HideBanner();
		gameState = 1;
	}

	public void Ending(){
		if (score > highScore){
			highScore = score;
			SaveGame();
			PlayServiceController.Instance.SubmitScore();
		}

		uiController.Ending();

		AdmobController.Instance.CreateBanner();
		AdmobController.Instance.ShowBanner();
		AdmobController.Instance.CheckToShowInterstitial();


		gameState = 2;
	}

	public void SaveGame(){
		PlayerPrefs.SetInt("HighScore", highScore);
	}

	public void LoadGame(){
		highScore = PlayerPrefs.GetInt("HighScore");
	}
}
