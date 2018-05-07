using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

	public Transform gridGround;
	public Vector3 startPoint;
	public float interval;

	private GameController gameController;

	void Awake(){
		gameController = GameObject.Find("UI Root").GetComponent<GameController>();
	}
	// Use this for initialization
	void Start () {
		GenerateGridGround ();
	}

	void GenerateGridGround (){
		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 4; j++) {
				Vector3 createPoint = new Vector3(startPoint.x + interval * i, startPoint.y +interval * j, 0);

				Transform createTrans = Instantiate (gridGround, Vector3.zero, Quaternion.identity) as Transform;
				createTrans.parent = this.transform;
				createTrans.localPosition = createPoint;
				createTrans.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	public void GenerateGridPositions(){
		for (int i = 0; i < 4; i++){
			for (int j = 0; j < 4; j++) {
				Vector3 gridPosition = new Vector3(startPoint.x + interval * i, startPoint.y +interval * j, 0);
				gameController.gridPositions[i,j] = gridPosition;
			}
		}
	}


}
