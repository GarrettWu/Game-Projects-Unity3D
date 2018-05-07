using UnityEngine;
using System.Collections;

public class Hammer : MonoBehaviour {

	public GameObject pivot;

	private Vector3 pivotPos;

	void Awake () {
	}

	// Use this for initialization
	void Start () {
		
	}

	void GetPivotPos() {
		pivotPos = pivot.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		GetPivotPos();
		transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pivotPos.y - transform.position.y, pivotPos.x - transform.position.x) * Mathf.Rad2Deg - 90f);
	}

	void Passed () {
		collider2D.isTrigger = true;
	}
}
