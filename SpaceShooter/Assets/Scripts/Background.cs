using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

//	public float planetSizeMin, planetSizeMax;
//	public int planetNumMin, planetNumMax;
//	public float starLargeSizeMin, starLargeSizeMax;
//	public int starLargeNumMin, starLargeNumMax;
//	public int starSmallNumMin, starSmallNumMax;
//
//	private GameObject planet0, planet1;
//	private GameObject starLarge, starSmall;
//	private GameObject background;

	private GameObject player;
	private PlayerController playerController;
	private float playerVelocity;
	private Vector2 relativeMousePos;
	private Vector2 direction;

	private float backHeight = 25.55f;//minus from 25.6
	private float backWidth = 19.30f;//19.34

	void Awake(){
//		planet0 = Resources.Load("Prefabs/Planet0") as GameObject;
//		planet1 = Resources.Load("Prefabs/Planet1") as GameObject;
//
//		starLarge = Resources.Load("Prefabs/StarLarge") as GameObject;
//		starSmall = Resources.Load("Prefabs/StarSmall") as GameObject;
		player = GameObject.Find("Player");
		playerController = player.GetComponent<PlayerController>();
		playerVelocity = playerController.velocity;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		CheckToMove();
		CheckPos();
	}

	void CheckToMove(){
		if (GameController.gameState != 1){
			return;
		}

		relativeMousePos = playerController.relativeMousePosV2;

		direction = - relativeMousePos.normalized;
		transform.Translate(direction * playerVelocity * Time.deltaTime);
	}

	void CheckPos(){
		if (transform.position.x > 2 * backWidth){
			transform.Translate(new Vector3(- 3 * backWidth, 0, 0));
		} else if(transform.position.x < - 2 * backWidth){
			transform.Translate(new Vector3(3 * backWidth, 0, 0));
		} else if(transform.position.y > 2 * backHeight){
			transform.Translate(new Vector3(0, - 3 * backHeight, 0));
		} else if(transform.position.y < -2 * backHeight){
			transform.Translate(new Vector3(0, 3 * backHeight, 0));
		}
	}
}
