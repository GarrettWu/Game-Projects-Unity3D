using UnityEngine;
using System.Collections;

public class TrainCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider otherObject) {
		Transform cellTransform = gameObject.transform.parent.parent.parent;
		if(cellTransform.FindChild("obstacles")!=null) {
			int count = cellTransform.FindChild("obstacles").childCount;
			for (int i = 0; i < count; i++){
				Transform obstacleGroupTransform = cellTransform.FindChild("obstacles").GetChild(i);
				if(obstacleGroupTransform.gameObject.activeSelf){
					setTrainsRun(obstacleGroupTransform);
				}
			}
		}
	}
	
	
	private void setTrainsRun(Transform obstacleGroupTransform){
		int count = obstacleGroupTransform.childCount;
		for (int i = 0; i < count; i++){
			Transform trainTransform = obstacleGroupTransform.GetChild(i);
			Train trainClass = (Train)trainTransform.GetComponent("Train");
			if(trainClass!=null){
				trainClass.isRun = true;
			}
		}
	}
	
}
