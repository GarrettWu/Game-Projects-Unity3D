////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;

public class LeaderBoardScoreVariant  {


	private int _rank;
	private int _score;

	private GPCollectionType _collection;
	private GPBoardTimeSpan _timeSpan;


	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	public LeaderBoardScoreVariant(string vScore, string vRank, string vTimeSpan, string sCollection) {
		_score = System.Convert.ToInt32 (vScore);
		_rank = System.Convert.ToInt32 (vRank);


		_timeSpan  = PlayServiceUtil.GetTimeSpanByInt (System.Convert.ToInt32(vTimeSpan));
		_collection = PlayServiceUtil.GetCollectionTypeByInt (System.Convert.ToInt32(sCollection));

	}


	//--------------------------------------
	// GET / SET
	//--------------------------------------


	public int rank {
		get {
			return _rank;
		}
	}


	public int score {
		get {
			return _score;
		}
	}
	

	public GPCollectionType collection {
		get {
			return _collection;
		}
	}


	public GPBoardTimeSpan timeSpan {
		get {
			return _timeSpan;
		}
	}

}
