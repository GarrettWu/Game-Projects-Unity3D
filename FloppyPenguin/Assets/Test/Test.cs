using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {


	void Awake(){
		Debug.Log("awake");
		DontDestroyOnLoad(this);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)){
			Application.LoadLevel("Test");
		}
	}

	void OnTriggerEnter2D(Collider2D other){

		Debug.Log ("hit");
	}
}
