using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	
	//consts
	public float cameraDistance = 8.0f;
	public float cameraHeight = 2.6f;
	public float watchPointHeight = 4.3f;
	public float rotationDamping = 4.0f;
	public float heightDamping = 4.0f;
	
	private float playerUpLimit = 5.0f;
	private float playerDownLimit = 4.0f;
	private float playerLeftRightLimit = 2.0f;
	
	//references
	private Transform playerTrans;
	private PlayerController playerController;
	private CharacterController controller;
	private Transform watchPoint;
	
	//vars
	private int Direction;
	private float normalPosWantedHeight;
	Vector3 playerNormalPos = Vector3.zero; 	
	
	
	void Awake()
	{
		//initiate reference
		playerTrans = GameObject.Find("Player").transform;
		playerController = playerTrans.GetComponent<PlayerController>();
		controller = playerTrans.GetComponent<CharacterController>();
		watchPoint = GameObject.Find("WatchPoint").transform;
		
	}
	
	void Start()
	{

		//set normal position H
		normalPosWantedHeight = 0.2f;
	}
	
	
	void LateUpdate () 
	{
		// Early out if we don't have a playerTrans
		if (!playerTrans)
			return;
		
		//get Direction
		Direction = playerController.GetDirection();
		
		//determine watchPoint.position
		watchPoint.position = GetPlayerNormalPos() + new Vector3(0, watchPointHeight, 0);
		
		//offset watchPoint if player is too far away
		WatchPointPlayerPosOffset(); 
		
		//set camera position (rotation damping)
		float wantedRotationAngle = playerTrans.eulerAngles.y;
		float currentRotationAngle = transform.eulerAngles.y;
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		transform.position = watchPoint.position;
		transform.position -= currentRotation * Vector3.forward * cameraDistance;
		
		//add camera height
		transform.position += new Vector3(0, cameraHeight, 0);
		
		transform.LookAt (watchPoint.position);
	}
	
	public Vector3 GetPlayerNormalPos()
	{
		Vector3 path2Pos = playerController.GetPath2Pos();
		
		switch (Direction)
		{
			case 0:
			playerNormalPos = new Vector3(path2Pos.x, playerNormalPos.y, playerTrans.position.z);
			break;
			
			case 1:
			playerNormalPos = new Vector3(playerTrans.position.x, playerNormalPos.y, path2Pos.z);
			break;
			
			case 2:
			playerNormalPos = new Vector3(path2Pos.x, playerNormalPos.y, playerTrans.position.z);
			break;
			
			case 3:
			playerNormalPos = new Vector3(playerTrans.position.x, playerNormalPos.y, path2Pos.z);
			break;
		}		
		
		float playerFootHeight = playerTrans.position.y - controller.height/2 * playerTrans.localScale.y;
		
		
		if (controller.isGrounded)
		{
			normalPosWantedHeight = playerFootHeight;
		}
		
		float normalPosCurrentHeight = playerNormalPos.y;

		normalPosCurrentHeight = Mathf.Lerp (normalPosCurrentHeight, normalPosWantedHeight, heightDamping * Time.deltaTime);

		playerNormalPos = new Vector3(playerNormalPos.x, normalPosCurrentHeight, playerNormalPos.z);

		
		return playerNormalPos;
	}
	
	void WatchPointPlayerPosOffset()
	{
		watchPoint.rotation = playerTrans.rotation;
		
		Vector3 playerRelativePos = watchPoint.InverseTransformPoint(playerTrans.position);
		
		float tempFloat;
		
		tempFloat = playerRelativePos.y - playerUpLimit;
		if (tempFloat > 0)
		{
			watchPoint.Translate(new Vector3(0, tempFloat, 0), Space.Self);
		}
		
		tempFloat = playerRelativePos.y + playerDownLimit;
		if (tempFloat < 0)
		{
			watchPoint.Translate(new Vector3(0, tempFloat, 0), Space.Self);
		}
		
		tempFloat = playerRelativePos.x + playerLeftRightLimit;
		if (tempFloat < 0)
		{
			watchPoint.Translate(new Vector3(tempFloat, 0, 0), Space.Self);
		}
		tempFloat = playerRelativePos.x - playerLeftRightLimit;
		if (tempFloat > 0)
		{
			watchPoint.Translate(new Vector3(tempFloat, 0, 0), Space.Self);
		}
	}
}