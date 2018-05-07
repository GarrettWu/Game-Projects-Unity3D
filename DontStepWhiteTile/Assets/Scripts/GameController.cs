using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public class Tile{
		public Vector3 position;
		public GameObject tileObject;
	}
	
	public GameObject tileBlack;
	public GameObject tileWhite;
	public GameObject tileHarbor;

	private Tile[,] tiles;
	
	private float tileStartX = -270;
	private float tileStartY = -480;
	private float tileHeight = 320;
	private float tileWidth = 180;

	private Camera gameCamera;
	private GameObject gameView;
	private GameObject labelStep;
	private GameObject labelTime;
	private GameObject footprint;
	private GameObject labelGameOver;
	private GameObject touchArea0;
	private GameObject touchArea1;
	private GameObject touchArea2;
	private GameObject touchArea3;

	private bool isLeftStep = true;
	private bool isAnimating = false;

	void Awake(){
		tiles = new Tile[4,6];

		gameCamera = GameObject.Find("UI Root/Camera").camera;
		gameView = GameObject.Find("UI Root/GameView");

		labelStep = GameObject.Find("UI Root/GameView/LabelStep");
		labelTime = GameObject.Find("UI Root/GameView/LabelTime");
		labelGameOver = GameObject.Find("UI Root/GameView/LabelGameOver");

		footprint = GameObject.Find("UI Root/GameView/Footprint");

		touchArea0 = GameObject.Find("UI Root/GameView/TouchArea0");
		touchArea1 = GameObject.Find("UI Root/GameView/TouchArea1");
		touchArea2 = GameObject.Find("UI Root/GameView/TouchArea2");
		touchArea3 = GameObject.Find("UI Root/GameView/TouchArea3");

		
	}

	void Start () {

		SetTileSize();
		SetTilePositions();
		SetTouchAreas();
		SetFootprintPosition();
		GenerateTiles();

		InitScoresShow();

		GameStates.state = 1;
		GameStates.victory = false;
	}
	
	void Update () {

		PluginManager.Instance.OnEscapeKey();
		CheckTouch ();
		CheckHideScore();
		if (isAnimating)
			return;

		CheckVictory();
		UpdateScores();
	}

	void InitScoresShow(){
		labelGameOver.SetActive(false);

		if (GameStates.mode == 0){
			labelStep.SetActive(false);
			labelTime.SetActive(false);
			GameStates.step = 0;
			GameStates.time = 0;
		} else if (GameStates.mode == 1){
			GameStates.step = 0;
			GameStates.time = 10.0f;
		} else if (GameStates.mode == 2){
			labelStep.SetActive(false);
			GameStates.step = 0;
			GameStates.time = 10.0f;
		}
	}
	

	void UpdateScores(){
		if (GameStates.mode == 0){
			GameStates.time += Time.deltaTime;
		} else if (GameStates.mode == 1){
			GameStates.time -= Time.deltaTime;
			labelTime.GetComponent<UILabel>().text = GameStates.time.ToString("F2");
			labelStep.GetComponent<UILabel>().text = GameStates.step.ToString();
		} else if (GameStates.mode == 2){
			GameStates.time -= Time.deltaTime;
			labelTime.GetComponent<UILabel>().text = GameStates.time.ToString("F2");
		}
	}

	void SetTileSize(){
		tileHeight = 1280 / 4;
		tileWidth = Screen.width * 1280 / Screen.height / 4;
		tileStartX = - tileWidth * 3 / 2;
		tileStartY = - tileHeight * 3 / 2;
	}
	

	void SetTilePositions(){
		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 6; j++) {
				tiles[i, j] = new Tile();
				tiles[i, j].position = new Vector3(tileStartX + tileWidth * i, tileStartY + tileHeight * (j - 1) , 0);
			}
		}
	}

	void SetTouchAreas(){
		touchArea0.transform.localPosition = new Vector3(tiles[0, 0].position.x, 0, 0);
		touchArea1.transform.localPosition = new Vector3(tiles[1, 0].position.x, 0, 0);
		touchArea2.transform.localPosition = new Vector3(tiles[2, 0].position.x, 0, 0);
		touchArea3.transform.localPosition = new Vector3(tiles[3, 0].position.x, 0, 0);

		touchArea0.transform.localScale = new Vector3(tileWidth, 1280, 1);
		touchArea1.transform.localScale = new Vector3(tileWidth, 1280, 1);
		touchArea2.transform.localScale = new Vector3(tileWidth, 1280, 1);
		touchArea3.transform.localScale = new Vector3(tileWidth, 1280, 1);

	}

	void SetFootprintPosition(){
		footprint.transform.localPosition = new Vector3(0, tiles[0, 1].position.y, 0);
	}

	GameObject GenerateNewTile(string type, int i, int j){
		GameObject generateTrans = null;
		if (type == "white"){
			generateTrans = tileWhite;
		} else if (type == "black"){
			generateTrans = tileBlack;
		} else if (type == "harbor"){
			generateTrans = tileHarbor;
		}

		GameObject createTile = Instantiate(generateTrans) as GameObject;
		createTile.transform.parent = gameView.transform;
		createTile.transform.localPosition = tiles[i, j].position;
		createTile.transform.localScale = new Vector3(1f, 1f, 1f);
		createTile.GetComponent<UI2DSprite>().width = (int) (tileWidth - 20);
		createTile.GetComponent<UI2DSprite>().height = (int) (tileHeight - 20);

		GameObject tileBack = createTile.transform.GetChild(0).gameObject;
		tileBack.GetComponent<UI2DSprite>().width = (int)tileWidth;
		tileBack.GetComponent<UI2DSprite>().height = (int)tileHeight;

		return createTile;
	}

	void GenerateNewTileLine(bool isHarbor, int j){
		int randInt = Random.Range(0, 4); 
		for (int i = 0; i < 4; i++){
			if (isHarbor){
				tiles[i, j].tileObject = GenerateNewTile("harbor", i, j);
			}
			else {
				if (i == randInt)
					tiles[i, j].tileObject = GenerateNewTile("black", i, j);
				else
					tiles[i, j].tileObject = GenerateNewTile("white", i, j);
			}
		}
	}

	void DestroyTileLine(int j){
		for (int i = 0; i < 4; i ++){
			Destroy(tiles[i, j].tileObject);
		}
	}

	void GenerateTiles(){
		for (int j = 0; j < 6; j++){
			if (j == 1){
				GenerateNewTileLine(true, 1);
			}
			else {
				GenerateNewTileLine(false, j);
			}
		}
	}
	
	
	void StepFoward(){
		DestroyTileLine(0);

		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 5; j++){
				tiles[i, j].tileObject = tiles[i, j + 1].tileObject;
			}

		}
		if (GameStates.step < (GameStates.TIME_ATTACK_STEPS - 4))
			GenerateNewTileLine(false, 5);
		else
			GenerateNewTileLine(true, 5);

		TilesMoveAnim();
		GameStates.step++;
		labelStep.GetComponent<UILabel>().text = GameStates.step.ToString();
	}

	void TilesMoveAnim(){
		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 6; j++){
				iTween.MoveTo(tiles[i, j].tileObject, iTween.Hash ("position", tiles[i, j].position, "time", 0.1f, "islocal", true, "easetype", "linear"));
			}
		}
	}

	bool CheckVictory(){
		if (GameStates.mode == 0){
			if (tiles[0, 2].tileObject.name == "TileHarbor(Clone)"){
				GameStates.victory = true;
				StartCoroutine("ShowVictory");
				isAnimating = true;
			} 
		} else if (GameStates.mode == 1){
			if (GameStates.time <= 0){
				GameStates.victory = true;
				StartCoroutine("ShowVictory");
				isAnimating = true;
			}
		} else if (GameStates.mode == 2){
			if (GameStates.time <= 0){
				GameStates.victory = true;
				StartCoroutine("ShowVictory");
				isAnimating = true;
			}
		} 
		return GameStates.victory;
	}
	

	void CheckTouch(){
//		if (Input.GetMouseButtonDown(0)){	
//			Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
			Ray ray = gameCamera.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)){
				switch (hit.collider.gameObject.name){
					case "TouchArea0": 
						TouchArea0();
						break;
					case "TouchArea1": 
						TouchArea1();
						break;
					case "TouchArea2": 
						TouchArea2();
						break;
					case "TouchArea3": 
						TouchArea3();
						break;
				}
			}
		}
	}

	void TouchEvent(int area){
		CheckStoppingJustFail();

		if (isAnimating)
			return;

		if (tiles[area, 2].tileObject.name != "TileBlack(Clone)"){
			GameStates.victory = false;
			AnimEnlarge(tiles[area, 2].tileObject);
			labelGameOver.SetActive(true);
			if (GameStates.victory){
				labelGameOver.GetComponent<UILabel>().text = "Safe";
				labelGameOver.GetComponent<UILabel>().color = Color.white;
			}
		} else {
			UpdateFootprint(area);
			StepFoward();
		}
	}

	void AnimEnlarge(GameObject tileObject){
		isAnimating = true;

		tileObject.GetComponent<UI2DSprite>().depth += 2;

		GameObject backObject = tileObject.transform.GetChild(0).gameObject;
		backObject.GetComponent<UI2DSprite>().depth += 2;

		iTween.ScaleTo(tileObject, iTween.Hash("time", 0.5f, "scale", new Vector3(1.5f, 1.5f, 0), "easetype", "linear", 
		                                       "oncomplete", "AnimShrink", "oncompletetarget", gameObject, "oncompleteparams", tileObject));
	}

	void AnimShrink(GameObject tileObject){
		iTween.ScaleTo(tileObject, iTween.Hash("time", 0.5f, "scale", new Vector3(1.0f, 1.0f, 0), "easetype", "linear", 
		                                       "oncomplete", "Die", "oncompletetarget", gameObject));
	}

	IEnumerator ShowVictory(){
		labelGameOver.SetActive(true);
		if (GameStates.victory){
			labelGameOver.GetComponent<UILabel>().text = "Safe";
			labelGameOver.GetComponent<UILabel>().color = Color.white;
		}
		yield return new WaitForSeconds(1.0f);
		Die();
	}

	void CheckStoppingJustFail(){
		if (GameStates.mode == 2 && isAnimating == true){
			GameStates.victory = false;
			labelGameOver.GetComponent<UILabel>().text = "Game Over!";
			labelGameOver.GetComponent<UILabel>().color = new Color(0.7305f, 0.043f, 0.043f);
		}
	}

	void CheckHideScore(){
		if (GameStates.mode == 2 && GameStates.time <= 5.0f)
			labelTime.SetActive(false);
	}

	void UpdateFootprint(int area){
		footprint.transform.localPosition = tiles[area, 1].position;
		footprint.transform.localEulerAngles = isLeftStep ? new Vector3(0, 180, 0) : Vector3.zero;
		isLeftStep = !isLeftStep;
	}

	void TouchArea0(){
		TouchEvent(0);
	}
	void TouchArea1(){
		TouchEvent(1);
	}
	void TouchArea2(){
		TouchEvent(2);
	}
	void TouchArea3(){
		TouchEvent(3);
	}

	void Die(){
		GameStates.state = 2;
		Application.LoadLevel("EndScene");
	}
	
}
