using UnityEngine;
using System.Collections;

public class GroundGenerator : MonoBehaviour {

	private float GroundLength = 3.35f;

	private GameObject bird;

	void Awake(){
		bird = GameObject.Find("Bird");
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (transform.position.x < (bird.transform.position.x - GroundLength) )
		{
			transform.Translate(new Vector3 (GroundLength * 3, 0, 0) );
		}

	
	}
}
