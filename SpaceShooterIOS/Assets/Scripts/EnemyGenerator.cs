using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
	public float generateCooldownVariable;

	private float generateCooldownMean = 3.0f;
	private GameObject enemy0, enemy1, enemy2;
	private Sprite sprite;
	private float generateCooldown;

	void Awake(){
		enemy0 = Resources.Load("Prefabs/Enemy0") as GameObject;
		enemy1 = Resources.Load("Prefabs/Enemy1") as GameObject;
		enemy2 = Resources.Load("Prefabs/Enemy2") as GameObject;

	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	}

	IEnumerator WaitToGenerate(){
		generateCooldown = Random.Range(generateCooldownMean - generateCooldownVariable, generateCooldownMean + generateCooldownVariable);
		yield return new WaitForSeconds(generateCooldown);
		GenerateEnemy();
		StartCoroutine("WaitToGenerate");
	}

	void GenerateEnemy(){
		Vector2 generatePoint;
		int randomLine = Random.Range(0, 4);// 0 for up, 1 for right, 2 for down, 3 for left

		switch (randomLine){
		case 0:
			generatePoint = new Vector2(Random.Range(-4.6f, 4.6f), 7.4f); break;
		case 1:
			generatePoint = new Vector2(4.6f, Random.Range(-7.4f, 7.4f)); break;
		case 2:
			generatePoint = new Vector2(Random.Range(-4.6f, 4.6f), -7.4f); break;
		case 3:
			generatePoint = new Vector2(-4.6f, Random.Range(-7.4f, 7.4f)); break;
		default:
			generatePoint = new Vector2(-4.6f, Random.Range(-7.4f, 7.4f)); break;
		}

		GameObject theEnemy; 
		int enemyNo = Random.Range(0, 3);
		if (enemyNo == 0){
			theEnemy = enemy0;
		} else if (enemyNo == 1){
			theEnemy = enemy1;
		} else {
			theEnemy = enemy2;
		}

		Instantiate(theEnemy, new Vector3(generatePoint.x, generatePoint.y), Quaternion.identity);

	}

	public void StartGenerate(){
		StartCoroutine("WaitToGenerate");
	}

	public void StopGenerate(){
		StopCoroutine("WaitToGenerate");
	}

	public void GenerateAccelerate(){
		if (generateCooldownMean > 1.6f){
			generateCooldownMean -= 0.2f;
		}
	}

	public void ResetEnemyGenerator(){
		generateCooldownMean = 3.0f;
	}
}
