using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {

	private float maxDistance = 20.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate(){
		float boundaryY = Camera.main.transform.position.y - maxDistance;
		if (transform.position.y < boundaryY)
			transform.position = new Vector3(0, boundaryY, 0);
	}
}
