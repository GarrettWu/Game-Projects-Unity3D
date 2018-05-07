using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		iTween.RotateBy(gameObject, iTween.Hash("time", 2.0f, "oncomplete", "SelfDestroy", "easetype", "linear", "amount", new Vector3(0, 0, -0.5f)));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SelfDestroy(){
		Destroy(gameObject);
	}
}
