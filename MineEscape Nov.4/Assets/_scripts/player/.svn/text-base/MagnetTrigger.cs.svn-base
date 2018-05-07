using UnityEngine;
using System.Collections;

public class MagnetTrigger : MonoBehaviour {	
	void OnTriggerEnter(Collider other)
	{	
		if (other.tag == "tag_coin")
		{
			MonoBehaviour coinScript = (MonoBehaviour)other.GetComponent("Coin");
			other.SendMessage("MagnetActivate");
		}
		
	}

}
