using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public int color = 0;


	EnemyGenerator enemyGenerator;
	float fallSpeed;

	Color rendererColor;

	void Awake(){
		enemyGenerator = GameObject.Find("EnemyGenerator").GetComponent<EnemyGenerator>();
	}
	// Use this for initialization
	void Start () {
		RandomColor();

		fallSpeed = enemyGenerator.fallSpeed;
		Move(fallSpeed);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void RandomColor(){
		color = Random.Range(1, 8);
		switch (color){
		case 0:
			GetComponent<SpriteRenderer>().color = GameController.white; break;
		case 1:
			GetComponent<SpriteRenderer>().color = GameController.red; break;
		case 2:
			GetComponent<SpriteRenderer>().color = GameController.yellow; break;
		case 3:
			GetComponent<SpriteRenderer>().color = GameController.blue; break;
		case 4:
			GetComponent<SpriteRenderer>().color = GameController.orange; break;
		case 5:
			GetComponent<SpriteRenderer>().color = GameController.green; break;
		case 6:
			GetComponent<SpriteRenderer>().color = GameController.purple; break;
		case 7:
			GetComponent<SpriteRenderer>().color = GameController.black; break;
		}
	}

	void Move(float fallSpeed){
		rigidbody2D.velocity = new Vector2(0, -fallSpeed);
	}
}
