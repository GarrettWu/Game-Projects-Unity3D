using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour {


	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -10){
			Destroy(gameObject);
		}
	}
}
