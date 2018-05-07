using UnityEngine;
using System.Collections;

public class SmallBallMove : MonoBehaviour {

	private float time = 2.0f;
	public float degree;

	private ColorController colorController;
	// Use this for initialization
	void Awake(){
		colorController = GameObject.Find("Colors/Color").GetComponent<ColorController>();
	}

	void Start () {
		GetComponent<SpriteRenderer>().color = colorController.objectColor;

		float velocityX = Mathf.Sin(degree/180f * Mathf.PI);
		float velocityY = Mathf.Cos(degree/180f * Mathf.PI);

		iTween.MoveBy(gameObject, iTween.Hash("amount", new Vector3(velocityX, velocityY) * 2.5f, "easetype", "easeOutCubic", "time", time));
		iTween.FadeTo(gameObject, iTween.Hash("time", time, "easetype", "easeInCubic", "alpha", 0));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
