using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float verticalSpeed;
	public float forceHorizontal;

	private bool isLeft = false;
	private int playerState = 0; //0 idle; 1 idle lookup; 2 prepare; 3 fly; 4 fall; 5 ground
	private float blinkGap = 3.0f;
	private float idleTime = 4.0f;
	private float prepareTime = 2.5f;
	private float rotateSpeed = 360f;

	private GameObject propeller;
	private Propeller propellerScript;
	private Animator animator;

	void Awake(){
		propeller = GameObject.Find("Player/Propeller");
		propellerScript = propeller.GetComponent<Propeller>();
//		blade0 = GameObject.Find("Player/Blade0");
//		blade1 = GameObject.Find("Player/Blade1");
		animator = GetComponent<Animator>();
	}
	// Use this for initialization
	void Start () {
		Idle();
	}
	
	// Update is called once per frame
	void Update () {
		if (playerState == 0 || playerState == 1){
			CheckStartTouch();
		} else if (playerState == 3){
			CheckTouch();
			CheckDirection();
		}
	}

	void FixedUpdate(){
		if (playerState == 3)
			AddHorizontalForce();
	}
	
	void CheckStartTouch(){
		if (Input.GetKeyDown(KeyCode.Space)){
			Prepare();
		}
	}

	void CheckTouch(){
		if (Input.GetKeyDown(KeyCode.Space)){
			isLeft = !isLeft;
		}
	}

	void CheckDirection(){
		if (isLeft) {
			transform.eulerAngles = new Vector3(0, 180f, 0);
		} else {
			transform.eulerAngles = Vector3.zero;
		}
	}

	void AddHorizontalForce(){
		if (isLeft){
			rigidbody2D.AddForce(new Vector2(-forceHorizontal, 0));
		} else {
			rigidbody2D.AddForce(new Vector2(forceHorizontal, 0));
		}
	}

	void Idle(){
		playerState = 0;
		animator.SetBool("lookup", false);
		StartCoroutine("WaitForBlink");
		StartCoroutine("WaitForLookup");

		propellerScript.Idle();
	}

	void IdleLookup(){
		playerState = 1;
		animator.SetBool("lookup", true);
	}

	void Prepare(){
		playerState = 2;
		animator.SetBool("lookup", true);
		StartCoroutine("WaitForFly");
		StopCoroutine("WaitForLookup");

		propellerScript.Prepare();
	}

	void Fly(){
		playerState = 3;
		animator.SetBool("lookup", true);
		StopCoroutine("WaitForBlink");
		rigidbody2D.velocity = new Vector2(0, verticalSpeed);

		propellerScript.Fly();
	}

	public void Fall(bool isCollLeft){
		if (playerState == 4)
			return;

		playerState = 4;
		animator.SetBool("lookup", false);
		StartCoroutine("WaitForBlink");

		rigidbody2D.velocity = new Vector3(isLeft ? 1.0f : -1.0f, 0, 0);
		rigidbody2D.gravityScale = 2.0f;
		propeller.SetActive(false);

		float rotateValue = isCollLeft ? -1f : 1f;
		iTween.RotateBy(gameObject, iTween.Hash("time", 0.5f, "z", rotateValue, "looptype", "loop", "easetype", "linear", "name", "playerRotate"));
	
		propellerScript.Fall();
	}
	
	void Ground() {
		playerState = 5;

		iTween.StopByName("playerRotate");
		animator.SetBool("lookup", false);
		rigidbody2D.velocity = Vector3.zero;
		transform.eulerAngles = new Vector3(0, 0, 180);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Enemy"){
			if (coll.contacts[0].point.x <= transform.position.x)
				Fall (true);
			else
				Fall (false);
		} else if (coll.gameObject.tag == "Ground"){
			Ground ();
		}
	}

	void PeriodicalBlinkAnim(bool isLookup){
		animator.SetBool("lookup", isLookup);
		StartCoroutine("WaitForBlink");
	}

	IEnumerator WaitForLookup(){
		yield return new WaitForSeconds(idleTime);
		if (playerState == 1)
			IdleLookup();
	}

	IEnumerator WaitForFly(){
		yield return new WaitForSeconds(prepareTime);
		Fly ();
	}

	IEnumerator WaitForBlink(){
		yield return new WaitForSeconds(blinkGap);
		animator.SetBool("blink", true);
		StartCoroutine("WaitForBlink");
	}



	void BlinkEnd(){
		animator.SetBool("blink", false);
	}



}
