using UnityEngine;
using System.Collections;

public class Backgrounds : MonoBehaviour {

	public Transform background_0;
	public Transform background_1;
	public Transform background_2;

	private float backgroundGap = 768f/50f;
	private Transform background0, background1;

	void Awake(){

	}

	void Start() {
		Init ();
	}

	void LateUpdate(){

	}

	void Init() {
		background0 = Instantiate(background_0, Vector3.zero, Quaternion.identity) as Transform;
		background1 = Instantiate(background_0, new Vector3(0, backgroundGap, 0), Quaternion.identity) as Transform;
	}

	void CheckShift() {
		Transform targetTrans;

		targetTrans = background0;
		if (targetTrans.position.y < (transform.position.y - 6.4f - backgroundGap/2)){
			targetTrans.transform.Translate(new Vector3 (0, backgroundGap * 2, 0));
		}
		targetTrans = background1;
		if (targetTrans.position.y < (transform.position.y - 6.4f - backgroundGap/2)){
			targetTrans.transform.Translate(new Vector3 (0, backgroundGap * 2, 0));
		}
	}

}
