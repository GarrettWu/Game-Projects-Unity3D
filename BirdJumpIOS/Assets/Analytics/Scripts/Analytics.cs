﻿// Analytics integration example for GoogleUniversalAnalytics helper class.
//
// Copyright 2013,2014 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 830 $
//
// File version history:
// 2013-04-26, 1.0.0 - Initial version
// 2013-05-03, 1.0.1 - Automatic events with OnLevelWasLoaded.
// 2013-09-01, 1.1.1 - Granularized some of the system info statistics.
//                     Different way to generate client ID for Android.
//                     Send 1st launch data only when network is reachable.
// 2013-09-25, 1.1.3 - Unity 3.5 support.
// 2013-12-12, 1.1.4 - Fix for trying to use Handheld class on Windows 8.
// 2013-12-17, 1.2.0 - Added disableAnalyticsByUserOptOut in PlayerPrefs
//                     and use of gua.analyticsDisabled.
// 2014-02-11, 1.3.0 - Changed trackingID to be invalid dummy by default.
// 2014-05-12, 1.4.0 - Added support for offline cache (net activity coroutine
//                     as part of that). Updated for method renames
//                     (see GoogleUniversalAnalytics.cs). Added
//                     ScreenResolution and ViewportSize to the 1st launch
//                     SystemInfo statistics, and removed viewport size from
//                     being submitted for the first screen hit. 
// 2014-06-16, 1.4.2 - Changed method of generating anonymous client ID.
// 2014-08-29, 1.5.0 - Added optional catching and sending of app errors and
//                     exceptions to analytics. Added tooltips for component
//                     (for Unity 4.5 or newer). Changed SystemInfo events to
//                     be non-interactive and made sending of them an option.
//                     Moved user language setting to be part of the auto-added
//                     hit data. SystemInfo changes for Unity 5.0+: Removed
//                     graphicsPixelFillrate and added graphicsMultiThreaded.

using UnityEngine;
using System.Collections;

public class Analytics : MonoBehaviour
{
    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Tracking ID from Google Analytics site:\nAdmin→Property→Tracking Info→Tracking Code")]
    #endif
    public string trackingID = "UA-XXXXXXX-Y"; // dummy id - use your actual id!

    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Name of your application\n(required for App tracking views)")]
    #endif
    public string appName = "GoogleUniversalAnalyticsForUnityExample";

    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Version of your application\n(required for App tracking views)")]
    #endif
    public string appVersion = "0.0.1";

    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Prefix for automatic screen view events\n(Sent after loading a new level)")]
    #endif
    public string newLevelAnalyticsEventPrefix = "level-";

    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Use HTTPS or plain HTTP to send analytics")]
    #endif
    public bool useHTTPS = false;

    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Use offline cache and auto-send cached hits in background when online.\n(Setting cannot be changed after initialization.)")]
    #endif
    // Note: switching useOfflineCache after initialization doesn't have any effect.
    public bool useOfflineCache = true;

    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Send SystemInfo client statistics on first launch.")]
    #endif
    public bool sendSystemInfo = true;

    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Automatically send logged errors and exceptions to analytics.\n(Uses Application.RegisterLogCallback, see Analytics.Callback_HandleLog)")]
    #endif
    public bool sendExceptions = true;


    // Common public instance of the internal analytics helper script.
    public static GoogleUniversalAnalytics gua = null;

    // Private default instance.
    private static Analytics instance = null;
    // The default analytics component instance as a property.
    public static Analytics Instance { get { return instance; } }

    private string sceneName = "";

    private const string disableAnalyticsByUserOptOutPrefKey = "GoogleUniversalAnalytics_optOut";

    string offlineCacheFileName = "GUA-offline-queue.dat";


