using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {

	public float fallSpeedInitial;
	public float fallSpeedIncrement;
	public float intervalMeanInitial;
	public float intervalVariation;
	public float intervalDecrement;

	public float fallSpeed;

	private float interval, intervalMean;
	private GameObject enemy;
	private GameObject theEnemy;
	private GameObject enemies;
	private GameObject theEnemies;

	void Awake(){
		enemy = Resources.Load("Prefabs/Enemy")as GameObject;
		enemies = Resources.Load("Prefabs/Enemies") as GameObject;
	}
	// Use this for initialization
	void Start () {
		ResetEnemyGenerator();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenerateEnemy(){
		theEnemy = Instantiate(enemy, transform.position, Quaternion.identity) as GameObject;
		theEnemy.transform.parent = theEnemies.transform;
	}

	public void StartGenerate(){
		theEnemies = Instantiate(enemies) as GameObject;
		StartCoroutine("WaitToGenerate");
	}

	public void StopGenerate(){
		StopCoroutine("WaitToGenerate");
	}

	IEnumerator WaitToGenerate(){
		yield return new WaitForSeconds(interval);
		interval = Random.Range (intervalMean - intervalVariation, intervalMean - intervalVariation);
		GenerateEnemy();
		StartCoroutine("WaitToGenerate");
	}

	public void FallSpeedIncrement(){
		fallSpeed += fallSpeedIncrement;
	}

	public void IntervalMeanDecrement(){
		if (intervalMean > 0.4f){
			intervalMean -= intervalDecrement;
		}
	}

	public void ResetEnemyGenerator(){
		intervalMean = intervalMeanInitial;
		interval = Random.Range (intervalMean - intervalVariation, intervalMean - intervalVariation);
		fallSpeed = fallSpeedInitial;
	}
}
