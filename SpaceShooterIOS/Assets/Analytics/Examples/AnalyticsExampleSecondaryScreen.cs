// Usage example of Google Universal Analytics.
//
// Copyright 2013 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 821 $
//
// File version history:
// 2013-09-01, 1.1.1 - Initial version
// 2013-09-25, 1.1.3 - Unity 3.5 support.
// 2013-12-17, 1.2.0 - Added user opt-out from analytics toggle.

using UnityEngine;
using System.Collections;

public class AnalyticsExampleSecondaryScreen : MonoBehaviour
{
    void OnGUI()
    {
        if (Analytics.gua == null)
        {
            // Error - AnalyticsExampleSecondaryScreen needs Analytics
            // object to be present which has been initialized by the
            // main AnalyticsExample scene.
            GUILayout.BeginVertical();
            GUILayout.Label("Error: No Analytics.gua object!\n");
            GUILayout.Label("AnalyticsExampleSecondaryScene works only when switched to from the main AnalyticsExample scene.");
            GUILayout.EndVertical();
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label(" ");
        GUILayout.BeginVertical();

        GUILayout.Label(" Current scene: " + Application.loadedLevelName);
        GUILayout.Label(" ");
        GUILayout.Label(" This scene demonstrates automatic screen switch\n" +
                        " events sent by the analytics example, and is an\n" +
                        " example of options screen allowing user to\n" +
                        " opt-out from analytics.");
        GUILayout.Label(" ");

        GUILayout.Label(" This app sends anonymous usage statistics over internet.");
        bool disableAnalyticsByUserOptOut = Analytics.gua.analyticsDisabled;
        bool newValue = GUILayout.Toggle(disableAnalyticsByUserOptOut, "Opt-out from anonymous statistics.");
        if (disableAnalyticsByUserOptOut != newValue)
            Analytics.setPlayerPref_disableAnalyticsByUserOptOut(newValue);

        GUILayout.Label(disableAnalyticsByUserOptOut ? " :-(\n" : " \n");

        GUILayout.Label("\nMore from Strobotnik:");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Pixel-Perfect\nDynamic Text\n(!!)"))
        {
            Analytics.gua.sendEventHit("OpenWebsite", "bitly.com/DynTextUnity");
            Application.OpenURL("http://bitly.com/DynTextUnity");
        }
        if (GUILayout.Button("Internet\nReachability\nVerifier"))
        {
            Analytics.gua.sendEventHit("OpenWebsite", "j.mp/IRVUNAS");
            Application.OpenURL("http://j.mp/IRVUNAS");
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("\n");
        if (GUILayout.Button("Back to Main"))
            Application.LoadLevel("AnalyticsExample");
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}
