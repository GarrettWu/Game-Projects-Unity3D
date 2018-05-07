using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	private GameController gameController;
	
	private GameObject panelBegin;
	private GameObject panelGame;
	private GameObject panelEnd;
	private GameObject panelTutorial;

	
	private UILabel scoreText;
	private UILabel scoreEnd;
	private UILabel bestEnd;
	
	void Awake(){
		gameController = Camera.main.GetComponent<GameController>();
		
		panelBegin = GameObject.Find("UI Root/PanelBegin");
		panelGame = GameObject.Find("UI Root/PanelGame");
		panelEnd = GameObject.Find("UI Root/PanelEnd");
		panelTutorial = GameObject.Find("UI Root/PanelTutorial");
		
		scoreText = GameObject.Find("UI Root/PanelGame/LabelScore").GetComponent<UILabel>();
		scoreEnd = GameObject.Find("UI Root/PanelEnd/LabelScoreCurrent").GetComponent<UILabel>();
		bestEnd = GameObject.Find ("UI Root/PanelEnd/LabelScoreBest").GetComponent<UILabel>();
		
	}
	// Use this for initialization
	void Start () {
		UIBegin();
	}

	public void UIBegin(){
		panelBegin.SetActive(true);
		panelGame.SetActive(false);
		panelEnd.SetActive(false);
		panelTutorial.SetActive(false);

	}
	
	public void UIGame(){
		panelBegin.SetActive(false);
		panelGame.SetActive(true);
		panelEnd.SetActive(false);
		panelTutorial.SetActive(false);

	}
	
	public void UIEnd(){
		panelBegin.SetActive(false);
		panelGame.SetActive(false);
		panelEnd.SetActive(true);
		panelTutorial.SetActive(false);

		scoreEnd.text = GameController.score.ToString();
		bestEnd.text = GameController.highScore.ToString();
	}

	public void UITutorial(){
		panelBegin.SetActive(false);
		panelGame.SetActive(false);
		panelEnd.SetActive(false);
		panelTutorial.SetActive(true);
	}

	public void ScoreRefresh(){
		scoreText.text = GameController.score.ToString();
	}

	public void ButtonStart(){
		gameController.Game();
		SoundManager.Instance.PlayAudio("button");
	}
	
	public void ButtonLadder(){
		SoundManager.Instance.PlayAudio("button");
		PluginManager.Instance.ReportScore(GameController.highScore);
		PluginManager.Instance.ShowLeaderBoard();
	}
	
//	public void ButtonMore(){
//		SoundManager.Instance.PlayAudio("button");
//		
//	}
	
	public void ButtonRestart(){
		ButtonStart();		
		SoundManager.Instance.PlayAudio("button");
		
	}
	
//	public void ButtonMain(){
//		gameController.Begin();
//		SoundManager.Instance.PlayAudio("button");
//		
//	}

	public void ButtonTutorial(){
		gameController.Tutorial();
		SoundManager.Instance.PlayAudio("button");
		
	}

	public void ButtonTutorialBack(){
		gameController.Begin();
		SoundManager.Instance.PlayAudio("button");
		
	}
}
