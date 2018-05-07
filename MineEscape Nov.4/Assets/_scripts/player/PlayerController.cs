using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	//consts & references
	public CellGenerator cellGenerator;
	public GUISkin skin;					
    public AudioClip JumpSound;
    public AudioClip CoinCollect; 
	public float speedLeftRight = 18.0f;		
	public float speedForward = 14.0f;			
	public float speedJump = 19.0f;
	public float gravity = 38.0f;
	public float speedJumpSuperSneaker = 25.0f;
	public float magnetAcc = 10.0f;
	
	private	float pathGap = 4.3f; //distance between paths 	
	
	private GameObject camera;	
	private GameObject player;
	private GameObject magnetTrigger;
	private GameObject larryTrigger;
	private CharacterController controller;
	
	//status
	private bool dead = false;
	private bool pause = false;
	private bool canTurn = false;		
	private bool isSliding = false;
	private bool jumpLaterEvent = false;
	private bool slideEndEvent = false;
	public bool resuming = true;
	
	private bool superSneaker = false;
	private float superSneakerLast = 5.0f;
	private float superSneakerTimer = 0;
	
	private bool magnet = false;
	private float magnetLast = 5.0f;
	private float magnetTimer = 0;
	
	private bool score2X = false;
	private float score2XLast = 5.0f;
	private float score2XTimer = 0;
	
	private bool larry = false;
	private float larryLast = 5.0f;
	private float larryTimer = 0;
	
	private bool monkeyVehicle = false;
	private float monkeyVehicleLast = 100.0f;
	private float monkeyVehicleTimer = 0;
	private float monkeyVehicleHeight = 3.5f;
	private float monkeyVehicleOPHeight = 10.0f;
	private Vector3 monkeyVehicleOPPos, monkeyVehiclePos;
	private float monkeyVehicleAngle = 30.0f;
	private float monkeyVehicleUpSpeed = 2.0f;
	private float monkeyVehicleRotationSpeed = 30.0f;
	private float monkeyVehicleForwardSpeedMulti = 2.0f;
	private bool monkeyVehiclePhase1 = false;
	private float monkeyVehicleTargetRotation;
	private float monkeyVehicleCurrentRotation;
	private float monkeyVehicleDeltaRotation;
	private Vector3 monkeyVehicleDeltaUp;
	
	//vars
	private float ySpeed = 0;	 
	private int Direction;		
	private Vector3[] pathPos = new Vector3[5];
	private int newPath = 2; //2 = middle, 1 = left, 3 = right
	private int oldPath = 2;
	private Vector3 switchVec = new Vector3();
	
	private Vector3 deltaRun, deltaSwitch, deltaJump, deltaSlide;
	
	private float gameSpeed = 1.0f;
	private float score = 0;
	private float coin = 0;
	private float gem = 0;
	private float box = 0;
	
	void Awake()
	{
		controller = GetComponent<CharacterController>();
		camera = GameObject.Find("MainCamera");
		player = GameObject.Find ("Player");
		magnetTrigger = GameObject.Find ("MagnetTrigger");
		larryTrigger = GameObject.Find("LarryTrigger");
		
	}
	
	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.Portrait;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		magnetTrigger.SetActive(false);
		larryTrigger.SetActive(false);
		
		InputManager.instance._touchDir = Dir.None;
		Direction = 0;
		pathPos[2] = Vector3.zero;
		SetPathPos();
		
