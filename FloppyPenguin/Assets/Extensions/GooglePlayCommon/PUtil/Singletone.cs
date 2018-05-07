////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Native Plugin 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public abstract class Singletone<T> : EventDispatcher where T : MonoBehaviour {

	private static T _instance = null;



	public static T instance {

		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType(typeof(T)) as T;
				if (_instance == null) {
					_instance = new GameObject ().AddComponent<T> ();
					_instance.gameObject.name = _instance.GetType ().Name;
				}
			}

			return _instance;

		}

	}

	public static bool IsDestroyed {
		get {
			if(_instance == null) {
				return true;
			} else {
				return false;
			}
		}
	}

}
