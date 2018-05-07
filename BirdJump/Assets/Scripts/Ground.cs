using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour {

	private Transform terrainGeneratorTrans;

	
	void Awake()
	{
		terrainGeneratorTrans = GameObject.Find("RootGame/TerrainGenerator").transform;
	}

	void FixedUpdate () 
	{
		
		if (transform.position.y < (this.terrainGeneratorTrans.position.y - 16.0f) )
			Destroy(gameObject);
		
	}

}
