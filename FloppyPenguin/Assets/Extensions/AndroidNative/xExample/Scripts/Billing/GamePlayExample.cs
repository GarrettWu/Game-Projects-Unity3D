using UnityEngine;
using System.Collections;

public class GamePlayExample : MonoBehaviour {

	public GUIStyle style;


	void Awake() {
		GameBillingManagerExample.init();
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 70, 150, 50), "Coins: " + GameDataExample.coins.ToString(), style);


		GUI.enabled = GameBillingManagerExample.isInited;
		if(GUI.Button(new Rect(10, 130, 150, 50), "Add Coins")) {
			GameBillingManagerExample.purchase(GameBillingManagerExample.COINS_ITEM);
		}


		if(GameDataExample.IsBoostPurchased) {
			GUI.Label(new Rect(10, 190, 150, 50), "Boost is purchased you got  20% bonus for each coins purchase", style);
		} else {
			if(GUI.Button(new Rect(10, 190, 150, 50), "Purchase Boost")) {
				GameBillingManagerExample.purchase(GameBillingManagerExample.COINS_BOOST);
			}

			GUI.Label(new Rect(10, 270, 150, 50), "Boost will add 20% for each coins purchase", style);
		}

	}
}
