using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public int number;
	public bool isMove;
	public bool isPredator;
	public bool isPrey;
	public int currentX, currentY;
	public int targetX, targetY;

	void Awake(){
		isMove = false;
		isPredator = false;
		isPrey = false;
	}


}
