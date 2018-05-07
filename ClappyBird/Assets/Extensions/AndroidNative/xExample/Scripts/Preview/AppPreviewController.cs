////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class AppPreviewController : MonoBehaviour {

	void OnGUI() {

		if(GUI.Button(new Rect(10, 10, 150, 50), "Billin Preview")) {
			Application.LoadLevel("BillingExample");
		}

		if(GUI.Button(new Rect(10, 80, 150, 50), "Play Service Preview")) {
			Application.LoadLevel("PlayServiceExample");
		}
	}
}
