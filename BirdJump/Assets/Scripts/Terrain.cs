using UnityEngine;
using System.Collections;

public class Terrain : MonoBehaviour {

	public GameObject terrainParts;

	private Transform playerTrans;
	private BoxCollider2D playerBox2d;
	private Transform terrainGeneratorTrans;

	private AudioController audioController;

	public GameObject pairTerrain;
	public GameObject originalTerrain;

	private BoxCollider2D box2d;

	void Awake()
	{
		playerTrans = GameObject.Find("RootGame/Player").transform;
		playerBox2d = GameObject.Find("RootGame/Player").GetComponent<BoxCollider2D>();
		terrainGeneratorTrans = GameObject.Find("RootGame/TerrainGenerator").transform;

		audioController = GameObject.Find("AudioController").GetComponent<AudioController>();

		box2d = GetComponent<BoxCollider2D>();
	}
	// Use this for initialization
	void Start () 
	{
		collider2D.isTrigger = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (transform.position.y < playerTrans.position.y - playerBox2d.size.y/2 - box2d.size.y/2)
			collider2D.isTrigger = false;
		else
			collider2D.isTrigger = true;

		if (transform.position.y < (this.terrainGeneratorTrans.position.y - 16.0f) )
			DestroySelf();

	}

	public void DestroySelf(){

		if (originalTerrain != null){
			originalTerrain.GetComponent<Terrain>().DestroySelf();
		} else {
			Instantiate(terrainParts, transform.position, Quaternion.identity);
			Destroy(gameObject);
//			audioController.PlayAudio("crack");
			audioController.PlayAudio("point");
			GameController.score++;

			if (pairTerrain != null){
				Destroy(pairTerrain);
			}
		}
	}
}
