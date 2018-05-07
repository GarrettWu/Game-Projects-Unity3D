using UnityEngine;
using System.Collections;

public class CellCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
		
	void OnTriggerEnter(Collider otherObject) {
		if(otherObject.tag == "Player"){
			
			if(CellGenerator.instance.m_toDeleteCell != null){
//				resetCellColor(CellGenerator.instance.m_toDeleteCell);
				CellGenerator.instance.m_cellObjects.Remove(CellGenerator.instance.m_toDeleteCell);
				ObjectPool.instance.destroyObj(CellGenerator.instance.m_toDeleteCell);
			}
			
				CellGenerator.instance.m_toDeleteCell = gameObject.transform.parent.parent.gameObject;
//				setCellColor(CellGenerator.instance.m_toDeleteCell);
		}
	}
	
	
	
	//for test
	void setCellColor(GameObject gameobject){
		if(gameobject!=null){
			Transform[] allChildren = gameobject.GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren) {
				child.transform.renderer.material.color = Color.blue;
			}
		}
	}
	
	
	//for test
	void resetCellColor(GameObject gameobject){
		if(gameobject!=null){
			Transform[] allChildren = gameobject.GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren) {
				child.transform.renderer.material.color = Color.white;
			}
		}
	}
	
	
}
