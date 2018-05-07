using UnityEngine;
using System.Collections;

public class GP_Partisipant : MonoBehaviour {

	private string _id;
	private string _playerid;
	private GP_RTM_ParticipantStatus _status = GP_RTM_ParticipantStatus.STATUS_UNRESPONSIVE;


	public GP_Partisipant(string uid, string playerUid, string stat) {
		_id = uid;
		_playerid = playerUid;
		_status = (GP_RTM_ParticipantStatus) System.Convert.ToInt32(stat);
	}

	public string id {
		get {
			return _id;
		}
	}

	public string playerId {
		get {
			return _playerid;
		}
	}

	public GP_RTM_ParticipantStatus status {
		get {
			return _status;
		}
	}
}
