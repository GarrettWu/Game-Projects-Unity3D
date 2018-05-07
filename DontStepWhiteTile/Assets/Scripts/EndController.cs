using UnityEngine;
using System.Collections;

public class EndController : MonoBehaviour {

	public GameObject lableBoard;

	private GameObject board;
	private GameObject labelGameOver;

	private int insertMark = -1;

	void Awake(){
		board = GameObject.Find("UI Root/EndView/Board");
		labelGameOver = GameObject.Find("UI Root/EndView/LabelGameOver");
	}

	void Start () {
		GameStates.state = 2;

		GameStates.LoadGame();

		if (GameStates.victory){
			InsertHighScore();
			GameStates.SaveGame();
		}
		SetLabelGameover();
		GenerateBoard();

		PluginManager.Instance.ShowBanner();
		PluginManager.Instance.TryInterstitial();
		PluginManager.Instance.CacheInterstitials();
	}

	void Update(){
		PluginManager.Instance.OnEscapeKey();
	}

	public void ButtonMain(){
		Application.LoadLevel("StartScene");
	}
	
	public void ButtonRetry(){
		Application.LoadLevel("CountScene");
	}

	void SetLabelGameover(){
		if (GameStates.victory){
			labelGameOver.GetComponent<UILabel>().text = "Safe";
			labelGameOver.GetComponent<UILabel>().color = Color.white;
		}
	}

	void InsertHighScore(){
		switch (GameStates.mode){
		case 0:
			for (int i = 0; i < 10; i++){
				if (GameStates.highTimeAttack[i] == 0f || GameStates.time < GameStates.highTimeAttack[i]){
					for (int j = 9; j > i; j--){
						GameStates.highTimeAttack[j] = GameStates.highTimeAttack[j-1];
					}
					GameStates.highTimeAttack[i] = GameStates.time;
					insertMark = i;
					break;
				}
			}
			break;
		case 1:
			for (int i = 0; i < 10; i++){
				if (GameStates.step > GameStates.highLongRun[i]){
					for (int j = 9; j > i; j--){
						GameStates.highLongRun[j] = GameStates.highLongRun[j-1];
					}
					GameStates.highLongRun[i] = GameStates.step;
					insertMark = i;
					break;
				}
			}
			break;
		case 2:
			for (int i = 0; i < 10; i++){
				if (GameStates.step > GameStates.highStoppingJust[i]){
					for (int j = 9; j > i; j--){
						GameStates.highStoppingJust[j] = GameStates.highStoppingJust[j-1];
					}
					GameStates.highStoppingJust[i] = GameStates.step;				
					insertMark = i;
					break;
				}
			}
			break;
		}
		
	}

	void GenerateBoard(){
		float startX = 160;
		float startY = 430;
		float intervalY = 64;
		
		for (int i = 0; i < 10; i ++){
			GameObject order = Instantiate(lableBoard) as GameObject;
			order.transform.parent = board.transform;
			order.transform.localScale = new Vector3(1f, 1f, 1f);
			order.transform.localPosition = new Vector3(-startX, startY - intervalY * i, 0);
			order.GetComponent<UILabel>().text = (i+1).ToString();
			
			GameObject score = Instantiate(lableBoard) as GameObject;
			score.transform.parent = board.transform;
			score.transform.localScale = new Vector3(1f, 1f, 1f);
			score.transform.localPosition = new Vector3(startX, startY - intervalY * i, 0);

			if (i == insertMark){
				order.GetComponent<UILabel>().color = new Color(0.7305f, 0.043f, 0.043f);
				score.GetComponent<UILabel>().color = new Color(0.7305f, 0.043f, 0.043f);
			}
			
			
			switch (GameStates.mode){
			case 0:
				if (GameStates.highTimeAttack[i] != 0)
					score.GetComponent<UILabel>().text = GameStates.highTimeAttack[i].ToString("F2");
				break;
			case 1:
				if (GameStates.highLongRun[i] != 0)
					score.GetComponent<UILabel>().text = GameStates.highLongRun[i].ToString();
				break;
			case 2:
				if (GameStates.highStoppingJust[i] != 0)
					score.GetComponent<UILabel>().text = GameStates.highStoppingJust[i].ToString();
				break;
			}
		}
	}
}
