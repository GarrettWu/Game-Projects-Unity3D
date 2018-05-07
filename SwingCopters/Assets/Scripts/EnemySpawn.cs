using UnityEngine;
using System.Collections;

public class EnemySpawn : MonoBehaviour {
	
	public Transform Enemy;
	public float randomRange;
	
	private Transform earthTrans; 
	private float EnemyGap = 320f/50f;
	private float EnemyPosY;
//	private int enemiesNum = 8;
//	private Transform[] enemies;
	
	void Awake(){
//		enemies = new Transform[enemiesNum];
		earthTrans = GameObject.Find("Earth").transform;
	}
	
	void Start() {
		Init ();
	}
	
	void Update(){
		if (transform.position.y >= EnemyPosY){
			GenerateEnemy();
		}
	}
	
	
	void Init() {
		EnemyPosY = 4.5f;
		GenerateEnemy();
	}

	void GenerateEnemy() {
		float randomX = Random.Range(-randomRange, randomRange);
		Instantiate(Enemy, new Vector3(randomX, EnemyPosY, 0), Quaternion.identity);
		EnemyPosY += EnemyGap;
	}
	
//	void CheckShift() {
//		float earthY = earthTrans.position.y;
//		
//		Transform targetTrans;
//		
//		for (int i = 0; i < enemiesNum; i++){
//			targetTrans = enemies[i];
//			if (targetTrans.position.y < earthY - 6.4f - EnemyGap/2){
//				Destroy(targetTrans.gameObject);
//				targetTrans.transform.Translate(new Vector3 (0, EnemyGap * (float)(enemiesNum), 0));
//			}
//		}
//		
//	}
	
}