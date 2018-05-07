using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

	public float distanceMean = 2.5f;
	public float distanceDev = 1.0f;

	public GameObject terrain;

	private GameObject pairTerrain;
	private GameObject originalTerrain;

	private float targetY = 0;
	private float upFromCamera = 8.0f;

	private float terrainBoxSizeX;
	private float playerBoxSizeX;


	// Use this for initialization
	void Awake()
	{
		terrainBoxSizeX = terrain.GetComponent<BoxCollider2D>().size.x;
		playerBoxSizeX = GameObject.Find("RootGame/Player").GetComponent<BoxCollider2D>().size.x;
	}

	void Start () 
	{
		transform.position = Vector3.zero;
		GetNewTargetY();
		StartGenerate();
	}
	
	// Update is called once per frame
	void Update () 
	{
		PushedUp();

		if (transform.position.y >= targetY)
		{
			GenerateTerrain();

			GetNewTargetY();
		}
	}

	void PushedUp()
	{
		float minY = Camera.main.transform.position.y + this.upFromCamera;
		if (transform.position.y < minY)
			transform.position = new Vector3(transform.position.x, minY, transform.position.z);

	}

	void GenerateTerrain()
	{
		GameObject newTerrain = null;
		GameObject origialTerrain = null;
		GameObject pairTerrain = null;

		Vector3 terrainPos = new Vector3(Random.Range (-GameController.edge + terrainBoxSizeX / 2 , GameController.edge - terrainBoxSizeX / 2), targetY, this.transform.position.z);

		newTerrain = Instantiate(terrain, terrainPos, Quaternion.identity) as GameObject;

		float terrainBound = GameController.edge - playerBoxSizeX / 2 - terrainBoxSizeX / 2;
		Vector3 offsetPosition = new Vector3 (GameController.edge * 2, 0, 0);

		if (terrainPos.x < -terrainBound){
			pairTerrain = Instantiate(terrain, newTerrain.transform.position + offsetPosition, Quaternion.identity) as GameObject;
		}
		else if (terrainPos.x > terrainBound){
			pairTerrain = Instantiate(terrain, newTerrain.transform.position - offsetPosition, Quaternion.identity) as GameObject;
		}

		if (pairTerrain != null){
			newTerrain.GetComponent<Terrain>().pairTerrain = pairTerrain;
			pairTerrain.GetComponent<Terrain>().originalTerrain = newTerrain;
		}
	}

	void StartGenerate()
	{
		for (float i = 0; i < 8.0f; i += 0.1f)
		{
			transform.position = new Vector3(transform.position.x, i, transform.position.z);

			if (transform.position.y >= targetY)
			{
				GenerateTerrain();
				GetNewTargetY();
			}

		}

	}

	void GetNewTargetY()
	{
		targetY = targetY + distanceMean + Random.Range (-distanceDev, distanceDev);
	}
}
