using UnityEngine;
using System.Collections;

public class Train : MonoBehaviour {
	
	public int speed = 1;
	public bool isRun = false;

	// Use this for initialization
	void Start () {
		print ("--- Train is start");
	}
	
	// Update is called once per frame
	void Update () {
		if(isRun){
			float amtToMove = speed * Time.deltaTime;
        	transform.Translate(Vector3.back*amtToMove);
		}
	}
}
