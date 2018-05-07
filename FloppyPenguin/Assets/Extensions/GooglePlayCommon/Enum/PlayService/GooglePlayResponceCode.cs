////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Native Plugin 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using System;


public enum GooglePlayResponceCode {
	STATUS_OK,
	STATUS_INTERNAL_ERROR,
	STATUS_NETWORK_ERROR_OPERATION_DEFERRED,
	STATUS_NETWORK_ERROR_OPERATION_FAILED,
	STATUS_NETWORK_ERROR_NO_DATA,
	STATUS_CLIENT_RECONNECT_REQUIRED,
	STATUS_LICENSE_CHECK_FAILED,
	STATUS_NETWORK_ERROR_STALE_DATA,
	UNKNOWN_ERROR,


	STATUS_ACHIEVEMENT_UNLOCKED,
	STATUS_ACHIEVEMENT_UNKNOWN,
	STATUS_ACHIEVEMENT_NOT_INCREMENTAL,
	STATUS_ACHIEVEMENT_UNLOCK_FAILURE,

	STATUS_STATE_KEY_NOT_FOUND,
	STATUS_STATE_KEY_LIMIT_EXCEEDED
}


