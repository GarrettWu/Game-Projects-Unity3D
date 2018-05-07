using UnityEngine;
using System.Collections;

public class GameSceneScore : MonoBehaviour {
	
	PlayerController playerController;
	UILabel label;
	// Use this for initialization
	void Start () {
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		label = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		label.text = Mathf.Round(playerController.GetScore()).ToString();
	
	}
}
