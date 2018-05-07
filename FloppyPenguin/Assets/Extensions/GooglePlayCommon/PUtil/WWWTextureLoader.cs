using UnityEngine;
using System.Collections;

public class WWWTextureLoader : EventDispatcher {

	private string _url;

	public static WWWTextureLoader Create() {
		return new GameObject("WWWTextureLoader").AddComponent<WWWTextureLoader>();
	}

	public void LoadTexture(string url) {
		_url = url;
		StartCoroutine(LoadCoroutin());
	}


	private IEnumerator LoadCoroutin () {
		// Start a download of the given URL
		WWW www = new WWW (_url);

		// Wait for download to complete
		yield return www;

		dispatch(BaseEvent.LOADED, www.texture);
	}

}
