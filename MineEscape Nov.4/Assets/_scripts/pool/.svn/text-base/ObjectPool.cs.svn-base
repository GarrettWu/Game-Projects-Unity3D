using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ObjectCache {
	public GameObject prefab;
	public int cacheSize = 10;
	private GameObject[] objects;
	private int cacheIndex = 0;
		
	public void initialize ()
	{
		objects = new GameObject[cacheSize];
			
		for (int i = 0; i < cacheSize; i++) {
			objects [i] = MonoBehaviour.Instantiate (prefab) as GameObject;
			objects [i].SetActive (false);
			objects [i].name = objects [i].name + i;
			objects [i].transform.parent 
				= ObjectPool.instance.transform;
			//Debug.Log("obj"+prefab.name+" index:"+i);
		}
	}
		
	public GameObject GetNextObjectInCache ()
	{
		GameObject obj = null;

		// The cacheIndex starts out at the position of the object created
		// the longest time ago, so that one is usually free,
		// but in case not, loop through the cache until we find a free one.
		for (int i = 0; i < cacheSize; i++) {
			obj = objects [cacheIndex];

			// If we found an inactive object in the cache, use that.
			if (!obj.activeSelf)
				break;

			// If not, increment index and make it loop around
			// if it exceeds the size of the cache
			cacheIndex = (cacheIndex + 1) % cacheSize;
		}

		// The object should be inactive. If it's not, log a warning and use
		// the object created the longest ago even though it's still active.
		if (obj.activeSelf) {
			Debug.LogWarning (
				"Spawn of " + prefab.name +
				" exceeds cache size of " + cacheSize +
				"! Reusing already active object.", obj);
			ObjectPool.instance.destroyObj (obj);
		}

		// Increment index and make it loop around
		// if it exceeds the size of the cache
		cacheIndex = (cacheIndex + 1) % cacheSize;

		return obj;
	}
}

public class ObjectPool : MonoSingleton< ObjectPool >
{
	public ObjectCache[] caches;
	private Hashtable activeCachedObjects;
	
	void Awake ()
	{
		// Total number of cached objects
		int amount = 0;

		// Loop through the caches
		for (int i = 0; i < caches.Length; i++) {
			// Initialize each cache
			caches [i].initialize ();

			// Count
			amount += caches [i].cacheSize;
		}

		// Create a hashtable with the capacity set to the amount of cached objects specified
		activeCachedObjects = new Hashtable (amount);
	}

	public GameObject getObj (GameObject prefab, Vector3 position, Quaternion rotation)
	{
		ObjectCache cache = null;

		// Find the cache for the specified prefab
			for (var i = 0; i < caches.Length; i++) {
				if (caches [i].prefab == prefab) {
					cache = caches [i];
				}
			}

		// If there's no cache for this prefab type, just instantiate normally
		if (cache == null) {
			return Instantiate (prefab, position, rotation) as GameObject;
		}

		// Find the next object in the cache
		GameObject obj = cache.GetNextObjectInCache ();

		// Set the position and rotation of the object
		obj.transform.position = position;
		obj.transform.rotation = rotation;

		// Set the object to be active
		obj.SetActive (true);
		activeCachedObjects [obj] = true;
		
//		Debug.Log("obj "+obj.name+" is got from pool...");
		return obj;
	}

	public void destroyObj (GameObject objectToDestroy)
	{
		if (activeCachedObjects.ContainsKey (objectToDestroy)) {
//			Debug.Log("obj "+objectToDestroy.name+" is back to pool...");
			objectToDestroy.SetActive (false);
			activeCachedObjects [objectToDestroy] = false;
			objectToDestroy.transform.parent 
				= ObjectPool.instance.transform;
		} else {
			Destroy (objectToDestroy);
		}
	}
}
