////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

public class IOSNativeMenu : EditorWindow {
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	#if UNITY_EDITOR


	//--------------------------------------
	//  EDIT
	//--------------------------------------

	[MenuItem("Window/IOS Native/Edit Settings")]
	public static void Edit() {
		Selection.activeObject = IOSNativeSettings.Instance;
	}


	//--------------------------------------
	//  DOCS
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/Game Center")]
	public static void d1() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.kwtszax5fimt";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/In-App Purchases")]
	public static void d2() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.2sukgxloiuxp";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/iCloud")]
	public static void d3() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.wykxwpo2wbsc";
		Application.OpenURL(url);
	}

	
	[MenuItem("Window/IOS Native/Documentation/iAd App Network")]
	public static void d4() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.wn9la0njw77p";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Other features")]
	public static void d5() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.pm3lzvvl3sb5";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  PLAY MAKER
	//--------------------------------------

	[MenuItem("Window/IOS Native/PlayMaker/Documentation")]
	public static void p1() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.dbwazi62lyk7";
		Application.OpenURL(url);
	}


	[MenuItem("Window/IOS Native/PlayMaker/Forum Thread")]
	public static void p2() {
		string url = "http://forum.unity3d.com/threads/243032-IOS-Native-PlayMaker-Action?p=1607764#post1607764";
		Application.OpenURL(url);
	}

	
	
	//--------------------------------------
	//  GUIDES
	//--------------------------------------


	[MenuItem("Window/IOS Native/Guides/Creating Certificate and Provocation profile")]
	public static void g1() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.9a49rulolaia";
		Application.OpenURL(url);
	}


	[MenuItem("Window/IOS Native/Guides/Creating iTunes app")]
	public static void g2() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.p7pg7j96fu49";
		Application.OpenURL(url);
	}


	[MenuItem("Window/IOS Native/Guides/Manage Game Center")]
	public static void g3() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.qmf4c2ah3lzn";
		Application.OpenURL(url);
	}


	[MenuItem("Window/IOS Native/Guides/Game Center Coding Guidelines")]
	public static void g4() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.67obzp6pacpg";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Guides/Manage In-App Purchases")]
	public static void g5() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.a72hr7iuk533";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Guides/In-App Purchases Coding Guidelines")]
	public static void g6() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.cel3x570amb3";
		Application.OpenURL(url);
	}


	[MenuItem("Window/IOS Native/Guides/iCloud Setup")]
	public static void g7() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.89hijr7070qe";
		Application.OpenURL(url);
	}



	//--------------------------------------
	//  IMPORTANT
	//--------------------------------------

	[MenuItem("Window/IOS Native/Know Issues")]
	public static void KnowIssues() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.sf462dgtgpc2";
		Application.OpenURL(url);
	}


	[MenuItem("Window/IOS Native/Plugin Setup")]
	public static void PluginSetup() {
		string url = "https://docs.google.com/document/d/1IT1qIuM6QsqV7D--69uv0sn4OSQ24UQKos3x0P3BQjc/edit#heading=h.k02y0jmoedxn";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Support")]
	public static void Support() {
		string url = "https://www.assetstore.unity3d.com/#publisher/2256";
		Application.OpenURL(url);
	}

	

	#endif

}
