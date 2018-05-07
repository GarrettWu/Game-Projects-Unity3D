using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static int score = 0;
	public static int highScore = 0;
	public static int gameState = 0; // 0 for begin, 1 for gaming, 2 for end

	private UIController uiController;
	private GameObject player;
	private EnemyGenerator enemyGenerator;

	void Awake(){
		uiController = GameObject.Find("Canvas").GetComponent<UIController>();
		player = GameObject.Find("Player");
		enemyGenerator = GameObject.Find("EnemyGenerator").GetComponent<EnemyGenerator>();
	}
	// Use this for initialization
	void Start () {
		Begin();
		PluginManager.Instance.Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Begin(){
		gameState = 0;
		score = 0;

		player.SetActive(false);
		uiController.UIBegin();
		
		LoadGame();
	}
	
	public void Game(){
		gameState = 1;

		DestroyEnemies();

		player.SetActive(true);
		player.transform.eulerAngles = Vector3.zero;

		enemyGenerator.ResetEnemyGenerator();
		enemyGenerator.StartGenerate();
		uiController.UIGame();

		score = 0;
		uiController.ScoreRefresh();

		PluginManager.Instance.HideBanner();
	}
	
	public void End(){
		gameState = 2;

		enemyGenerator.StopGenerate();

		
		RenewHighScore();
		SaveGame();

		uiController.UIEnd();

		PluginManager.Instance.ShowBanner();
		PluginManager.Instance.TryInterstitial();
	}

	IEnumerator WaitEnd() {
		yield return new WaitForSeconds(1.0f);
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
			enemyGenerator.GenerateAccelerate();
		}
	}

	void DestroyEnemies(){
		Destroy(GameObject.FindWithTag("Enemy"));
	}
}
