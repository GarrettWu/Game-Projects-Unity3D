using UnityEngine;
using System.Collections;

public class GameSceneItemProgress : MonoBehaviour {
	
	UISlider itemProgress;
	
	// Use this for initialization
	void Awake () {
		itemProgress = GetComponent<UISlider>();
	
	}
	
	// Update is called once per frame
	void Update () {
		itemProgress.value = 0.5f;	
	
	}
}
