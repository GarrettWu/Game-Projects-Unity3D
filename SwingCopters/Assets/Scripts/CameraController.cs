using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private Transform playerTrans;

	void Awake(){
		playerTrans = GameObject.Find("Player").transform;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = new Vector3(0, playerTrans.position.y + 2.9f, -10.0f);
	}
}
