using UnityEngine;
using System.Collections;

public class TerrainPart : MonoBehaviour {

	private float forceRangeX = 100.0f;
	private float forceRangeY = 50.0f;
	private Transform terrainGeneratorTrans;

	// Use this for initialization
	void Start () {
		terrainGeneratorTrans = GameObject.Find("RootGame/TerrainGenerator").transform;

		Vector2 force = new Vector2(Random.Range(-forceRangeX, forceRangeX), Random.Range(-forceRangeY, 0));
		rigidbody2D.AddForce(force);
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < (this.terrainGeneratorTrans.position.y - 16.0f) )
			Destroy(transform.parent.gameObject);
	}
}
