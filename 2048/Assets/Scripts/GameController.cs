using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	private int currentScore;
	private int highScore;


	public int direction; //0 for null, 1 for x+, 2 for y+, 3 for x-, 4 for y-;
	public Vector3[,] gridPositions;
	public GameObject[,] grids;

	public float probability4 = 0.2f;

	public GameObject[] gridOriginals;
	public List<GameObject> toBeDestroyedGOs;

	private Board board;
	private GameObject gameOver;
	private GameObject victory;
	private GameObject buttonRestart;
	private GameObject gameOverMask;
	private UILabel uiCurrentScore;
	private UILabel uiHighScore;

	private int gameState = 0; // 0 for touch, 1 for animating, 2 for dead or vic
	private bool isGenerate = false;

	void Awake(){
		gridPositions = new Vector3[4,4];
		grids = new GameObject[4,4];
		gridOriginals = new GameObject[11];

		for (int i = 0; i < 11; i++)
			gridOriginals[i] = Resources.Load("Prefabs/NumberGrids/" + Mathf.Pow(2f, (float)(i+1)).ToString()) as GameObject;

		board = GameObject.Find("UI Root/Board").GetComponent<Board>();
		gameOver = GameObject.Find("UI Root/Board/LabelGameOver");
		victory = GameObject.Find("UI Root/Board/LabelVictory");
		gameOverMask = GameObject.Find("UI Root/GameOverMask");
		uiCurrentScore = GameObject.Find("UI Root/Board/BGScore/LabelCurrentScore").GetComponent<UILabel>();
		uiHighScore = GameObject.Find("UI Root/Board/BGBest/LabelHighScore").GetComponent<UILabel>();
		buttonRestart = GameObject.Find("UI Root/Board/ButtonRestart");

		PluginManager.Instance.Init();
	}

	void Start(){

		gameOver.SetActive(false);
		victory.SetActive(false);
		gameOverMask.SetActive(false);
		buttonRestart.SetActive(false);

		GetGridPositions();
		currentScore = 0;
		LoadHighScore();
		UpdateScores();

		GenerateNewGrid();
		GenerateNewGrid();

		PluginManager.Instance.ShowBanner();
		PluginManager.Instance.CacheInterstitials();
	}
	

	void Update(){
		CheckBackButton();


		if (gameState != 0)
			return;

		if (GetTouch()){
			CheckMove();
			isGenerate = true;
		}
	}

	void GetGridPositions(){
		board.GenerateGridPositions();
	}





	bool GetTouch(){
		bool touched = false;

		if (Input.GetKeyDown(KeyCode.D)){
			direction = 1;
			touched = true;
		}
		else if (Input.GetKeyDown(KeyCode.W)){
			direction = 2;
			touched = true;
		}
		else if (Input.GetKeyDown(KeyCode.A)){
			direction = 3;
			touched = true;
		}
		else if (Input.GetKeyDown(KeyCode.S)){
			direction = 4;
			touched = true;
		}
		
		Vector2 touchDeltaPosition;

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved){
			touchDeltaPosition = Input.GetTouch(0).deltaPosition;

			if (touchDeltaPosition.magnitude >= 10f){
				touched = true;

				float x, y;
				x = touchDeltaPosition.x;
				y = touchDeltaPosition.y;
				if (Mathf.Abs(x) >= Mathf.Abs(y)){
					if (x >= 0)
						direction = 1;
					else
						direction = 3;
				}
				else{
					if (y >= 0)
						direction = 2;
					else
						direction = 4;
				}

			}
		}

		return touched;
	}

	void CheckMove(){
		bool isMove = false;

		switch (direction)
		{
		case 1:
			//for each grid
			for (int i = 2; i >= 0; i--){
				for (int j = 0; j < 4; j++)
				{
					//if null, do nothing
					if (grids[i, j] == null) continue;
					//else check move
					for (int k = i + 1; k < 4; k++){
						//empty
						if (grids[k, j] == null){
							grids[i, j].GetComponent<Grid>().isMove = true;
							grids[i, j].GetComponent<Grid>().targetX = k;
							grids[i, j].GetComponent<Grid>().targetY = j;
						}
						//emerge
						else if ( ((!grids[k, j].GetComponent<Grid>().isPredator) && (!grids[k, j].GetComponent<Grid>().isPrey)) && (grids[k, j].GetComponent<Grid>().number == grids[i, j].GetComponent<Grid>().number) ){
							grids[i, j].GetComponent<Grid>().isMove = true;
							grids[i, j].GetComponent<Grid>().isPredator = true;
							grids[k, j].GetComponent<Grid>().isPrey = true;
							grids[i, j].GetComponent<Grid>().targetX = k;
							grids[i, j].GetComponent<Grid>().targetY = j;

							toBeDestroyedGOs.Add(grids[k, j]);
						}
						else {
							break;
						}
					}
					if (grids[i, j].GetComponent<Grid>().isMove){
						DoMove(grids[i, j]);
					}
				}
			}
			break;
		case 2:
			for (int j = 2; j >= 0; j--){
				for (int i = 0; i < 4; i++)
				{
					if (grids[i, j] == null) continue;
					//else check move
					for (int k = j + 1; k < 4; k++){
						if (grids[i, k] == null){
							grids[i, j].GetComponent<Grid>().isMove = true;
							grids[i, j].GetComponent<Grid>().targetX = i;
							grids[i, j].GetComponent<Grid>().targetY = k;
						}
						else if ( ((!grids[i, k].GetComponent<Grid>().isPredator) && (!grids[i, k].GetComponent<Grid>().isPrey)) && (grids[i, k].GetComponent<Grid>().number == grids[i, j].GetComponent<Grid>().number) ){
							grids[i, j].GetComponent<Grid>().isMove = true;
							grids[i, j].GetComponent<Grid>().isPredator = true;
							grids[i, k].GetComponent<Grid>().isPrey = true;
							grids[i, j].GetComponent<Grid>().targetX = i;
							grids[i, j].GetComponent<Grid>().targetY = k;
							
							toBeDestroyedGOs.Add(grids[i, k]);
						}
						else {
							break;
						}
					}
					if (grids[i, j].GetComponent<Grid>().isMove){
						DoMove(grids[i, j]);
					}
				}
			}
			break;
		case 3:
			for (int i = 1; i  <= 3; i++){
				for (int j = 0; j < 4; j++)
				{
					if (grids[i, j] == null) continue;
					//else check move
					for (int k = i - 1; k >= 0; k--){
						if (grids[k, j] == null){
							grids[i, j].GetComponent<Grid>().isMove = true;
							grids[i, j].GetComponent<Grid>().targetX = k;
							grids[i, j].GetComponent<Grid>().targetY = j;
						}
						else if ( ((!grids[k, j].GetComponent<Grid>().isPredator) && (!grids[k, j].GetComponent<Grid>().isPrey)) && (grids[k, j].GetComponent<Grid>().number == grids[i, j].GetComponent<Grid>().number) ){
							grids[i, j].GetComponent<Grid>().isMove = true;
							grids[i, j].GetComponent<Grid>().isPredator = true;
							grids[k, j].GetComponent<Grid>().isPrey = true;
							grids[i, j].GetComponent<Grid>().targetX = k;
							grids[i, j].GetComponent<Grid>().targetY = j;
							
							toBeDestroyedGOs.Add(grids[k, j]);
						}
						else {
							break;
						}
					}
					if (grids[i, j].GetComponent<Grid>().isMove){
						DoMove(grids[i, j]);
					}
				}
			}
			break;
		case 4:
			for (int j = 1; j <= 3; j++){
				for (int i = 0;  i < 4; i++)
				{
					if (grids[i, j] == null) continue;
					//else check move
					for (int k = j - 1; k >= 0; k--){
						if (grids[i, k] == null){
							grids[i, j].GetComponent<Grid>().isMove = true;
							grids[i, j].GetComponent<Grid>().targetX = i;
							grids[i, j].GetComponent<Grid>().targetY = k;
						}
						else if ( ((!grids[i, k].GetComponent<Grid>().isPredator) && (!grids[i, k].GetComponent<Grid>().isPrey)) && (grids[i, k].GetComponent<Grid>().number == grids[i, j].GetComponent<Grid>().number) ){
							grids[i, j].GetComponent<Grid>().isMove = true;
							grids[i, j].GetComponent<Grid>().isPredator = true;
							grids[i, k].GetComponent<Grid>().isPrey = true;
							grids[i, j].GetComponent<Grid>().targetX = i;
							grids[i, j].GetComponent<Grid>().targetY = k;
							
							toBeDestroyedGOs.Add(grids[i, k]);
						}
						else {
							break;
						}
					}
					if (grids[i, j].GetComponent<Grid>().isMove){
						DoMove(grids[i, j]);
					}
				}
			}
			break;
		default:
			return;
			break;
		}
		
	}
	
	void DoMove(GameObject gridGameObject){
		gameState = 1;
		Grid grid = gridGameObject.GetComponent<Grid>();

		grids[grid.targetX, grid.targetY] = grids[grid.currentX, grid.currentY];
		grids[grid.currentX, grid.currentY] = null;
		
		iTween.MoveTo(gridGameObject, iTween.Hash("position", gridPositions[grid.targetX, grid.targetY], "islocal", true, "time", 0.1f,  "easetype", "linear", 
		                                          "oncomplete", "OnMoveComplete", "oncompletetarget", this.gameObject, "oncompleteparams", gridGameObject) );

		grid.isMove = false;

		UpdateCurrentPos(); 
	}

	
	void OnMoveComplete(GameObject gridGameObject){
		Grid grid = gridGameObject.GetComponent<Grid>();

		if (grid.isPredator){
			toBeDestroyedGOs.Add(gridGameObject);
			gridGameObject = CreateGrid(grid.number * 2, grid.currentX, grid.currentY);

			ScoresAdd(grid.number * 2);
		}
		DestroyGOs();

		if (isGenerate){
			GenerateNewGrid();
			isGenerate = false;
		}

		if (!CheckVictory()){
			if (!CheckDead()){
				StartCoroutine("WaitToTouch");
			}
		}
		

	}

	void GenerateNewGrid(){
		
		int i, j;
		do {
			i = Random.Range (0, 4);
			j = Random.Range (0, 4);
		}
		while (grids[i, j] != null);
		
		float tempF = Random.Range (0f, 1.0f);
		if (tempF < probability4)
			CreateGrid(4, i, j);
		else
			CreateGrid(2, i, j);

	}

