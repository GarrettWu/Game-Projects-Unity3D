////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Native Plugin 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

public class GoogleCloudResult {

	private GooglePlayResponceCode _response;
	private string _message;

	private int _stateKey;


	public byte[] stateData;
	public byte[] serverConflictData;
	public string resolvedVersion;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public GoogleCloudResult(string code) {
		_response = PlayServiceUtil.GetGPCodeFromInt(System.Convert.ToInt32(code));
		_message = _response.ToString ();
	}

	public GoogleCloudResult (string code, string key) {
		_response = PlayServiceUtil.GetGPCodeFromInt(System.Convert.ToInt32(code));
		_message = _response.ToString ();

		_stateKey = System.Convert.ToInt32 (key);
	}
	

	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public GooglePlayResponceCode response {
		get {
			return _response;
		}
	}
	
	public string stateDataString {
		get {
			if(stateData == null) {
				return String.Empty;
			} else {
				return  System.Text.Encoding.UTF8.GetString(stateData); 
			}
			
		}
	}
	
	public string serverConflictDataString {
		get {
			if(serverConflictData == null) {
				return String.Empty;
			} else {
				return System.Text.Encoding.UTF8.GetString(serverConflictData); 
			}
			
		}
	}

	public string message {
		get {
			return _message;
		}
	}

	public int stateKey {
		get {
			return _stateKey;
		}
	}


	public bool isSuccess  {
		get {
			return _response == GooglePlayResponceCode.STATUS_OK;
		}
	}

	public bool isFailure {
		get {
			return !isSuccess;
		}
	}

}