    int getPOSIXTime()
    {
        return (int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
    }


    // If analyticsDisabled is true, all analytics is disabled = no hits are sent.
    // If analyticsDisabled is false, analytics are enabled and hits will be sent
    // unless there is no internet reachability.
    // This setting is persistent (saved to PlayerPrefs).
    public static void setPlayerPref_disableAnalyticsByUserOptOut(bool analyticsDisabled)
    {
        if (gua != null)
            gua.analyticsDisabled = analyticsDisabled;
        PlayerPrefs.SetInt(disableAnalyticsByUserOptOutPrefKey, analyticsDisabled ? 1 : 0);
        PlayerPrefs.Save();
    }


    void Awake()
    {
        //Debug.Log("AnalyticsTesting Awake()");

        // prevent additional Analytics objects being created on level reloads
        if (instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        string clientID = "";
        const string clientIdPrefKey = "GoogleUniversalAnalytics_clientID";
        // If you want to force a new client ID for temporary testing,
        // uncomment following line for a single run:
        //////// PlayerPrefs.DeleteKey(clientIdPrefKey);
        // (remember to disable it again)

        if (PlayerPrefs.HasKey(clientIdPrefKey))
            clientID = PlayerPrefs.GetString(clientIdPrefKey);
        if (clientID.Length < 8 || !PlayerPrefs.HasKey(clientIdPrefKey))
        {
            // Need to generate unique & anonymous client ID for analytics.
            // SystemInfo.deviceUniqueIdentifier would be nice for this, but
            // use of it is abandoned as it can lead to additional required
            // permissions or submission approval problems on some platforms.
            // So we only use a combination of timestamp, random value and
            // some sources of device-specific data to make up an anonymous
            // id which is also sufficiently unique.
            int deviceData = SystemInfo.graphicsDeviceName.GetHashCode() ^ SystemInfo.graphicsDeviceVersion.GetHashCode() ^ SystemInfo.operatingSystem.GetHashCode() ^ SystemInfo.processorType.GetHashCode();
            clientID = getPOSIXTime().ToString("X8") + Random.Range(0, 0x7fffffff).ToString("x8") + deviceData.ToString("X8");

            //Debug.Log("Created client id for analytics: " + clientID);
            PlayerPrefs.SetString(clientIdPrefKey, clientID);
            PlayerPrefs.Save();
        }

        string offlineCacheFilePath = ""; // empty string=disable (in GUA class)
        if (useOfflineCache && offlineCacheFileName != null && offlineCacheFileName.Length > 0)
        {
            offlineCacheFilePath = Application.persistentDataPath + '/' + offlineCacheFileName;
            //Debug.Log("Full path for offline cache: " + offlineCacheFilePath);
        }

        //bool useStringEscaping = true; // see the docs about this
        if (gua == null)
            gua = GoogleUniversalAnalytics.Instance;
        gua.initialize(this, trackingID, clientID, appName, appVersion, useHTTPS, offlineCacheFilePath);
        //gua.setStringEscaping(useStringEscaping); // see the docs about this

        if (PlayerPrefs.HasKey(disableAnalyticsByUserOptOutPrefKey))
        {
            gua.analyticsDisabled = (PlayerPrefs.GetInt(disableAnalyticsByUserOptOutPrefKey, 0) != 0);
        }

        if (!gua.analyticsDisabled)
        {
            // Start by sending a hit with some generic info, including an app
            // screen hit with the first level name, since first scene doesn't get
            // automatically call to OnLevelWasLoaded.
            gua.beginHit(GoogleUniversalAnalytics.HitType.Screenview);
            gua.addScreenResolution(Screen.currentResolution.width, Screen.currentResolution.height);
            gua.addViewportSize(Screen.width, Screen.height);
            gua.addScreenName(newLevelAnalyticsEventPrefix + Application.loadedLevelName);
            gua.sendHit();


            // Next, client SystemInfo statistics are submitted ONCE on the first
            // launch when internet is reachable.

            // If you make a few version upgrades and at some point want to get
            // fresh statistics of your active users, update the category string
            // below and after next update users will re-submit SystemInfo once.
            const string category = "SystemInfo_since_v001";
            const string prefKey = "GoogleUniversalAnalytics_" + category;

            // Existing pref key could be deleted with following command:
            //// PlayerPrefs.DeleteKey(prefKey);
            // Warning: Do not enable that code row here (except for single time
            //          testing). Otherwise all following single time statistics
            //          hits would be sent on each launch.

            if (sendSystemInfo &&
                (useOfflineCache || gua.internetReachable) &&
                !PlayerPrefs.HasKey(prefKey))
            {
                sendSystemInfoEvent(category, "ScreenResolution", "" + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
                sendSystemInfoEvent(category, "ViewportSize", "" + Screen.width + "x" + Screen.height);
                sendSystemInfoEvent(category, "ScreenDPI", ((int)Screen.dpi).ToString(), (int)Screen.dpi);

                sendSystemInfoEvent(category, "operatingSystem", SystemInfo.operatingSystem);
                sendSystemInfoEvent(category, "processorType", SystemInfo.processorType);
                sendSystemInfoEvent(category, "processorCount", SystemInfo.processorCount.ToString(), SystemInfo.processorCount);
                // round down to 128MB chunks for label
                sendSystemInfoEvent(category, "systemMemorySize", (128 * (SystemInfo.systemMemorySize / 128)).ToString(), SystemInfo.systemMemorySize);
                // round down to 16MB chunks for label
                sendSystemInfoEvent(category, "graphicsMemorySize", (16 * (SystemInfo.graphicsMemorySize / 16)).ToString(), SystemInfo.graphicsMemorySize);
                sendSystemInfoEvent(category, "graphicsDeviceName", SystemInfo.graphicsDeviceName);
                sendSystemInfoEvent(category, "graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
                sendSystemInfoEvent(category, "graphicsDeviceID", SystemInfo.graphicsDeviceID.ToString(), SystemInfo.graphicsDeviceID);
                sendSystemInfoEvent(category, "graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID.ToString(), SystemInfo.graphicsDeviceVendorID);
                sendSystemInfoEvent(category, "graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
                sendSystemInfoEvent(category, "graphicsShaderLevel", SystemInfo.graphicsShaderLevel.ToString(), SystemInfo.graphicsShaderLevel);
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
                // (graphicsPixelFillrate is not supported on Unity 5.0+)
                // round down to chunks for label (10-5000 MPix depending on value range)
                int pixelFillrateChunkSize = 5000;
                if (SystemInfo.graphicsPixelFillrate < 40000)
                    pixelFillrateChunkSize = 2000;
                if (SystemInfo.graphicsPixelFillrate < 10000)
                    pixelFillrateChunkSize = 1000;
                else if (SystemInfo.graphicsPixelFillrate < 4000)
                    pixelFillrateChunkSize = 500;
                else if (SystemInfo.graphicsPixelFillrate < 1300)
                    pixelFillrateChunkSize = 100;
                else if (SystemInfo.graphicsPixelFillrate < 200)
                    pixelFillrateChunkSize = 20;
                else if (SystemInfo.graphicsPixelFillrate < 100)
                    pixelFillrateChunkSize = 10;
                sendSystemInfoEvent(category, "graphicsPixelFillrate", (pixelFillrateChunkSize * (SystemInfo.graphicsPixelFillrate / pixelFillrateChunkSize)).ToString(), SystemInfo.graphicsPixelFillrate);
#else // or Unity 5.0+:
                // New additions in Unity 5.0+:
                sendSystemInfoEvent(category, "graphicsMultiThreaded", SystemInfo.graphicsMultiThreaded ? "yes" : "no", SystemInfo.graphicsMultiThreaded ? 1 : 0);
#endif // Unity 5.0+
                sendSystemInfoEvent(category, "deviceType", SystemInfo.deviceType.ToString());
                // round down to 512 chunks for label
#if !UNITY_3_5
                sendSystemInfoEvent(category, "maxTextureSize", (512 * (SystemInfo.maxTextureSize / 512)).ToString(), SystemInfo.maxTextureSize);
                sendSystemInfoEvent(category, "supports3DTextures", SystemInfo.supports3DTextures ? "yes" : "no", SystemInfo.supports3DTextures ? 1 : 0);
                sendSystemInfoEvent(category, "supportsComputeShaders", SystemInfo.supportsComputeShaders ? "yes" : "no", SystemInfo.supportsComputeShaders ? 1 : 0);
                sendSystemInfoEvent(category, "supportsInstancing", SystemInfo.supportsInstancing ? "yes" : "no", SystemInfo.supportsInstancing ? 1 : 0);
                sendSystemInfoEvent(category, "npotSupport", SystemInfo.npotSupport.ToString());
#endif
                sendSystemInfoEvent(category, "supportsShadows", SystemInfo.supportsShadows ? "yes" : "no", SystemInfo.supportsShadows ? 1 : 0);
                sendSystemInfoEvent(category, "supportsRenderTextures", SystemInfo.supportsRenderTextures ? "yes" : "no", SystemInfo.supportsRenderTextures ? 1 : 0);
                sendSystemInfoEvent(category, "supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount.ToString(), SystemInfo.supportedRenderTargetCount);
                sendSystemInfoEvent(category, "deviceModel", SystemInfo.deviceModel);
                sendSystemInfoEvent(category, "supportsAccelerometer", SystemInfo.supportsAccelerometer ? "yes" : "no", SystemInfo.supportsAccelerometer ? 1 : 0);
                sendSystemInfoEvent(category, "supportsGyroscope", SystemInfo.supportsGyroscope ? "yes" : "no", SystemInfo.supportsGyroscope ? 1 : 0);
                sendSystemInfoEvent(category, "supportsLocationService", SystemInfo.supportsLocationService ? "yes" : "no", SystemInfo.supportsLocationService ? 1 : 0);
                sendSystemInfoEvent(category, "supportsVibration", SystemInfo.supportsVibration ? "yes" : "no", SystemInfo.supportsVibration ? 1 : 0);
                sendSystemInfoEvent(category, "supportsImageEffects", SystemInfo.supportsImageEffects ? "yes" : "no", SystemInfo.supportsImageEffects ? 1 : 0);
                PlayerPrefs.SetInt(prefKey, getPOSIXTime());
                PlayerPrefs.Save();
            } // sendSystemInfo
        } // !gua.analyticsDisabled

        if (sendExceptions)
        {
            Application.RegisterLogCallback(Callback_HandleLog);
        }
	} // Awake


    string prevExceptionLogString = "";
    string prevExceptionStrackTrace = "";

    // Callback to handle Logging calls when Analytics.sendExceptions is enabled.
    // NOTE: stackTrace may not be present on non-Development builds!
    void Callback_HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log || type == LogType.Warning)
            return; // ignore normal logging
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            // Map LogType.Error to "non-fatal" exception analytics hit,
            // and LogType.Assert & LogType.Exception to "fatal" one.
            bool isFatal = (type != LogType.Error);
            gua.cancelHit();
            // Don't send same thing over and over again, exception count
            // happening e.g. in Update() can add up very quickly.
            if (!prevExceptionLogString.Equals(logString) ||
                !prevExceptionStrackTrace.Equals(stackTrace))
            {
                gua.sendExceptionHit("(" + type.ToString() + ")" + logString + ":\n" + stackTrace, isFatal);
            }
            prevExceptionLogString = logString ?? "";
            prevExceptionStrackTrace = stackTrace ?? "";
        }
    }

    void OnLevelWasLoaded(int level)
    {
        if (!sceneName.Equals(Application.loadedLevelName))
        {
            //Debug.Log("AnalyticsTesting: app screen event - switch to scene " + Application.loadedLevelName);
            sceneName = Application.loadedLevelName;
            GoogleUniversalAnalytics gua = GoogleUniversalAnalytics.Instance;
            gua.sendAppScreenHit(newLevelAnalyticsEventPrefix + sceneName);
        }
    }

    void OnDisable()
    {
        #if !UNITY_WEBPLAYER
        // so that files won't be kept open when exiting play mode in editor
        gua.closeOfflineCacheFile();
        #endif

        if (sendExceptions)
        {
            Application.RegisterLogCallback(null);
        }
    }

    void Start()
    {
        //exampleAnalyticsTestHits();
    }


    // This method is here for backward compatibility (legacy example).
    // See the sendSystemInfoEvent() method below for an example of
    // a slightly customized event method. See commented-out
    // exampleAnalyticsTestHits() below for more analytics ideas.
    public static void changeScreen(string newScreenName)
    {
        gua.sendAppScreenHit(newScreenName);
    }

    // Customized event helper method for non-interactive system info events.
    // This is almost the same as GoogleUniversalAnalytics.sendEventHit,
    // but the events are flagged as "non-interactive".
    public void sendSystemInfoEvent(string eventCategory, string eventAction, string eventLabel = null, int eventValue = -1)
    {
        if (gua.analyticsDisabled)
            return;

        gua.beginHit(GoogleUniversalAnalytics.HitType.Event);
        gua.addEventCategory(eventCategory);
        gua.addEventAction(eventAction);
        if (eventLabel != null)
            gua.addEventLabel(eventLabel);
        if (eventValue >= 0)
            gua.addEventValue(eventValue);
        gua.addNonInteractionHit();
        gua.sendHit();
    }


    /*
    void exampleAnalyticsTestHits()
    {
        GoogleUniversalAnalytics gua = GoogleUniversalAnalytics.Instance;


        // event for entering "test-start" screen
        gua.sendAppScreenHit("test-start");


        // event in "test-main" category with "test-enter-shop" action,
        // rest of the parameters are optional label and value.
        gua.sendEventHit("test-main", "test-enter-shop", "TestEnterShop", 123);


        // Below is an ad hoc example of a purchase transaction containing
        // bunch of items. NOTE: This uses the earlier "E-Commerce" params
        // of the Measurement Protocol. You should instead consider using
        // the new group of methods in the "Enhanced E-Commerce" category.
        // Read more from the Doxygen docs, GoogleUniversalAnalytics.cs and
        // the official Measurement Protocol specs. There's no any example
        // code for using those methods yet.
        // https://developers.google.com/analytics/devguides/collection/protocol/v1/
        //
        // Note about the currency values below:
        //       Not sure if there is recommendation if item prices should
        //       include or exclude shipping & taxes. For analytics purposes
        //       it may be probably best to compare the two options, pick
        //       one and stick to it. Here we use item prices which include
        //       all the extra costs.
        string transactionID = gua.clientID + UnityEngine.Random.Range(0, 1000000000);
        // here are some test values which are hopefully reasonable enough
        string currencyCode = "EUR"; // http://en.wikipedia.org/wiki/ISO_4217#Active_codes
        int itemQuantity = 10;
        double totalPrice = 9.99;
        double itemPrice = totalPrice / itemQuantity;
        double vatPc = 0.24; // for example - Finland has 24% VAT as of 2013
        double shippingPc = 0.30; // let's use sales channel cost as "shipping", and assume it equals 30% after taxes
        double tax = totalPrice - totalPrice / (1 + vatPc);
        double shipping = (totalPrice - tax) * shippingPc;
        gua.sendTransactionHit(transactionID, "test-affiliation", totalPrice, shipping, tax, "EUR");
        string itemName = "test-item3";
        string itemCode = "TESTSKU003";
        string itemCategory = "test-items";
        gua.sendItemHit(transactionID, itemName, itemPrice, itemQuantity, itemCode, itemCategory, currencyCode);


        // some examples of social media events: (+1 and Like)
        gua.sendSocialHit("GooglePlus", "plus", "test-social-target-gplus");
        gua.sendSocialHit("Facebook", "like", "test-social-target-fb");


        // could log exceptions like this (this one is non-fatal)
        // Note: If a truly fatal exception occurs, handling/sending those to
        //       GUA might not be possible with the Unity's WWW class, so for
        //       reliable hard exception tracking you should look at using some
        //       dedicated packages with OS-native implementation.
        gua.sendExceptionHit("test-exception", false);


        // example of timing event, e.g. for measuring average loading times...
        gua.sendUserTimingHit("loadtimes", "init", 100, "test-loadtimes-init");


        // test custom screenview event of going to screen named "test-end",
        // with the event containing forced end of current session
        //
        // (The default gua.sendSomething()-style helpers check for the
        //  analyticsDisabled as the first thing, but for custom hits it's
        //  good to check that first like we do here. This way we won't
        //  build a hit for nothing to be discarded by gua.sendHit() if
        //  analytics is disabled due to user opt-out, for example.)
        //
        if (!gua.analyticsDisabled)
        {
            gua.beginHit(GoogleUniversalAnalytics.HitType.Screenview);
            gua.addApplicationVersion();
            gua.addScreenName("test-end");
            gua.addSessionControl(false);
            gua.sendHit();
        }
    }
    */
}