//		MonkeyVehicleStart();

	}
	
	// Update is called once per frame
	void Update () {
		
		if (dead)
			return;
		
		if (pause)
			return;
		
		SuperSneakerUpdate();
		MagnetUpdate();
		Score2XUpdate();
		LarryUpdate();
		
		if (!monkeyVehicle)
		{
			RunUpdate();
			SwitchUpdate();
			JumpUpdate();
			SlideUpdate();
			TurnUpdate();

			controller.Move(deltaRun + deltaJump + deltaSwitch + deltaSlide);
		}
		else
		{
			MonkeyVehicleUpdate();
			RunUpdate();
			
			if (!monkeyVehiclePhase1)
				monkeyVehicleDeltaUp = Vector3.zero;
			
			controller.Move(deltaRun * monkeyVehicleForwardSpeedMulti + monkeyVehicleDeltaUp);
		}
		
		if (!score2X)
			score += 5 * gameSpeed * Time.deltaTime;
		else
			score += 2 * 5 * gameSpeed * Time.deltaTime;
	
		if (transform.position.y < -0.5f) {
			Die();
		}
		
		
		if (controller.isGrounded && ySpeed <= 0)
		{
			ySpeed = 0;
		}
		
		
		
	}
	
	void JumpUpdate()
	{
		//gravity
		ySpeed -= gravity * Time.deltaTime;
		
		
		
		//jump
		if (jumpLaterEvent == true)
		{
			if (JumpPhase() == 0)
			{
				if (isSliding) slideEndEvent = true;
				audio.PlayOneShot(JumpSound);
				if (superSneaker)
					ySpeed = speedJumpSuperSneaker;
				else
					ySpeed = speedJump;
				jumpLaterEvent = false;
			}
		}
		else if (JumpPhase() == 0)
		{
			if (InputManager.instance._touchDir == Dir.Up )
			{
				if (isSliding) slideEndEvent = true;
				audio.PlayOneShot(JumpSound);
				if (superSneaker)
					ySpeed = speedJumpSuperSneaker;
				else
					ySpeed = speedJump;
				InputManager.instance._touchDir = Dir.Done;
			}
		}
		else if (JumpPhase() == 2) {
			if ( InputManager.instance._touchDir == Dir.Up ) {
				jumpLaterEvent = true;
				InputManager.instance._touchDir = Dir.Done;
			}
		}
				
		deltaJump = new Vector3(0, ySpeed, 0) * Time.deltaTime;
	}
	
	
	void RunUpdate()
	{	
		Vector3 vec3_run = new Vector3();
		switch (Direction)
		{
			case 0:
				vec3_run = new Vector3(0, 0, speedForward); break;
			case 1:
				vec3_run = new Vector3(speedForward, 0, 0); break;
			case 2:
				vec3_run = new Vector3(0, 0, -speedForward); break;
			case 3:
				vec3_run = new Vector3(-speedForward, 0, 0); break;
		}
		
		deltaRun = vec3_run * Time.deltaTime * gameSpeed;
	}
	
	void SwitchUpdate()
	{
		//switch event
		if (!canTurn) 
		{
			if (InputManager.instance._touchDir == Dir.Right && (newPath != 4) )
			{
				if (isSliding) slideEndEvent = true;
				
				oldPath = newPath;
				newPath ++;
				
				InputManager.instance._touchDir = Dir.Done;
			}
			else if (InputManager.instance._touchDir == Dir.Left && (newPath != 0) )
			{
				if (isSliding) slideEndEvent = true;
				
				oldPath = newPath;
				newPath --;
				
				InputManager.instance._touchDir = Dir.Done;
			}
		}
		
		//switch
		if (Direction == 0)
			switchVec = new Vector3 (pathPos[newPath].x - transform.position.x, 0, 0);
		else if (Direction == 1)
			switchVec = new Vector3 (0, 0, pathPos[newPath].z - transform.position.z);
		else if (Direction == 2)
			switchVec = new Vector3 (pathPos[newPath].x - transform.position.x, 0, 0);
		else if (Direction == 3)
			switchVec = new Vector3 (0, 0, pathPos[newPath].z - transform.position.z);
		
		Vector3 norSwitchVec = switchVec.normalized;
		if (switchVec.magnitude > (speedLeftRight * Time.deltaTime) )
		{
			deltaSwitch = norSwitchVec * speedLeftRight * Time.deltaTime;
		}
		else
		{
			deltaSwitch = switchVec;
		}
		
	}
	
	void TurnUpdate()
	{
		if (canTurn && InputManager.instance._touchDir == Dir.Right)
		{
			if (isSliding) slideEndEvent = true;
			Rotate("Right");
			InputManager.instance._touchDir = Dir.Done;
		}
		if (canTurn && InputManager.instance._touchDir == Dir.Left)
		{
			if (isSliding) slideEndEvent = true;
			Rotate("Left");
			InputManager.instance._touchDir = Dir.Done;
		}
	}
	
	void SlideUpdate()
	{
		deltaSlide = Vector3.zero;
		
		//slide
		if (InputManager.instance._touchDir == Dir.Down)
		{
			if (!isSliding) Slide();
			InputManager.instance._touchDir = Dir.Done;
		}
		
		//slide end
		if (slideEndEvent)
		{
			SlideEnd();
			
		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit) 
	{	
		Vector3 relativePoint = transform.InverseTransformPoint(hit.point);
		if ( relativePoint.y > -0.5f )
		{
			if ( relativePoint.z > 0 && Mathf.Abs(relativePoint.z) > Mathf.Abs(relativePoint.x))
				Die();
			else
				Stumble();
		}
    }
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "tag_turning")
		{
			canTurn = true;
			pathPos[2] = other.transform.position;
		}
		if (other.tag == "tag_coin")
		{
			audio.PlayOneShot(CoinCollect);
			coin++;
			Destroy(other.gameObject);
		}
		if (other.tag == "tag_gem")
		{
			audio.PlayOneShot(CoinCollect);
			gem++;
			Destroy(other.gameObject);
		}
		if (other.tag == "tag_box")
		{
			audio.PlayOneShot(CoinCollect);
			box++;
			Destroy(other.gameObject);
		}
		if (other.tag == "tag_superSneaker")
		{
			SuperSneakerStart();
		}
		if (other.tag == "tag_magnet")
		{
			MagnetStart();
		}
		if (other.tag == "tag_score2X")
		{
			Score2XStart();
		}
		if (other.tag == "tag_larry")
		{
			LarryStart();
		}
		if (other.tag == "tag_monkeyVehicle")
		{
			MonkeyVehicleStart();
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "tag_turning")
		{
			canTurn = false;
		}
	}
	
	void OnGUI()
	{
		GUI.skin = skin;

		if (dead)
		{
			if(GUI.Button(new Rect(Screen.width / 2 - 90,Screen.height / 2 - 60,180,50),"Play Again?"))
			{
				Application.LoadLevel("GameScene");
			}
		}	
	}
	
	
	void Rotate(string _dir)
	{
		canTurn = false;
		if (_dir == "Left")
		{
			transform.eulerAngles += new Vector3(0,-90,0);
		}
		else
		{
			transform.eulerAngles += new Vector3(0,90,0);
		}
		
	
		if (_dir == "Left")
		{
			Direction--;
		}
		else if (_dir == "Right")
		{
			Direction++;
		}
		if (Direction > 3)
		{
			Direction = 0;
		}
		else if (Direction < 0)
		{
			Direction = 3;
		}
		
		SetPathPos();
		
		FixToPath();
	}
	
	void Slide()
	{
		if (isSliding) return;
		
		isSliding = true;
		
		ySpeed = -10.0f;

		transform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
		deltaSlide = new Vector3(0, -controller.height/4.0f, 0);
		
		StartCoroutine("WaitToSlide");

	}
		
	void SlideEnd()
	{
		if (!isSliding) return;
		
		slideEndEvent = false;
		isSliding = false;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		deltaSlide = new Vector3(0, controller.height/4.0f, 0);
		
		StopCoroutine("WaitToSlide");
		
	}
	
	void Stumble()
	{
		newPath = oldPath;
	}
	
	void SuperSneakerStart()
	{
		superSneaker = true;
		superSneakerTimer = superSneakerLast;
	}
	
	void SuperSneakerUpdate()
	{
		if (!superSneaker)
			return;
		
		superSneakerTimer -= Time.deltaTime;
		if (superSneakerTimer <= 0)
			superSneaker = false;
	}
	
	void MagnetStart()
	{
		magnet = true;
		magnetTrigger.SetActive(true);
		magnetTimer = magnetLast;
	}
	
	void MagnetUpdate()
	{
		if (!magnet)
			return;
		
		magnetTimer -= Time.deltaTime;
		if (magnetTimer <= 0)
		{
			magnet = false;
			magnetTrigger.SetActive(false);
		}
	}
	
	void Score2XStart()
	{
		score2X = true;
		score2XTimer = score2XLast;
	}
	
	void Score2XUpdate()
	{
		if (!score2X)
			return;
		
		score2XTimer -= Time.deltaTime;
		if (score2XTimer <= 0)
		{
			score2X = false;
		}
	}
	
	void LarryStart()
	{
		larry = true;
		larryTimer = larryLast;
	}
	
	void LarryUpdate()
	{
		if (!larry)
			return;
		
		larryTimer -= Time.deltaTime;
		if (larryTimer <= 0)
		{
			larry = false;
		}
	}
	
	void LarryHit()
	{
		larryTimer = 0;
		larry = false;
		
		larryTrigger.SetActive(true);
		
		StartCoroutine("WaitLarryHitStop");
	}
	
	void MonkeyVehicleStart()
	{
		monkeyVehicle = true;
		monkeyVehicleTimer = monkeyVehicleLast;
		monkeyVehiclePhase1 = true;
		
		Vector3 playerNormalPos = camera.GetComponent<CameraController>().GetPlayerNormalPos();
		monkeyVehiclePos = new Vector3(playerNormalPos.x, 0, playerNormalPos.z) + Vector3.up * monkeyVehicleHeight;
		monkeyVehicleOPPos = new Vector3(playerNormalPos.x, 0, playerNormalPos.z) + Vector3.up * monkeyVehicleOPHeight;
		
		MonkeyVehiclePhase1();
	}
	
	void MonkeyVehicleUpdate()
	{
		if (!monkeyVehicle)
			return;
		
		monkeyVehicleTimer -= Time.deltaTime;
		if (monkeyVehicleTimer <= 0)
		{
			MonkeyVehicleEnd();
		}
		

		Vector3 playerNormalPos = camera.GetComponent<CameraController>().GetPlayerNormalPos();
		
		monkeyVehicleOPPos = new Vector3(playerNormalPos.x, 0, playerNormalPos.z) + Vector3.up * monkeyVehicleOPHeight;
		
		MonkeyVehiclePhase1();
		
		MonkeyVehicleControl();
		//player head to the OriginalPoint
		MonkeyVehiclePointToOP();
			
	}
	
	void MonkeyVehicleControl()
	{

		float j = Input.acceleration.x < 0.5f ? Input.acceleration.x : 0.5f;
		monkeyVehicleTargetRotation = j * 2 * monkeyVehicleAngle;
		
		
		monkeyVehicleCurrentRotation = transform.rotation.eulerAngles.z;
		if (monkeyVehicleCurrentRotation > 180.0f)
			monkeyVehicleCurrentRotation -= 360.0f;
		
		if ((monkeyVehicleCurrentRotation * monkeyVehicleTargetRotation <= 0) ||
			(Mathf.Abs(monkeyVehicleCurrentRotation) <= Mathf.Abs(monkeyVehicleTargetRotation)))
		{
			float i; 
			if (monkeyVehicleTargetRotation == 0)
				i = 0;
			else if (monkeyVehicleTargetRotation < 0)
				i = -1;
			else
				i = 1;
			
			monkeyVehicleDeltaRotation = i * monkeyVehicleRotationSpeed * Time.deltaTime;
			transform.RotateAround(monkeyVehicleOPPos, Vector3.forward, monkeyVehicleDeltaRotation);
		}
	}
	
	void MonkeyVehiclePhase1()
	{
		if (!monkeyVehiclePhase1)
			return;
		
		monkeyVehicleDeltaUp = monkeyVehicleUpSpeed * Vector3.up * Time.deltaTime;
		
		float monkeyVehicleLength = monkeyVehicleOPHeight - monkeyVehicleHeight;
		float tempDistance = Vector3.Distance(transform.position, monkeyVehicleOPPos);
		if (tempDistance <= monkeyVehicleLength)
		{
			monkeyVehicleDeltaUp = Vector3.zero;
			monkeyVehiclePhase1 = false;
			//fix height
			float x = 0, y = 0, r = 0;
			r = monkeyVehicleLength;
			
			if (Direction == 0 || Direction == 2)
				x = Mathf.Abs(transform.position.x - monkeyVehicleOPPos.x);
			else if (Direction == 1 || Direction == 3)
				x = Mathf.Abs(transform.position.z - monkeyVehicleOPPos.z);
			
			y = Mathf.Sqrt(Mathf.Pow(r, 2.0f) - Mathf.Pow(x, 2.0f));
			
			float fixHeight = monkeyVehicleOPPos.y - y - transform.position.y;
			transform.Translate(new Vector3(0, fixHeight, 0) );
		
		}

	}
	
	void MonkeyVehicleEnd()
	{
		monkeyVehicle = false;
		transform.eulerAngles = Vector3.zero;
		FixToPath();	
	}
	
	void MonkeyVehiclePointToOP()
	{
		float tempA = 0;
		if (Direction == 0)
		{
			tempA = transform.position.x - monkeyVehicleOPPos.x;	
		}
		else if (Direction == 1)
		{
			tempA = -(transform.position.z - monkeyVehicleOPPos.z);	
		}
		else if (Direction == 2)
		{
			tempA = -(transform.position.x - monkeyVehicleOPPos.x);	
		}
		else if (Direction == 3)
		{
			tempA = transform.position.z - monkeyVehicleOPPos.z;
		}
		
		float tempB = monkeyVehicleOPPos.y - transform.position.y;
		float tempR = Mathf.Sqrt(Mathf.Pow(tempA, 2f) + Mathf.Pow(tempB, 2f));
		float tempRotateAngle = Mathf.Asin(tempA/tempR) / Mathf.PI * 180f;
		
		transform.eulerAngles = new Vector3(0, 0, tempRotateAngle);
	}
	
	void SetPathPos()
	{
		if (Direction == 0)
		{
			pathPos[1] = pathPos[2] - new Vector3(pathGap, 0, 0);
			pathPos[0] = pathPos[1] - new Vector3(pathGap, 0, 0);
			pathPos[3] = pathPos[2] + new Vector3(pathGap, 0, 0);
			pathPos[4] = pathPos[3] + new Vector3(pathGap, 0, 0);
		}
		else if (Direction == 1)
		{
			pathPos[1] = pathPos[2] + new Vector3(0, 0, pathGap);
			pathPos[0] = pathPos[1] + new Vector3(0, 0, pathGap);
			pathPos[3] = pathPos[2] - new Vector3(0, 0, pathGap);
			pathPos[4] = pathPos[3] - new Vector3(0, 0, pathGap);
		}
		else if (Direction == 2)
		{
			pathPos[1] = pathPos[2] + new Vector3(pathGap, 0, 0);
			pathPos[0] = pathPos[1] + new Vector3(pathGap, 0, 0);
			pathPos[3] = pathPos[2] - new Vector3(pathGap, 0, 0);
			pathPos[4] = pathPos[3] - new Vector3(pathGap, 0, 0);
		}
		else if (Direction == 3)
		{
			pathPos[1] = pathPos[2] - new Vector3(0, 0, pathGap);
			pathPos[0] = pathPos[1] - new Vector3(0, 0, pathGap);
			pathPos[3] = pathPos[2] + new Vector3(0, 0, pathGap);
			pathPos[4] = pathPos[3] + new Vector3(0, 0, pathGap);
		}
		
	}
	
	void FixToPath()
	{
		float[] dif = new float[5]; 
		if (Direction == 0)
		{
			dif[1] = Mathf.Abs(pathPos[1].x - transform.position.x);
			dif[2] = Mathf.Abs(pathPos[2].x - transform.position.x);
			dif[3] = Mathf.Abs(pathPos[3].x - transform.position.x);
		}
		else if (Direction == 1)
		{
			dif[1] = Mathf.Abs(pathPos[1].z - transform.position.z);
			dif[2] = Mathf.Abs(pathPos[2].z - transform.position.z);
			dif[3] = Mathf.Abs(pathPos[3].z - transform.position.z);
		}
		else if (Direction == 2)
		{
			dif[1] = Mathf.Abs(pathPos[1].x - transform.position.x);
			dif[2] = Mathf.Abs(pathPos[2].x - transform.position.x);
			dif[3] = Mathf.Abs(pathPos[3].x - transform.position.x);
		}
		else if (Direction == 3)
		{
			dif[1] = Mathf.Abs(pathPos[1].z - transform.position.z);
			dif[2] = Mathf.Abs(pathPos[2].z - transform.position.z);
			dif[3] = Mathf.Abs(pathPos[3].z - transform.position.z);
		}	
		
		//find nearest path
		int i;
		if (dif[1] < dif[2])
			i = 1;
		else
			i = 2;
		if (dif[3] < dif[i])
			i = 3;
		
		newPath = i;
	}
	
	int JumpPhase()
	{
		if (controller.isGrounded == true)
			return 0; //no jump
		else if (ySpeed >= 0)
			return 1; //jump up
		else
			return 2; //fall down
	}
	
	public void Die()
	{
		if (!larry)
			dead = true;
		else
			larry = false;
	}
	
	public Vector3 GetPath2Pos()
	{
		return pathPos[2];
	}
	
	public int GetDirection()
	{
		return Direction;
	}
	
	IEnumerator WaitToSlide()
	{
		yield return new WaitForSeconds(1.0f);
		
		if (isSliding) 
			slideEndEvent = true;
	}
	
	IEnumerator WaitLarryHitStop()
	{
		yield return new WaitForSeconds(0.5f);
		
		larryTrigger.SetActive(false);
	}
	
	public float GetScore()
	{
		return score;
	}
	
	public float GetCoin()
	{
		return coin;
	}
	
	public void Pause()
	{
		pause = true;
	}
	
	public void Resume()
	{
		pause = false;
		resuming = true;
	}
	

	
}

