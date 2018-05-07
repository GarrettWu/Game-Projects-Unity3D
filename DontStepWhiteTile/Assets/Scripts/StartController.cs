using UnityEngine;
using System.Collections;

public class StartController : MonoBehaviour {
	
	void Start () {
		GameStates.state = 0;
		PluginManager.Instance.Init();
		PluginManager.Instance.CacheInterstitials();
		PluginManager.Instance.CreateBanner();
	}

	void Update(){
		PluginManager.Instance.OnEscapeKey();
	}


	public void ButtonTimeAttack(){
		GameStates.mode = 0;
		Application.LoadLevel("CountScene");
	}
	
	public void ButtonLongRun(){
		GameStates.mode = 1;
		Application.LoadLevel("CountScene");
	}
	
	public void ButtonStoppingJust(){
		GameStates.mode = 2;
		Application.LoadLevel("CountScene");
	}
}
