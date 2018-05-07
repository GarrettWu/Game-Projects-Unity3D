using UnityEngine;
using System.Collections;

public class CountController : MonoBehaviour {

	private UILabel labelCounting;
	private float time = 2.49f;

	void Awake(){
		labelCounting = GameObject.Find("LabelCounting").GetComponent<UILabel>();

	}

	void Start () {
		labelCounting.text = time.ToString("F0");
		PluginManager.Instance.HideBanner();
	}

	void Update () {
		time -= Time.deltaTime;
		if (time <= 0.5f)
			Application.LoadLevel("GameScene");
		else 
			labelCounting.text = time.ToString("F0");

	}
}
