using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	

	public void DestroySelf(){
		Destroy(gameObject);
	}
}
