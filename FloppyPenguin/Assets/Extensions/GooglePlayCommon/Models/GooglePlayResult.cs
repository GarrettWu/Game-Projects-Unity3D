////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Native Plugin 
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////


public class GooglePlayResult {


	private GooglePlayResponceCode _response;
	private string _message;

	public string leaderboardId = "";
	public string achievementId = "";


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public GooglePlayResult(string code) {
		_response = PlayServiceUtil.GetGPCodeFromInt(System.Convert.ToInt32(code));
		_message = _response.ToString ();
	}



	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public GooglePlayResponceCode response {
		get {
			return _response;
		}
	}

	public string message {
		get {
			return _message;
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