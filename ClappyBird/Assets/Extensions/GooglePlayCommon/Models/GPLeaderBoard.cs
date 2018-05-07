////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GPLeaderBoard  {

	private List<LeaderBoardScoreVariant>  _scores = new List<LeaderBoardScoreVariant>();


	private string _id;
	private string _name;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	public GPLeaderBoard(string lId, string lName) {
		_id = lId;
		_name = lName;
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------


	public void addScoreVarian(LeaderBoardScoreVariant varian) {
		_scores.Add (varian);
	}

	public int GetScore (GPCollectionType collection, GPBoardTimeSpan timeSpan) {
		foreach(LeaderBoardScoreVariant variant in _scores) {
			if(variant.collection == collection && variant.timeSpan == timeSpan) {
				return variant.score;
			}
		}

		return -1;
	}

	public int GetRank (GPCollectionType collection, GPBoardTimeSpan timeSpan) {
		foreach(LeaderBoardScoreVariant variant in _scores) {
			if(variant.collection == collection && variant.timeSpan == timeSpan) {
				return variant.rank;
			}
		}

		return -1;
	}
	
	public LeaderBoardScoreVariant GetVariant (GPCollectionType collection, GPBoardTimeSpan timeSpan) {
		foreach(LeaderBoardScoreVariant variant in _scores) {
			if(variant.collection == collection && variant.timeSpan == timeSpan) {
				return variant;
			}
		}

		return null;
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

	public List<LeaderBoardScoreVariant>  scores {
		get {
			return _scores;
		}
	}





}
