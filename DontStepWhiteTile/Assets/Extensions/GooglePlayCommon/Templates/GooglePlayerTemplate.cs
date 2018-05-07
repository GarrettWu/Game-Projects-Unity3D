////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;

public class GooglePlayerTemplate {
	
	private string _id;
	private string _name;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	public GooglePlayerTemplate(string pId, string pName) {
		_id = pId;
		_name = pName;
	} 


	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public string id {
		get {
			return _id;
		}
	}

	public string name {
		get {
			return _name;
		}
	}

}
