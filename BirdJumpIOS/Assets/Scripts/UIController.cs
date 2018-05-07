using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	private GameController gameController;
	private AudioController audioController;

	private UILabel gameScoreLabel;
	private UILabel endScoreLabel;
	private UILabel endBestScoreLabel;
	private GameObject rootEnd;
	private GameObject board;

	void Awake(){
		gameController = GameObject.Find("RootGame").GetComponent<GameController>();
		audioController = GameObject.Find("AudioController").GetComponent<AudioController>();


		gameScoreLabel = GameObject.Find("RootGameUI/LabelScore").GetComponent<UILabel>();
		endScoreLabel = GameObject.Find("RootEnd/Board/LabelScore").GetComponent<UILabel>();
		endBestScoreLabel = GameObject.Find("RootEnd/Board/LabelBestScore").GetComponent<UILabel>();

		rootEnd = GameObject.Find("RootEnd");
		board = GameObject.Find("RootEnd/Board");
	}

	void Update(){
		CheckBeginTouch();
		UpdateScore();
	}

	void CheckBeginTouch(){
		if (GameController.gameState == 0 && (Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ) )
		{
			gameController.Game();
		}
	}

	void UpdateScore(){
		gameScoreLabel.text = GameController.score.ToString();
		endScoreLabel.text = GameController.score.ToString();
		endBestScoreLabel.text = GameController.highScore.ToString();
	}

	public void BeginUIAnim(){

	}

	public void EndUIAnim(){
		rootEnd.SetActive(true);

		iTween.MoveFrom(board, iTween.Hash("islocal", true, "position", new Vector3(0, 1600, 0), "time", 1.0f, "easetype", "easeOutElastic"));
	}

	public void ButtonGo(){
		Application.LoadLevel("GameScene");
		audioController.PlayAudio("button");
	}

	public void ButtonLeaderBoard(){
		PluginManager.Instance.ShowLeaderBoard();
		PluginManager.Instance.ReportScore(GameController.highScore);


		audioController.PlayAudio("button");
	}
}
