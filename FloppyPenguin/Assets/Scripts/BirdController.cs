using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour {

	public float speedHorizontal;
	public float gravity;
	public float jumpSpeed;
	public float jumpAngle;
	public float diveSpeedThreshold;
	public float diveAngularSpeed;
	public float diveAngularAcc;

	private float currentYSpeed = 0;
	private float currentAngularSpeed = 0;

	private bool dead = false;
	private bool grounded = false;

	private GameController gameController;
	private Animator birdAnimator;
	private UIController uiController;
	private AudioController audioController;

	void Awake(){
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		birdAnimator = gameObject.GetComponent<Animator>();
		uiController = GameObject.Find("UIController").GetComponent<UIController>();
		audioController = GameObject.Find("AudioController").GetComponent<AudioController>();

	}
	// Use this for initialization
	void Start () {
		birdAnimator.SetBool("dive", false);
		uiController.ScoreUpdate();
		iTween.MoveBy(gameObject, iTween.Hash("time", 0.3, "y", 0.06, "easeType", "easeInOutSine","loopType", "pingPong", "delay", 0));
	}
	
	// Update is called once per frame
	void Update () {
		CalculateSpeed();

		if (dead){
			if (!grounded)
				ApplyYSpeed();

			return;
		}


		CheckJump();
		MoveHorizontal();

		if (GameController.gameState == 0)
			return;

		CheckDive();
		ApplyYSpeed();

	}

	void CalculateSpeed(){
		currentYSpeed -= gravity*Time.deltaTime;
		currentAngularSpeed += diveAngularAcc;
	}

	void ApplyYSpeed(){

		transform.Translate(new Vector3(0, currentYSpeed, 0), Space.World);
	}
	

	void MoveHorizontal(){
		transform.Translate (new Vector3(speedHorizontal * Time.deltaTime, 0, 0), Space.World);
	}

	bool CheckJump(){
		if (Input.GetKeyDown(KeyCode.Space) || ( (Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began) ) )
		{
			Jump();
			if (GameController.gameState == 0){
				iTween.Stop(this.gameObject);
				gameController.Gaming();
			}
			return true;
		}
		return false;
	}

	bool CheckDive(){
		if (currentYSpeed <= diveSpeedThreshold){
			Dive ();
			return true;
		}
		return false;
	}

	void Jump(){
		birdAnimator.speed = 1.8f;
		currentYSpeed = jumpSpeed;
		currentAngularSpeed = 0;
		transform.eulerAngles = new Vector3(0, 0, jumpAngle);
		birdAnimator.SetBool("dive", false);

		audioController.PlayAudio("wing");
	}

	void Dive(){
		birdAnimator.SetBool("dive", true);
		//angle
		if (transform.eulerAngles.z > 270.0f || transform.eulerAngles.z < 90.0f)
		{
			transform.eulerAngles -= new Vector3(0, 0, currentAngularSpeed * Time.deltaTime);

		}
		else
		{
			transform.eulerAngles = new Vector3(0, 0, 270.0f);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Enemy"){
			Die();
			audioController.PlayAudio("die");
		}
		else if (coll.gameObject.tag == "Ground"){
			grounded = true;
			if (!dead)
				Die();

			gameController.Ending ();
		}


	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Pass"){
			GameController.score++;
			uiController.ScoreUpdate();

			audioController.PlayAudio("point");
		}
	}

	void Die(){
		transform.eulerAngles = new Vector3(0, 0, 270f);
		birdAnimator.SetBool("dive", true);

		foreach (GameObject i in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			i.collider2D.isTrigger = true ;
		}

		dead = true;

		audioController.PlayAudio("hit");
	}
	
}
