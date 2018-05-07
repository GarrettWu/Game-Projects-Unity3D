// Usage example of Google Universal Analytics.
//
// Copyright 2013 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 815 $
//
// File version history:
// 2013-09-01, 1.1.1 - Initial version
// 2013-09-25, 1.1.3 - Unity 3.5 support.
// 2013-12-17, 1.2.0 - Added warning for missing Analytics object and check
//                     for Analytics.gua.analyticsDisabled in custom Quit hit.
// 2014-05-12, 1.4.0 - Updated for method renames. (see GoogleUniversalAnalytics.cs)
//                     Refined showing of network status and added showing
//                     count of remaining entries in offline hit cache.

using UnityEngine;
using System.Collections;

public class AnalyticsExampleMainScreen : MonoBehaviour
{
    void OnGUI()
    {
        if (Analytics.Instance == null)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(" ERROR! No Analytics object in scene!");
            GUILayout.Label(" Add Analytics script to an active game object.");
            GUILayout.EndVertical();
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("v");
        GUILayout.Label(Analytics.Instance.appVersion);

        GUILayout.BeginVertical();
        GUILayout.Label("- Google Universal Analytics for Unity");
        GUILayout.Label(" Current scene: " + Application.loadedLevelName + "\n");

        // Possibility to switch between scenes demonstrates the
        // automatic screen events sent by Analytics.OnLevelWasLoaded().
        //
        // For this test you need to add both AnalyticsExample and
        // AnalyticsExampleSecondaryScene scenes to the project
        // using File->Build Settings.
        //
        GUILayout.Label("Scene switch sends automatic screen view events:");
        if (GUILayout.Button("Go to Secondary Scene\n(Opt-out example & more)"))
            Application.LoadLevel("AnalyticsExampleSecondaryScene");

        GUILayout.Label("Buttons to send imaginary screen switch events:");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("\"Menuscreen A\""))
            Analytics.changeScreen("AnalyticsExample - Menuscreen A");
        if (GUILayout.Button("\"Menuscreen B\""))
            Analytics.changeScreen("AnalyticsExample - Menuscreen B");
        GUILayout.EndHorizontal();


        GUILayout.Label("\nSocial hits and events - Links to Strobotnik:");
        // This is just an inspirational example. In reality you should
        // integrate official social SDKs and probably send the "Like"
        // type of analytics hit only when user actually does that
        // inside your application.
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Google+"))
        {
            Analytics.gua.sendSocialHit("GooglePlus", "plus", "StrobotnikGooglePlus");
            Application.OpenURL("http://plus.google.com/101873213646861422131");
        }
        if (GUILayout.Button("Facebook"))
        {
            Analytics.gua.sendSocialHit("Facebook", "like", "StrobotnikFacebook");
            Application.OpenURL("http://facebook.com/strobotnik");
        }
        if (GUILayout.Button("Twitter"))
        {
            Analytics.gua.sendSocialHit("Twitter", "follow", "StrobotnikTwitter");
            Application.OpenURL("http://twitter.com/strobotnik");
        }
        if (GUILayout.Button(" Web "))
        {
            Analytics.gua.sendEventHit("OpenWebsite", "Strobotnik.com");
            Application.OpenURL("http://strobotnik.com");
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("\nSend errors and exceptions to analytics:");
        if (Analytics.Instance.sendExceptions)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Log Error"))
            {
                Debug.LogError("Logged Error to analytics", this);
            }
            if (GUILayout.Button("Divide by zero"))
            {
                // Cause an exception to be sent to analytics
                int b = 0;
                int a = 31337 / b;
                Debug.Log("" + a); // won't get here
            }
            GUILayout.EndHorizontal();
        }
        else
            GUILayout.Label("(Analytics.sendExceptions is disabled)");

        GUILayout.Label("---");

        GUILayout.Label("Remaining entries in offline hit cache:");
        #if UNITY_WEBPLAYER
        GUILayout.Label("(not applicable with web player)");
        #else
        if (Analytics.gua != null)
            GUILayout.Label(Analytics.gua.remainingEntriesInOfflineCache.ToString());
        #endif


        if (GUILayout.Button("Quit"))
        {
            // End session with custom built hit:
            if (!Analytics.gua.analyticsDisabled)
            {
                Analytics.gua.beginHit(GoogleUniversalAnalytics.HitType.Screenview);
                Analytics.gua.addScreenName("AnalyticsExample - Quit");
                Analytics.gua.addSessionControl(false); // end current session
                Analytics.gua.sendHit();
            }
            #if UNITY_3_5
            gameObject.active = false;
            #else
            gameObject.SetActive(false);
            #endif
            Application.Quit();
        }

        GUILayout.Label("Verified internet access: " + Analytics.gua.internetReachable);

        string networkReachability = "Unity NetworkReachability: none";
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            networkReachability = "Unity NetworkReachability: via carrier data network";
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            networkReachability = "Unity NetworkReachability: via local area network";
        GUILayout.Label(networkReachability);

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    void Update()
    {
        float t = Time.fixedTime;
        Camera.main.backgroundColor = new Color(
            Mathf.Sin(t * 0.39f) * 0.2f + 0.25f,
            Mathf.Sin(t * 0.23f) * 0.2f + 0.25f,
            Mathf.Sin(t * 0.55f) * 0.2f + 0.25f);
    }
}
