using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	
	private Transform playerTrans;
	private bool magneted = false;
	private float magnetAcc = 20.0f;
	private float magnetSpeed = 0;
	
	// Use this for initialization
	void Start () 
	{
		playerTrans = GameObject.Find("Player").transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(new Vector3(0, 180.0f, 0) * Time.deltaTime);
		
		MagnetUpdate();
	}
	
	void MagnetUpdate()
	{
		if (!magneted)
			return;
		
		Vector3 moveVec;
		moveVec = playerTrans.position - transform.position;	
		
		moveVec.Normalize();
		
		magnetSpeed += magnetAcc * Time.deltaTime;
		transform.Translate(moveVec * magnetSpeed * Time.deltaTime, Space.World); 
		
	}
	
	public void MagnetActivate()
	{
		magneted = true;
	}
}
