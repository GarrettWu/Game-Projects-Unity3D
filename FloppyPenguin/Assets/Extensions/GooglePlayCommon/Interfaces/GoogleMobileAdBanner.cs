////////////////////////////////////////////////////////////////////////////////
//  
// @module <module_name>
// @author Osipov Stanislav lacost.st@gmail.com
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
