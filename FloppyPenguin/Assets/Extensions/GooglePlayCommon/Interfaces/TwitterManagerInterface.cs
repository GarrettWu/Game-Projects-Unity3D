using UnityEngine;
using System.Collections;

public interface TwitterManagerInterface  {
	
	void Init(string consumer_key, string consumer_secret);
	void AuthificateUser();
	void LoadUserData();
	void Post(string status);
	void Post(string status, Texture2D texture);
	void LogOut();

	bool IsAuthed {get;}
	TwitterUserInfo userInfo  {get;}


	void addEventListener(string eventName, EventHandlerFunction handler);
	void addEventListener(string eventName, DataEventHandlerFunction handler);

}
