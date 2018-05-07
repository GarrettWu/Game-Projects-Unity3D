using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	private Transform earthTrans; 

	// Use this for initialization
	void Awake () {
		earthTrans = GameObject.Find("Earth").transform;
	}
	
	// Update is called once per frame
	void Update () {
		float earthY = earthTrans.position.y;
		if (transform.position.y < earthY - 10f) {
			Destroy(gameObject);
		}
	}


}
