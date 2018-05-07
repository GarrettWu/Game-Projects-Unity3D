////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;

public interface GoogleMobileAdBanner  {
	
	void Hide();
	void Show();
	void Refresh();


	int id {get;}

	bool IsLoaded {get;}
	bool IsOnScreen {get;}
	bool ShowOnLoad{get; set;}

	GADBannerSize size {get;}
	TextAnchor anchor {get;}




	void addEventListener(string eventName, EventHandlerFunction handler);
	void addEventListener(string eventName, DataEventHandlerFunction handler);

}
