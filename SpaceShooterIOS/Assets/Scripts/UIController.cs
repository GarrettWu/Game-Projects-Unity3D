using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

	private GameController gameController;

	private GameObject panelBegin;
	private GameObject panelGame;
	private GameObject panelEnd;
	private GameObject tap;
	private GameObject release;

	private UILabel scoreText;
	private UILabel scoreEnd;
	private UILabel bestEnd;

	void Awake(){
		gameController = Camera.main.GetComponent<GameController>();

		panelBegin = GameObject.Find("Canvas/PanelBegin");
		panelGame = GameObject.Find("Canvas/PanelGame");
		panelEnd = GameObject.Find("Canvas/PanelEnd");
		tap = GameObject.Find("Canvas/PanelGame/Tap");
		release = GameObject.Find("Canvas/PanelGame/Release");

		scoreText = GameObject.Find("Canvas/PanelGame/TextScore").GetComponent<UILabel>();
		scoreEnd = GameObject.Find("Canvas/PanelEnd/TextScoreCurrent").GetComponent<UILabel>();
		bestEnd = GameObject.Find ("Canvas/PanelEnd/TextScoreBest").GetComponent<UILabel>();

	}
	// Use this for initialization
	void Start () {
		UIBegin();
	}
	


	public void UIBegin(){
		panelBegin.SetActive(true);
		panelGame.SetActive(false);
		panelEnd.SetActive(false);
	}

	public void UIGame(){
		panelBegin.SetActive(false);
		panelGame.SetActive(true);
		panelEnd.SetActive(false);

		UncoverTapRelease();
	}

	public void UIEnd(){
		panelBegin.SetActive(false);
		panelGame.SetActive(false);
		panelEnd.SetActive(true);

		scoreEnd.text = GameController.score.ToString();
		bestEnd.text = GameController.highScore.ToString();
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

	public void ButtonMore(){
		SoundManager.Instance.PlayAudio("button");

	}

	public void ButtonRestart(){
		ButtonStart();		
		SoundManager.Instance.PlayAudio("button");

	}

	public void ButtonMain(){
		gameController.Begin();
		SoundManager.Instance.PlayAudio("button");

	}

	public void ScoreRefresh(){
		scoreText.text = GameController.score.ToString();
	}

	public void UncoverTapRelease(){
		tap.SetActive(true);
		release.SetActive(true);
	}

	public void HideTapRelease(){
		tap.SetActive(false);
		release.SetActive(false);
	}
}
