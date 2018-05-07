using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static int score = 0;
	public static int highScore = 0;
	public static int gameState = 0; // 0 for begin, 1 for gaming, 2 for end, 3 for tutorial

	public static Color white = Color.white;
	public static Color red = new Color(228/255f, 43/255f, 25/255f);
	public static Color yellow = new Color(238/255f, 237/255f, 16/255f);
	public static Color blue = new Color(24/255f, 80/255f, 227/255f);
	public static Color orange = new Color(239/255f, 151/255f, 15/255f);
	public static Color green = new Color(45/255f, 203/255f, 24/255f);
	public static Color purple = new Color(171/255f, 38/255f, 213/255f);
	public static Color black = new Color(49/255f, 49/255f, 49/255f);
	
	private UIController uiController;
	private EnemyGenerator enemyGenerator;
	private GameObject colors;
	private BackgroundController backgroundController;
	
	void Awake(){
		uiController = GameObject.Find("UI Root").GetComponent<UIController>();
		colors = GameObject.Find("Colors");
		enemyGenerator = GameObject.Find("EnemyGenerator").GetComponent<EnemyGenerator>();
		backgroundController = GameObject.Find("Background").GetComponent<BackgroundController>();
	}
	// Use this for initialization
	void Start () {
		Begin();
		PluginManager.Instance.Init ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Begin(){
		gameState = 0;
		score = 0;
		
		colors.SetActive(false);
		uiController.UIBegin();
		
		LoadGame();
	}
	
	public void Game(){
		gameState = 1;

		colors.SetActive(true);

		enemyGenerator.ResetEnemyGenerator();
		enemyGenerator.StartGenerate();
		backgroundController.RandomBackColor();
		uiController.UIGame();
		
		score = 0;
		uiController.ScoreRefresh();

		PluginManager.Instance.HideBanner();
	}
	
	public void End(){
		gameState = 2;

		RenewHighScore();
		SaveGame();
		
		uiController.UIEnd();

		PluginManager.Instance.ShowBanner();
		PluginManager.Instance.TryInterstitial();
	}

	public void Tutorial(){
		gameState = 3;

		uiController.UITutorial();
	}
	
	IEnumerator WaitEnd() {
		enemyGenerator.StopGenerate();

		colors.SetActive(false);
		DestroyEnemies();
		yield return new WaitForSeconds(2.0f);
		End();
	}
	
	public void StartWaitEnd(){
		StartCoroutine("WaitEnd");
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
	
	public void ScoreAdd(){
		score++;
		uiController.ScoreRefresh();
		
		if (score % 10 == 0) {
			enemyGenerator.FallSpeedIncrement();
			enemyGenerator.IntervalMeanDecrement();
			backgroundController.RandomBackColor();
		}
	}
//	
	void DestroyEnemies(){
		GameObject theEnemies = GameObject.FindWithTag("Enemies");
		Destroy(theEnemies);

	}
}
