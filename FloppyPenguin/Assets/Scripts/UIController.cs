using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {


	private GameController gameController;
	private AudioController audioController;
	//Starting
	private GameObject starting;
	private GameObject title;
	private GameObject tap;

	//Gaming
	private GameObject gaming;
	private GameObject scoreLable;

	//Ending
	private GameObject ending;
	private GameObject board;
	private GameObject score;
	private GameObject best;
	private GameObject medal;
	private GameObject gameOver;
	private GameObject go;
	private GameObject ladder;

	public Sprite medalCopper;
	public Sprite medalSilver;
	public Sprite medalGold;
	public Sprite medalPlatinum;

	
	void Awake(){
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		audioController = GameObject.Find("AudioController").GetComponent<AudioController>();

		starting = GameObject.Find("UI Root/Starting");
		title = GameObject.Find("UI Root/Starting/Title");
		tap = GameObject.Find("UI Root/Starting/Tap");

		gaming = GameObject.Find("UI Root/Gaming");
		scoreLable = GameObject.Find("UI Root/Gaming/ScoreLable");

		ending = GameObject.Find("UI Root/Ending");
		board = GameObject.Find("UI Root/Ending/Board");
		score = GameObject.Find("UI Root/Ending/Board/Score");
		best = GameObject.Find("UI Root/Ending/Board/Best");
		medal = GameObject.Find("UI Root/Ending/Board/Medal");
		gameOver = GameObject.Find("UI Root/Ending/GameOver");
		go = GameObject.Find("UI Root/Ending/Go");
		ladder = GameObject.Find("UI Root/Ending/Ladder");
	}

	void Update(){
		CheckBackButton();
	}

  	public void ScoreUpdate(){
		scoreLable.GetComponent<UILabel>().text = GameController.score.ToString();
	}

	public void Starting(){
		starting.SetActive(true);
		gaming.SetActive(true);
		ending.SetActive(false);
	}

	public void Gaming(){
		starting.SetActive(false);
		gaming.SetActive(true);
		ending.SetActive(false);
	}

	public void Ending(){
		starting.SetActive(false);
		gaming.SetActive(false);

		ending.SetActive(true);
		gameOver.SetActive(false);
		board.SetActive(false);
		go.SetActive(false);
		ladder.SetActive(false);

		GameOverShow();

	}


	

	public void GameOverShow(){
		gameOver.SetActive(true);
		audioController.PlayAudio("swooshing");

	}

	public void BoardShow(){
		board.SetActive(true);
		MedalShow();

		audioController.PlayAudio("swooshing");
	}

	public void MedalShow(){
		if (GameController.score >= 40)
			medal.GetComponent<UI2DSprite>().sprite2D = medalPlatinum;
		else if (GameController.score >= 30)
			medal.GetComponent<UI2DSprite>().sprite2D = medalGold;
		else if (GameController.score >= 20)
			medal.GetComponent<UI2DSprite>().sprite2D = medalSilver;
		else if (GameController.score >= 10)
			medal.GetComponent<UI2DSprite>().sprite2D = medalCopper;
	}

	public void ButtonsShow(){
		go.SetActive(true);
		ladder.SetActive(true);
	}

	public void StartNumRise(){

		StartCoroutine("NumRise");
	}

	public IEnumerator NumRise() {
		best.GetComponent<UILabel>().text = GameController.highScore.ToString();

		for (int i = 0; i <= GameController.score; ++i) {
			score.GetComponent<UILabel>().text = i.ToString();
			yield return new WaitForSeconds(0.02f);
		}

		ButtonsShow();
	}

	public void ButtonGo(){
		Application.LoadLevel("GameScene");
		
		audioController.PlayAudio("swooshing");
	}
	
	public void ButtonLadder(){
		
		PlayServiceController.Instance.OpenLeaderBoard();
		audioController.PlayAudio("swooshing");
	}
	
	public void CheckBackButton(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			if (CBController.Instance.isInterstitialCached())
				CBController.Instance.ShowInterstisial();
			else {
//				audioController.PlayAudio("hit");
//				CBController.Instance.CacheInterstitial();
				Application.Quit();
			}
				
		}
	}


}
