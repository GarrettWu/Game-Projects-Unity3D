using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float jumpForce = 20.0f;
	public float moveSpeed = 1.0f;
	
	private float deltaJumpTime = -0.5f;

	private float[] yArray = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

	private Transform terrainGeneratorTrans;
	private Transform groundTrans;
	private GameController gameController;
	private Animator animator;
	private AudioController audioController;



	void Awake()
	{
		terrainGeneratorTrans = GameObject.Find("RootGame/TerrainGenerator").transform;
		groundTrans = GameObject.Find("RootGame/Ground").transform;
		gameController = GameObject.Find("RootGame").GetComponent<GameController>();
		animator = GameObject.Find("RootGame/Player/PlayerAnim").GetComponent<Animator>();
		audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
	}
	void Start () 
	{
		transform.position = new Vector3(0, groundTrans.position.y + GetComponent<BoxCollider2D>().size.y/2 + groundTrans.GetComponent<BoxCollider2D>().size.y/2 + 0.01f, 0);
	}

	void Update () 
	{


		if (GameController.gameState == 2)
			return;

		CheckDie();

		AnimatorUpdate();

		deltaJumpTime -= Time.deltaTime;

		CheckInput();

		CheckBeyondEdge();
		CheckStuck();

	}

	void CheckInput(){
//		float x = Input.GetAxis("Horizontal");
		float x = Input.acceleration.x;
		transform.Translate(new Vector3(moveSpeed * x, 0, 0));



	}

	void CheckBeyondEdge(){
		if (transform.position.x > GameController.edge)
			transform.position = new Vector3(-GameController.edge, transform.position.y, transform.position.z);
		
		else if (transform.position.x < -GameController.edge)
			transform.position = new Vector3(GameController.edge, transform.position.y, transform.position.z);
	}

	void OnCollisionEnter2D(Collision2D coll) 
	{
		if (GameController.gameState != 1)
			return;

        Jump(coll);
	}

	void OnCollisionStay2D(Collision2D coll) 
	{
		if (GameController.gameState != 1)
			return;

		Jump();
	}

	public void Jump(Collision2D coll)
	{
        if (deltaJumpTime > -0.5f)
            return;
        rigidbody2D.AddForce(new Vector2(0, jumpForce));
		deltaJumpTime = 0;

		if (coll != null && coll.collider.tag.Equals("Terrain")){
			coll.gameObject.SendMessage("DestroySelf");
		}

		audioController.PlayAudio("wing");
		animator.SetBool("isGround", false);
	}

	public void Jump(){
		Jump (null);
	}

	void CheckDie()
	{
		if (transform.position.y < (this.terrainGeneratorTrans.position.y - 16f) )
		{
			Die ();
		}
	}

	void Die(){
		animator.SetBool("isDie", true);
		gameController.End();

	}

	void CheckStuck(){
		if (IsStuck() && GameController.gameState == 1){
			Jump();
		}
	}

	void AnimatorUpdate(){
		animator.SetFloat("speedY", rigidbody2D.velocity.y);
	}

	bool IsStuck()
	{
		bool mayStuck = true;

		for (int i = 0; i < 10; i++)
		{
			if ( !((transform.position.y < this.yArray[i] + 0.01f) && (transform.position.y > this.yArray[i] - 0.01f)) )
				mayStuck = false;

			if (i < 9)
				yArray[i] = yArray[i+1];
			else 
				yArray[i] = transform.position.y;
		}
		return mayStuck;
	}


}
