using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AndroidNotificationManager : SA_Singleton<AndroidNotificationManager>  {
	public const int LENGTH_SHORT = 0; // 2 seconds 
	public const int LENGTH_LONG  = 1; // 3.5 seconds



	//Actions
	public Action<int> OnNotificationIdLoaded = delegate{};

	
	//Events
	public const string  NOTIFICATION_ID_LOADED = "notification_id_loaded";


	private const string PP_KEY = "AndroidNotificationManagerKey";
	private const string PP_ID_KEY = "AndroidNotificationManagerKey_ID";
	private const string DATA_SPLITTER = "|";


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void LocadAppLaunchNotificationId() {
		AndroidNative.requestCurrentAppLaunchNotificationId();
	}
	
	public void ShowToastNotification(string text) {
		ShowToastNotification (text, LENGTH_SHORT);
	}
	
	public void ShowToastNotification(string text, int duration) {
		AndroidNative.ShowToastNotification (text, duration);
	}

	public int ScheduleLocalNotification(string title, string message, int seconds) {

		int id = GetNextId;
		AndroidNative.ScheduleLocalNotification(title, message, seconds, id);


		LocalNotificationTemplate notification =  new LocalNotificationTemplate(id, title, message, DateTime.Now.AddSeconds(seconds));

		List<LocalNotificationTemplate> scheduled = LoadPendingNotifications();
		scheduled.Add(notification);

		SaveNotifications(scheduled);

		return id;

	}


	public void CancelLocalNotification(int id, bool clearFromPrefs = true) {
		AndroidNative.CanselLocalNotification(id);

		if(clearFromPrefs) {
			List<LocalNotificationTemplate> scheduled = LoadPendingNotifications();
			List<LocalNotificationTemplate> newList =  new List<LocalNotificationTemplate>();

			foreach(LocalNotificationTemplate n in scheduled) {
				if(n.id != id) {
					newList.Add(n);
				}
			}


			SaveNotifications(newList);

		}
	}


	public void CancelAllLocalNotifications() {
		List<LocalNotificationTemplate> scheduled = LoadPendingNotifications();

		foreach(LocalNotificationTemplate n in scheduled) {
			CancelLocalNotification(n.id, false);
		}

		SaveNotifications(new List<LocalNotificationTemplate>());
	}

	// --------------------------------------
	// Events
	// --------------------------------------


	private void OnNotificationIdLoadedEvent(string data)  {
		int id = System.Convert.ToInt32(data);

		OnNotificationIdLoaded(id);
		dispatch(NOTIFICATION_ID_LOADED, id);

	}

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------


	private int GetNextId {
		get {
			int id = 1;
			if(PlayerPrefs.HasKey(PP_ID_KEY)) {
				id = PlayerPrefs.GetInt(PP_ID_KEY);
				id++;
			} 
			
			PlayerPrefs.SetInt(PP_ID_KEY, id);
			return id;
		}

	}

	private void SaveNotifications(List<LocalNotificationTemplate> notifications) {

		if(notifications.Count == 0) {
			PlayerPrefs.DeleteKey(PP_KEY);
			return;
		}

		string srialzedNotifications = "";
		int len = notifications.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				srialzedNotifications += DATA_SPLITTER;
			}
			
			srialzedNotifications += notifications[i].SerializedString;
		}

		PlayerPrefs.SetString(PP_KEY, srialzedNotifications);

	}


	public  List<LocalNotificationTemplate> LoadPendingNotifications() {

		string data = string.Empty;
		if(PlayerPrefs.HasKey(PP_KEY)) {
			data = PlayerPrefs.GetString(PP_KEY);
		}
		List<LocalNotificationTemplate>  tpls = new List<LocalNotificationTemplate>();

		if(data != string.Empty) {
			string[] notifications = data.Split(DATA_SPLITTER [0]);
			foreach(string n in notifications) {

				String templateData = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(n) );
			
				LocalNotificationTemplate notification = new LocalNotificationTemplate(templateData);
				if(!notification.IsFired) {
					tpls.Add(notification);
				}
			}
		}

		return tpls;
	}



}

