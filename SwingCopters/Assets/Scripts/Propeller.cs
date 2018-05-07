using UnityEngine;
using System.Collections;

public class Propeller : MonoBehaviour {

	public GameObject blade;

	PlayerController playerController;
	GameObject blade0, blade1;
	float bladeForceX = 200f;
	float bladeForceY = 120f;

	Animator animator;

	float prepareWaitTime = 1.0f;

	void Awake(){
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		animator = GetComponent<Animator>();
	}

	void Start(){
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Enemy"){
			if (coll.contacts[0].point.x <= transform.position.x)
				playerController.Fall (true);
			else
				playerController.Fall (false);
		}
	}

	public void Idle(){

	}

	public void Prepare(){
		StartCoroutine("WaitForPrepareAnim");
	}

	public void Fly(){
		animator.SetBool("fly", true);
	}

	public void Fall(){
		blade0 = Instantiate(blade, transform.position - new Vector3(0.45f, 0, 0), Quaternion.Euler(0, 180f, 90f)) as GameObject;
		iTween.RotateBy(blade0, iTween.Hash("time", 0.1f, "z", -1f, "looptype", "loop", "easetype", "linear"));
		blade0.rigidbody2D.AddForce(new Vector3(-bladeForceX, bladeForceY, 0));

		blade1 = Instantiate(blade, transform.position + new Vector3(0.45f, 0, 0), Quaternion.Euler(0, 0, 90f)) as GameObject;
		iTween.RotateBy(blade1, iTween.Hash("time", 0.1f, "z", -1f, "looptype", "loop", "easetype", "linear"));
		blade1.rigidbody2D.AddForce(new Vector3(bladeForceX, bladeForceY, 0));
	}

	IEnumerator WaitForPrepareAnim(){
		yield return new WaitForSeconds(prepareWaitTime);
		animator.SetBool("prepare", true);
	}
}