//	void Generate2048(){
//		CreateGrid(2048, 0, 0);
//	}

	GameObject CreateGrid(int number, int i, int j){
		int pow = (int)Mathf.Log(number, 2);
		
		grids[i, j] = Instantiate(gridOriginals[pow - 1]) as GameObject;
		
		grids[i, j].transform.parent = board.transform;
		grids[i, j].transform.localPosition = gridPositions[i, j];
		grids[i, j].transform.localScale = new Vector3(0.5f, 0.5f, 1f);

		UpdateCurrentPos();

		iTween.ScaleTo(grids[i, j], iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.2f, "easetype", "easeOutBack"));

		return grids[i, j];
	}

	void DestroyGOs(){
		foreach (GameObject gameObject in toBeDestroyedGOs)
			Destroy(gameObject);

		toBeDestroyedGOs.Clear();
	}

	void UpdateCurrentPos(){
		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 4; j++){
				if (grids[i, j] != null){
					grids[i, j].GetComponent<Grid>().currentX = i;
					grids[i, j].GetComponent<Grid>().currentY = j;
					
				}
			}
		}
	}

	void ResetGridBools(){
		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 4; j++){
				if (grids[i, j] != null){
					grids[i, j].GetComponent<Grid>().isPredator = false;
					grids[i, j].GetComponent<Grid>().isPrey = false;
					grids[i, j].GetComponent<Grid>().isMove = false;
					
				}
			}
		}

	}

	bool CheckVictory(){
		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 4; j++){
				if (grids[i, j] != null && grids[i, j].GetComponent<Grid>().number == 2048){

					victory.SetActive(true);
					buttonRestart.SetActive(true);
					gameOverMask.SetActive(true);
					gameState = 2;
					return true;
				}
			}
		}
		return false;
	}

	bool CheckDead(){
		bool isFull = true;
		bool isNoMove = true;

		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 4; j++){

				if (grids[i, j] == null){
					isFull = false;
					break;
				}

				if (i < 3){
					if ( (grids[i + 1, j] != null) && (grids[i + 1, j].GetComponent<Grid>().number == grids[i, j].GetComponent<Grid>().number) ){
						isNoMove = false;
						break;
					}
				}

				if (j < 3){
					if ( (grids[i, j + 1] != null) && (grids[i, j + 1].GetComponent<Grid>().number == grids[i, j].GetComponent<Grid>().number) ){
						isNoMove = false;
						break;
					}
				}

			}
		}

		//if dead
		if (isFull && isNoMove){
			gameOver.SetActive(true);
			buttonRestart.SetActive(true);
			gameOverMask.SetActive(true);

			PluginManager.Instance.ShowInterstitialInGame();
			gameState = 2;
			return true;
		}
		else
			return false;

	}

	void ScoresAdd(int increment){
		currentScore += increment;
		if (highScore < currentScore){
			highScore = currentScore;
			SaveHighScore();
		}

		UpdateScores();
	}

	void UpdateScores(){
		uiCurrentScore.text = currentScore.ToString();
		uiHighScore.text = highScore.ToString();
	}

	void SaveHighScore(){
		PlayerPrefs.SetInt("highScore", highScore);
		PlayerPrefs.Save();
	}

	void LoadHighScore(){
		highScore = PlayerPrefs.GetInt("highScore");
	}

	public void OnButtonRestart(){
		Application.LoadLevel("GameScene");
	}

	void CheckBackButton(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			PluginManager.Instance.ShowInterstitialOnQuit();
		}
	}

	IEnumerator WaitToTouch(){
		yield return new WaitForSeconds(0.2f);
		gameState = 0;
	}
	
	
}
