using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour {
	public float scaleMin, scaleMax;
	public float speedMin, speedMax;
	public float intervalMin, intervalMax;

	private Color backColorRed = new Color(228/255f, 43/255f, 25/255f);
	private Color backColorYellow = new Color(238/255f, 237/255f, 16/255f);
	private Color backColorBlue = new Color(24/255f, 80/255f, 227/255f);


	private float edge;
	private float interval;
	private float ballScale;
	private float ballSpeed;
	private Vector2 ballPosition;

	private Color ballColor;
	
	private GameObject backBall;
	private GameObject theBall;
	private GameObject background1;


	void Awake(){
		backBall = Resources.Load("Prefabs/BackBall") as GameObject;
		background1 = GameObject.Find("Background1");
	}
	// Use this for initialization
	void Start () {
		edge = 960 / 100f / 2f;

		StartCoroutine("WaitToGenerate");
		RandomBackColor();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RandomBackColor(){
		int backColorNum = Random.Range(0, 3);
		switch (backColorNum) 
		{
		case 0: iTween.ColorTo(gameObject, backColorRed, 2.0f); 
				iTween.ColorTo(background1, backColorRed, 2.0f);
			break;
		case 1: iTween.ColorTo(gameObject, backColorYellow, 2.0f); 
				iTween.ColorTo(background1, backColorYellow, 2.0f); 
			break;
		case 2: iTween.ColorTo(gameObject, backColorBlue, 2.0f); 
				iTween.ColorTo(background1, backColorBlue, 2.0f); 
			break;
		}
	}

	IEnumerator WaitToGenerate(){
		RandomInterval();
		yield return new WaitForSeconds(interval);

		GenerateNewBackBall();
		StartCoroutine("WaitToGenerate");
	}

	void GenerateNewBackBall(){
		RandomScale();
		RandomSpeed();
		RandomPosition();
		RandomColor();

		theBall = Instantiate(backBall, ballPosition, Quaternion.identity) as GameObject;
		theBall.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
		theBall.rigidbody2D.velocity = new Vector2(0, -ballSpeed);
		theBall.GetComponent<SpriteRenderer>().color = ballColor;
	}

	void RandomInterval(){
		interval = Random.Range (intervalMin, intervalMax);
	}

	void RandomScale(){
		ballScale = Random.Range (scaleMin, scaleMax);
	}

	void RandomSpeed(){
		ballSpeed = Random.Range (speedMin, speedMax);
	}

	void RandomPosition(){
		ballPosition = new Vector2(Random.Range(-edge, edge), 8.0f);
	}

	void RandomColor(){
		int colorNum = Random.Range(0, 8);
		switch (colorNum){
		case 0:
			ballColor = GameController.white; break;
		case 1:
			ballColor = GameController.red; break;
		case 2:
			ballColor = GameController.yellow; break;
		case 3:
			ballColor = GameController.blue; break;
		case 4:
			ballColor = GameController.orange; break;
		case 5:
			ballColor = GameController.green; break;
		case 6:
			ballColor = GameController.purple; break;
		case 7:
			ballColor = GameController.black; break;
		}
	}
}
