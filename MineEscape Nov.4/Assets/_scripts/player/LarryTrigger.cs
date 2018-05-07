using UnityEngine;
using System.Collections;

public class LarryTrigger : MonoBehaviour {
	
	void OnTriggerEnter(Collider other)
	{	
		if (other.tag == "tag_block")
		{
			Destroy(other.gameObject);
		}
	}
}
