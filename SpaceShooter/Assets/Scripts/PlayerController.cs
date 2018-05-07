using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float bulletVelocity;
	public float powerTime;
	public float velocity;

	private float playerAngle = 0;

	public Vector2 relativeMousePosV2;

	private GameObject bulletPrefab;
	private bool isCharge = false;

	private Vector3 firePos;
	private GameObject explosion;
	private GameObject charge;
	private Animator chargeAnim;
	private GameController gameController;
	private UIController uiController;

	void Awake(){
		bulletPrefab = Resources.Load("Prefabs/bullet") as GameObject;
		explosion = Resources.Load("Prefabs/Explosion") as GameObject;

		charge = GameObject.Find("Player/Charge");
		chargeAnim = charge.GetComponent<Animator>();

		gameController = Camera.main.GetComponent<GameController>();
		uiController = GameObject.Find("Canvas").GetComponent<UIController>();
	}
	// Use this for initialization
	void Start () {
		charge.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		CheckInput();
	}

	void CheckInput(){
//		Vector3 centerPoint = new Vector3(Screen.width/2, Screen.height/2, 0);
		Vector3 relativeMousePosition = Vector3.zero;

		//face to
		if (Input.touchCount > 0){
			relativeMousePosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

//			relativeMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			playerAngle = -Mathf.Atan2(relativeMousePosition.x, relativeMousePosition.y) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, 0, playerAngle);
			relativeMousePosV2 = relativeMousePosition;

			//fire
			if (Input.GetTouch(0).phase == TouchPhase.Began){
				ChargeBegin();
				uiController.HideTapRelease();
			}

			if (Input.GetTouch(0).phase == TouchPhase.Ended){
				if (isCharge){
					Fire(relativeMousePosition);
				}
				
				ChargeOff();
			}
		}






	}

	void Fire(Vector3 targetPos){
		firePos = charge.transform.position;

		GameObject bullet = Instantiate(bulletPrefab, firePos, Quaternion.identity) as GameObject;

		Vector2 velocityVec2 = new Vector2(targetPos.x, targetPos.y).normalized * bulletVelocity;
		bullet.rigidbody2D.velocity = velocityVec2;

		float angle = -Mathf.Atan2(bullet.rigidbody2D.velocity.x, bullet.rigidbody2D.velocity.y) * Mathf.Rad2Deg;
		bullet.transform.eulerAngles = new Vector3(0, 0, angle);

		SoundManager.Instance.PlayAudio("shoot");

	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag.Equals("Enemy")){
			Instantiate(explosion, transform.position, Quaternion.identity);
			Instantiate(explosion, other.transform.position, Quaternion.identity);

			Destroy(other.gameObject);
			Die ();
			SoundManager.Instance.PlayAudio("explosion");

		}
	}

	void ChargeBegin(){
		StartCoroutine("Charging");
		charge.SetActive(true);
		SoundManager.Instance.PlayAudio("charge");

	}

	void ChargeReady(){
		isCharge = true;
		chargeAnim.SetBool("isCharge", true);
		SoundManager.Instance.PlayAudio("power");

	}

	void ChargeOff(){
		StopCoroutine("Charging");
		isCharge = false;
		charge.SetActive(false);
		SoundManager.Instance.StopAudio("power");

	}

	IEnumerator Charging() {
		yield return new WaitForSeconds(powerTime);
		ChargeReady();
	}

	void Die(){
		ChargeOff();
		gameController.StartWaitEnd();
		gameObject.SetActive(false);

		relativeMousePosV2 = Vector2.zero;
	}



}
