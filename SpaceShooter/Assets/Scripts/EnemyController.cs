using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float velocityPatrolMin;
	public float velocityPatrolMax;

	public float velocityAttackMin;
	public float velocityAttackMax;

	private float velocityPatrol;
	private float velocityAttack;

	private Vector2 patrolTarget;
	private GameObject explosion;
	private GameController gameController;

	void Awake(){
		explosion = Resources.Load("Prefabs/Explosion") as GameObject;
		gameController = Camera.main.GetComponent<GameController>();
	}
	// Use this for initialization
	void Start () {
		velocityPatrol = Random.Range (velocityPatrolMin, velocityPatrolMax);
		velocityAttack = Random.Range (velocityAttackMin, velocityAttackMax);

		Patrol();
	}
	
	// Update is called once per frame
	void Update () {

	}

	//Patrol
	void Patrol(){ 
		patrolTarget = new Vector2(Random.Range(-3.6f, 3.6f), Random.Range(-6.4f, 6.4f));

		EnemyMove(patrolTarget, velocityPatrol);
	}

	//Move towards player
	public void Attack(){
		EnemyMove(Vector2.zero, velocityAttack);
	}

	void EnemyMove(Vector2 target, float speed){
		rigidbody2D.velocity = (target - new Vector2(transform.position.x, transform.position.y)).normalized * speed;
		float angle = -Mathf.Atan2(target.x - transform.position.x, target.y - transform.position.y) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0, 0, angle);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag.Equals("Bullet")){
			Instantiate(explosion, transform.position, Quaternion.identity);
			gameController.ScoreAdd();

			Destroy(gameObject);
			Destroy(other.gameObject);
			SoundManager.Instance.PlayAudio("explosion");

		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.name.Equals("EnemyArea")){
			Destroy(gameObject);
		}
	}
}
