using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {



	void OnTriggerExit2D(Collider2D other){
		if (other.name.Equals("EnemyArea")){

			Destroy(gameObject);
		}
	}

}
