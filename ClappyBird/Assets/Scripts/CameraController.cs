using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private GameObject bird;
	public float offsetX;

	void Awake(){
		bird = GameObject.Find("Bird");

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = new Vector3(bird.transform.position.x + offsetX , transform.position.y, transform.position.z);
	}
}
