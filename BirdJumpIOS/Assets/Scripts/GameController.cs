using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static int gameState = 0; // 0 for begin, 1 for game, 2 for end
	public static int endPhase = 0; // 0 for no, 1 for phase 1, 2 for phase 2

	public static float edge;
	public static int score = 0;
	public static int highScore = 0;

	private GameObject rootBegin;
	private GameObject rootEnd;
	private GameObject rootGameUI;
	private PlayerController playerController;
	private UIController uiController;
	private AudioController audioController;

	private int screenshotCount = 0;

	void Awake()
	{
		rootBegin = GameObject.Find("RootBegin");
		rootEnd = GameObject.Find("RootEnd");
		rootGameUI = GameObject.Find("RootGameUI");
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		uiController = GameObject.Find("UIController").GetComponent<UIController>();
		audioController = GameObject.Find("AudioController").GetComponent<AudioController>();


		edge = Screen.width * 1280f / Screen.height / 100f / 2;
	}

	void Start () 
	{
		Begin();
		PluginManager.Instance.Init();
	}

	void Update(){
		if (Input.GetKeyDown(KeyCode.R)){
			Application.CaptureScreenshot("Assets/Screenshots/screenshotbird" + screenshotCount.ToString() + ".png");
			screenshotCount++;
		}
	}

	public void Begin(){

		gameState = 0;
		score = 0;
		
		rootBegin.SetActive(true);
		rootGameUI.SetActive(false);
		rootEnd.SetActive(false);

		LoadGame();

		PluginManager.Instance.HideBanner();

	}

	public void Game(){
		gameState = 1;
		
		rootBegin.SetActive(false);
		rootGameUI.SetActive(true);
		rootEnd.SetActive(false);

		PluginManager.Instance.HideBanner();

	}

	public void End(){
		gameState = 2;
		
		rootBegin.SetActive(false);
		rootGameUI.SetActive(false);

		RenewHighScore();
		SaveGame();

		EndPhase1();

		PluginManager.Instance.ShowBanner();
		PluginManager.Instance.TryInterstitial();
	}

	public void EndPhase1(){
		endPhase = 1;

		audioController.PlayAudio("die");
		StartCoroutine("EndPhase1Wait");
	}

	IEnumerator EndPhase1Wait(){
		yield return new WaitForSeconds(.5f);
		EndPhase2();
	}

	void EndPhase2(){

		endPhase = 2;
		uiController.EndUIAnim();

	}

	void RenewHighScore(){
		if (score > highScore){
			highScore = score;
		}
	}

	public void SaveGame(){
		if (PlayerPrefs.GetInt("HighScore") < highScore){
			PlayerPrefs.SetInt("HighScore", highScore);
			PlayerPrefs.Save();
		}
	}

	public void LoadGame(){
		highScore = PlayerPrefs.GetInt("HighScore");
	}


}
