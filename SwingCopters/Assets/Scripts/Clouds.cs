using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {
	
	public Transform cloud_0;
	public Transform cloud_1;
	public Transform cloud_2;

	private Transform earthTrans; 
	private float cloudGap = 378f/50f;
	private int cloudsNum = 6;
	private Transform[] clouds;
	
	void Awake(){
		clouds = new Transform[cloudsNum];
		earthTrans = GameObject.Find("Earth").transform;
	}
	
	void Start() {
		Init ();
	}

	void Update(){
		CheckShift();
	}

	
	void Init() {
		Transform cloud = cloud_0;

		for (int i = 0; i < cloudsNum; i++){
			clouds[i] = Instantiate(cloud, new Vector3(0, 2.2f + cloudGap * i, 0), Quaternion.identity) as Transform;
		}

	}
	
	void CheckShift() {
		float earthY = earthTrans.position.y;

		Transform targetTrans;

		for (int i = 0; i < cloudsNum; i++){
			targetTrans = clouds[i];
			if (targetTrans.position.y < earthY - 6.4f - cloudGap/2){
				targetTrans.transform.Translate(new Vector3 (0, cloudGap * (float)(cloudsNum), 0));
			}
		}

	}
	
}