using UnityEngine;
using System.Collections;

public class Turninglistener : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isInCell){
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
//				print ("--GetKeyDown: RightArrow");
				CellGenerator.instance.rotateCells(gameObject.transform.parent.parent.gameObject);
				CellGenerator.instance.cellModel_cornerT = null;
			}
		}
	}
	
	/*
	void OnTriggerStay(Collider otherObject) {
		if(otherObject.tag == "Player"){
//			if (!Input.GetKeyDown (KeyCode.RightArrow)) {
//				return;
//			}
//			CellGenerator.instance.rotateCells(gameObject.transform.parent.gameObject);
//			CellGenerator.instance.cellModel_cornerT = null;
			
			print ("OnTriggerStay");
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				print ("--GetKeyDown: RightArrow");
				CellGenerator.instance.rotateCells(gameObject.transform.parent.gameObject);
				CellGenerator.instance.cellModel_cornerT = null;
			}
			
		}
	}
	*/
	
	bool isInCell = false;
	
	void OnTriggerEnter(Collider otherObject) {
		isInCell = true;
	}
	
	void OnTriggerExit(Collider otherObject) {
		isInCell = false;
	}
	
	
}
