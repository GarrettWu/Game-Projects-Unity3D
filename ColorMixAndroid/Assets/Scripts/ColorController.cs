using UnityEngine;
using System.Collections;

public class ColorController : MonoBehaviour {

	SpriteRenderer spriteRenderer;
	Animator animator;

	public Color objectColor = Color.white;

	public int color = 0;

	public bool redKey = false;
	public bool yellowKey = false;
	public bool blueKey = false;

	GameObject redPower;
	GameObject yellowPower;
	GameObject bluePower;

	Vector2 redPosition, yellowPosition, bluePosition;
	private float colorRadius = 0.7f;

	GameController gameController;
	GameObject explosion;
	
	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();

		redPower = GameObject.Find("Colors/Red/Power");
		yellowPower = GameObject.Find("Colors/Yellow/Power");
		bluePower = GameObject.Find("Colors/Blue/Power");

		gameController = Camera.main.GetComponent<GameController>();

		redPosition = GameObject.Find ("Colors/Red").transform.position;
		yellowPosition = GameObject.Find ("Colors/Yellow").transform.position;
		bluePosition = GameObject.Find ("Colors/Blue").transform.position;

		explosion = Resources.Load("Prefabs/Explosion") as GameObject;

	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		ResetColors();

		CheckInput();
		CheckTouchInput();

		ColorChange();
		PowerControl();
	}

	void ResetColors(){
		redKey = false;
		yellowKey = false;
		blueKey = false;
	}

	void CheckInput(){
		if (Input.GetKey(KeyCode.A)){
			redKey = true;
		} else {
			redKey = false;
		}

		if (Input.GetKey(KeyCode.S)){
			yellowKey = true;
		} else {
			yellowKey = false;
		}

		if (Input.GetKey(KeyCode.D)){
			blueKey = true;
		} else {
			blueKey = false;
		}

	}

	void CheckTouchInput(){
		int touchCount = Input.touchCount;

		if (touchCount > 0){
			for (int i = 0; i < touchCount; i++){
//				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
//				RaycastHit hit;
//
//				if (Physics.Raycast(ray, out hit)){
//
//				}

				Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
				if (Vector2.Distance(touchPosition, redPosition) < colorRadius){
					redKey = true;
				}

				if (Vector2.Distance(touchPosition, yellowPosition) < colorRadius){
					yellowKey = true;
				}

				if (Vector2.Distance(touchPosition, bluePosition) < colorRadius){
					blueKey = true;
				}

			}
		}
	}

	void ColorChange(){
		if (redKey){
			if (yellowKey){
				if (blueKey){
					objectColor = GameController.black;
					color = 7;
				} else {
					objectColor = GameController.orange;
					color = 4;
				}
			} else {
				if (blueKey) {
					objectColor = GameController.purple;
					color = 6;
				} else {
					objectColor = GameController.red;
					color = 1;
				}
			}
		} else {
			if (yellowKey){
				if (blueKey){
					objectColor = GameController.green;
					color = 5;
				} else {
					objectColor = GameController.yellow;
					color = 2;
				}
			} else {
				if (blueKey) {
					objectColor = GameController.blue;
					color = 3;
				} else {
					objectColor = GameController.white;
					color = 0;
				}
			}
		}

		spriteRenderer.color = objectColor;
	}

	void PowerControl(){
		if (redKey){
			redPower.SetActive(true);
		} else {
			redPower.SetActive(false);
		}

		if (yellowKey){
			yellowPower.SetActive(true);
		} else {
			yellowPower.SetActive(false);
		}

		if (blueKey){
			bluePower.SetActive(true);
		} else {
			bluePower.SetActive(false);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals("Enemy")){
			int enemyColor = other.GetComponent<EnemyController>().color;

			if (color == enemyColor){
				Score();
			} else {
				Explosion();
				gameController.StartWaitEnd();

			}
			Destroy(other.gameObject);
		}
	}

	void Explosion(){
		Instantiate(explosion, transform.position, Quaternion.identity);

		SoundManager.Instance.PlayAudio("explosion");
	}

	void Score(){
		int id = Animator.StringToHash("Absorb");
		animator.SetTrigger(id);

		SoundManager.Instance.PlayAudio("pop");

		gameController.ScoreAdd();
	}
}
