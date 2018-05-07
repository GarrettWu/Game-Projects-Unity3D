////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using System;


public static class PlayServiceUtil {





	public static GooglePlayResponceCode GetGPCodeFromInt(int code) {
		switch(code) {
		case 0:
			return GooglePlayResponceCode.STATUS_OK;
		case 1:
			return GooglePlayResponceCode.STATUS_INTERNAL_ERROR;
		case 2:
			return GooglePlayResponceCode.STATUS_CLIENT_RECONNECT_REQUIRED;
		case 3:
			return GooglePlayResponceCode.STATUS_NETWORK_ERROR_STALE_DATA;
		case 4:
			return GooglePlayResponceCode.STATUS_NETWORK_ERROR_NO_DATA;
		case 5:
			return GooglePlayResponceCode.STATUS_NETWORK_ERROR_OPERATION_DEFERRED;
		case 6:
			return GooglePlayResponceCode.STATUS_NETWORK_ERROR_OPERATION_FAILED;
		case 7:
			return GooglePlayResponceCode.STATUS_LICENSE_CHECK_FAILED;

		case 2002:
			return GooglePlayResponceCode.STATUS_STATE_KEY_NOT_FOUND;
		case 2003:
			return GooglePlayResponceCode.STATUS_STATE_KEY_LIMIT_EXCEEDED;
		
		case 3000:
			return GooglePlayResponceCode.STATUS_ACHIEVEMENT_UNLOCK_FAILURE;
		case 3001:
			return GooglePlayResponceCode.STATUS_ACHIEVEMENT_UNKNOWN;
		case 3002:
			return GooglePlayResponceCode.STATUS_ACHIEVEMENT_NOT_INCREMENTAL;
		case 3003:
			return GooglePlayResponceCode.STATUS_ACHIEVEMENT_UNLOCKED;
		default:
			return GooglePlayResponceCode.UNKNOWN_ERROR;
		}
	} 


	public static GPCollectionType GetCollectionTypeByInt(int code) {
		switch(code) {
		case 0:
			return GPCollectionType.COLLECTION_PUBLIC;
		default:
			return GPCollectionType.COLLECTION_SOCIAL;
		}
	}

	public static GPBoardTimeSpan GetTimeSpanByInt(int code) {
		switch(code) {
		case 0:
			return GPBoardTimeSpan.TIME_SPAN_DAILY;
		case 1:
			return GPBoardTimeSpan.TIME_SPAN_WEEKLY;
		default:
			return GPBoardTimeSpan.TIME_SPAN_ALL_TIME;
		}
	}


	public static GPAchievementState GetAchievementStateById(int code) {
		switch(code) {
			case 0:
			return GPAchievementState.STATE_UNLOCKED;
			case 1:
			return GPAchievementState.STATE_REVEALED;
			default:
			return GPAchievementState.STATE_HIDDEN;
		}
	}


	public static GPAchievementType GetAchievementTypeById(int code) {
		switch(code) {
			case 0:
			return GPAchievementType.TYPE_STANDARD;
			default:
			return GPAchievementType.TYPE_INCREMENTAL;
		}
	}


}


