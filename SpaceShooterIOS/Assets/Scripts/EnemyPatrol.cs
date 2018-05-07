using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {
	EnemyController enemyController;

	void Awake(){
		enemyController = transform.parent.GetComponent<EnemyController>();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag.Equals("Player")){
			enemyController.Attack();
		}
	}
}
