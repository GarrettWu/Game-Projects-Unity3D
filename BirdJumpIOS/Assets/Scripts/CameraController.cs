using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private Transform playerTrans;
	private Transform cameraLeftTrans;
	private Transform cameraRightTrans;

    public float yUpperLimit = 0;
    public float yLowerLimit = 0;


	// Use this for initialization

	void Awake()
	{
		playerTrans = GameObject.Find("RootGame/Player").transform;
		cameraLeftTrans = GameObject.Find("RootGame/CameraLeft").transform;
		cameraRightTrans = GameObject.Find("RootGame/CameraRight").transform;

	}

	void Start () {
		SetSideCameras();
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if (GameController.gameState == 2)
			return;

		float difY = transform.position.y - playerTrans.position.y;
		difY = difY < yLowerLimit ? difY : yLowerLimit;
		difY = difY > yUpperLimit ? difY : yUpperLimit;


		Vector3 cameraPos = new Vector3(0, playerTrans.position.y + difY, transform.position.z);
		transform.position = cameraPos;

		SetSideCameras();
	}

	void SetSideCameras(){
		cameraLeftTrans.position = transform.position - new Vector3(GameController.edge * 2, 0, 0);
		cameraRightTrans.position = transform.position + new Vector3(GameController.edge * 2, 0, 0);
	}
}
