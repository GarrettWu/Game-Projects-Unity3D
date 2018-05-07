using UnityEngine;
using System.Collections;
using System;

public enum Dir
{
	Left,
	Right,
	Up,
	Down,
	None,
	Done,
};

public class InputManager: MonoSingleton< InputManager >
{
	public float duration = 0.5f;
	public Dir _touchDir;
	
	void Update ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved && Input.GetTouch (0).deltaPosition.magnitude >= 30 ) {
				if (Math.Abs (Input.GetTouch (0).deltaPosition.x) >= Math.Abs (Input.GetTouch (0).deltaPosition.y)) {
					if (Input.GetTouch (0).deltaPosition.x < 0 - Mathf.Epsilon) {
						if (_touchDir == Dir.None) {
							_touchDir = Dir.Left;
							Debug.Log("left..");
						}
					} else {
						if (_touchDir == Dir.None) {
							_touchDir = Dir.Right;
							Debug.Log("right..");
						}
					}
				} else {
					if (Input.GetTouch (0).deltaPosition.y < 0 - Mathf.Epsilon) {
						if (_touchDir == Dir.None) {
							_touchDir = Dir.Down;
							Debug.Log("down..");
						}
					} else {
						if (_touchDir == Dir.None) {
							_touchDir = Dir.Up;
							Debug.Log("up..");
						}
					}
				}
				
			}
			if (Input.touchCount > 0 && (Input.GetTouch (0).phase == TouchPhase.Ended || Input.GetTouch (0).phase == TouchPhase.Canceled)) {
				_touchDir = Dir.None;
				Debug.Log("none..");
			}
		} else {
			if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
				_touchDir = Dir.Left;
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
				_touchDir = Dir.Right;
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
				_touchDir = Dir.Down;
			}
			else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
				_touchDir = Dir.Up;
			}
			else {
				_touchDir = Dir.None;
			}
		}
	}
}