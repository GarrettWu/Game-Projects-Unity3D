using UnityEngine;
using System.Collections;

public class ScoreTrigger : MonoBehaviour {

	GameObject enemy;

	void Awake(){
		enemy = transform.parent.gameObject;
	}

	void OnTriggerExit2D(Collider2D other){
		GameController.score ++;
		enemy.BroadcastMessage("Passed");
	}
}
