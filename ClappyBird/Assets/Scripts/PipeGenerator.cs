using UnityEngine;
using System.Collections;

public class PipeGenerator : MonoBehaviour {

	public float pipeInterval = 1.6f;
	private float pipeHeightVar = 0.9f;
	private float pipeHeightOffset = 0.5f;


	private GameObject bird;

	void Awake(){
		bird = GameObject.Find("Bird");
	}
	// Use this for initialization
	void Start () {
		transform.position = new Vector3(transform.position.x, Random.Range(-pipeHeightVar, pipeHeightVar) + pipeHeightOffset, transform.position.z);
		
	}
	
	// Update is called once per frame
	void Update () {
		CheckGenerate();
	}

	void CheckGenerate(){
		if (GameController.gameState == 0){
			if (bird.transform.position.x + 5f  > transform.position.x)
				Generate ();
		}

		else if (bird.transform.position.x - 2f  > transform.position.x ){
			Generate ();
		}
	}

	void Generate(){
		transform.Translate(new Vector3 (pipeInterval * 3.0f, 0, 0));
		transform.position = new Vector3(transform.position.x, Random.Range(-pipeHeightVar, pipeHeightVar) + pipeHeightOffset, transform.position.z);

	}
}
