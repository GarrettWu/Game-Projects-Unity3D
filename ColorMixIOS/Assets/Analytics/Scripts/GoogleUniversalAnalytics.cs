// Helper class for submitting hits to Google Universal Analytics.
// Based on the Measurement Protocol documentation.
//
// Copyright 2013,2014 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 830 $
//
// File version history:
// 2013-04-26, 1.0.0 - Initial version
// 2013-05-03, 1.0.1 - Common instance, check for Internet reachability.
// 2013-06-28, 1.1.0 - Support for web player running in browser.
// 2013-09-01, 1.1.1 - Session hit limit debug warning.
// 2013-09-25, 1.1.3 - Unity 3.5 support, app version to sendAppScreenHit.
// 2013-12-17, 1.2.0 - Added link ID and content experiments parameters.
//                     Added bool analyticsDisabled for easier user opt-out.
// 2014-02-11, 1.3.0 - LogWarning when using tracking ID of examples, and
//                     LogError for trying to use the dummy value for it.
//                     Use custom User-Agent on iOS (improve device detection).
//                     Fixed reset of sessionHitCountResetPending.
// 2014-05-12, 1.4.0 - Added offline caching of hits, and netActivity coroutine
//                     for sending cached hits & monitoring net access.
//                     Changed hits to be sent using a coroutine and result
//                     verification to update net access status on failure.
//                     Updates to reflect changes in measurement protocol:
//                     * Renamed HitType.Appview to HitType.Screenview,
//                     * Renamed addContentDescription to addScreenName.
//                     * Added setUserID, setIPOverride, setUserAgentOverride,
//                       setApplicationID, addApplicationInstallerID.
//                     Also added setApplicationName and replaced
//                     addApplicationVersion with setApplicationVersion. Now
//                     app name, version and ID are always added to hits if
//                     they have been set.
// 2014-06-16, 1.4.2 - Fixed offline cache for Windows Store builds, added
//                     use of custom User-Agent on OSX.
// 2014-08-29, 1.5.0 - Added custom user agent construction and usage for
//                     Windows desktop and Linux standalone builds.
//                     Added parameter category to start of the short doxygen
//                     doc info strings. Added big list of new "Enhanced
//                     E-Commerce" methods. If you use these, you should replace
//                     existing E-Commerce code with new code using these.
//                     Replaced addAnonymizeIP with setAnonymizeIP. Replaced
//                     addUserLanguage with setUserLanguage (automatically
//                     initialized). Added cancelHit. Refined offline cached
//                     hit handling to cancel sending of hits with time way too
//                     far away from current time. Only for webplayer builds:
//                     Added support for null url to addDocumentReferrer method,
//                     in which case web page document.referrer will be used
//                     instead.

#if UNITY_EDITOR
#define DEBUG
// it's good idea to keep DEBUG_WARNINGS enabled when running in editor!
#define DEBUG_WARNINGS
//#define DEBUG_LOGS
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
#if NETFX_CORE
using LegacySystem.IO;
#endif


//! Helper class for sending data to Google Universal Analytics.
/*! Based on the Measurement Protocol format/specification. Sending is done by
 *  HTTP requests using Unity's own WWW class on most platforms, or delegated
 *  to the browser on web player platforms.
 *
 * \note Try using the Google Analytics Query Explorer,
 *       which uses the Core Reporting API:
 *       http://ga-dev-tools.appspot.com/explorer/
 * \note Additional helpful documentation from Google:
 *       https://developers.google.com/analytics/devguides/collection/protocol/v1/
 *       https://developers.google.com/analytics/devguides/collection/gajs/eventTrackerGuide
 * \note Maximum post data payload per hit is 8192 bytes, but a typical hit
 *       payload is perhaps roughly 100-200 bytes or so.
 */
public sealed class GoogleUniversalAnalytics
{
    //! Hit types to be used with beginHit().
    public enum HitType
    {
        Pageview, Screenview, Event, Transaction, Item, Social, Exception, Timing, None
    }

    private readonly static string[] hitTypeNames = new string[9]
    {
        "pageview", "screenview", "event", "transaction", "item", "social", "exception", "timing", "none"
    };

    //! Product actions for Enhanced E-Commerce hits.
    public enum ProductAction
    {
        Detail, Click, Add, Remove, Checkout, CheckoutOption, Purchase, Refund
    }

    private readonly static string[] productActionNames = new string[8]
    {
        "detail", "click", "add", "remove", "checkout", "checkout_option", "purchase", "refund"
    };

    private readonly static string httpCollectUrl = "http://www.google-analytics.com/collect";
    private readonly static string httpsCollectUrl = "https://ssl.google-analytics.com/collect";

    private readonly static string guaVersionData = "v=1"; // Measurement Protocol version

    private string defaultHitData;

    // Post data for a Hit is built in this string builder first.
    private StringBuilder sb = new StringBuilder(256, 8192);
    // Current Hit type being built.
    private HitType hitType = HitType.None;

    // By default, parameter strings are escaped with WWW.EscapeURL for safety.
    // Escaping can be turned off in Initialize.
    private delegate string Delegate_escapeString(string s);
    private Delegate_escapeString escapeString = WWW.EscapeURL;

    // Private default instance.
    private static readonly GoogleUniversalAnalytics instance = new GoogleUniversalAnalytics();

    //! The default instance as a property.
    public static GoogleUniversalAnalytics Instance { get { return instance; } }

    //! Flag to define if https should be used or not when submitting hits.
    public bool useHTTPS;
    //! Reference to helper MonoBehaviour used for Coroutines.
    public MonoBehaviour helperBehaviour;
    //! Current tracking ID for Google Analytics, like "UA-XXXXXXX-Y".
    public string trackingID;
    //! Current anonymous client ID.
    public string clientID;
    //! Current user ID.
    public string userID;
    //! Current user IP (override implicit).
    public string userIP;
    //! Current user agent (override implicit).
    public string userAgent;
    //! (Optional) Current app name. For app tracking analytics views.
    public string appName;
    //! (Optional) Current app version. For app tracking analytics views.
    public string appVersion;
    //! (Optional) Current app ID. For app tracking analytics views.
    public string appID;
    //! Is IP anonymized?
    public bool anonymizeIP;
    //! User's language (e.g. "en" or "en-us").
    public string userLanguage;

    //! Flag to specify if analytics are fully disabled (e.g. because of user opt-out).
    private bool analyticsDisabled_;
    public bool analyticsDisabled
    {
        get
        {
            return analyticsDisabled_;
        }
        set
        {
            analyticsDisabled_ = value;
#           if !UNITY_WEBPLAYER
            if (analyticsDisabled_)
                clearOfflineQueue();
#           endif
        }
    }

#if !UNITY_WEBPLAYER
    // Is offline cache enabled (not supported with web player)
    public bool useOfflineCache;
    // Full path and file name of the offline cache file.
    private string offlineCacheFilePath;
    // Reader for reading cached hits to be sent when online
    private StreamReader offlineCacheReader;
    // Writer for saving hits when offline
    private StreamWriter offlineCacheWriter;

    // cached values so that we don't have to read from PlayerPrefs all the time
    private int offlineQueueLength = -1; // -1 = uninitialized
    private int offlineQueueSentHits = -1;

    private enum NetAccessStatus
    {
        Offline, PendingVerification, Error, Mismatch, Functional
    }
    private NetAccessStatus netAccessStatus = NetAccessStatus.Offline;

    WaitForSeconds defaultReachabilityCheckPeriod = new WaitForSeconds(5.0f);
    WaitForSeconds hitBeingBuiltRetryTime = new WaitForSeconds(0.25f);
    WaitForSeconds netVerificationErrorRetryTime = new WaitForSeconds(20.0f);
    WaitForSeconds netVerificationMismatchRetryTime = new WaitForSeconds(10.0f);
    WaitForSeconds cachedHitSendThrottleTime = new WaitForSeconds(1.0f);

    //! Sets net activity time wait periods.
    /*! There are reasonable defaults, so there is no need to call this
     *  at all unless you want to change the times.
     */
    public void setOfflineQueueNetActivityTimes(float defaultReachabilityCheckPeriodSeconds,
                                                float hitBeingBuiltRetryTimeSeconds,
                                                float netVerificationErrorRetryTimeSeconds,
                                                float netVerificationMismatchRetryTimeSeconds,
                                                float cachedHitSendThrottleTimeSeconds)
    {
#       if DEBUG_WARNINGS
        if (defaultReachabilityCheckPeriodSeconds < 1 / 61.0f)
            Debug.LogWarning("GUA - custom defaultReachabilityCheckPeriodSeconds is set to a very low value: " + defaultReachabilityCheckPeriodSeconds);
        if (hitBeingBuiltRetryTimeSeconds < 1 / 61.0f)
            Debug.LogWarning("GUA - custom hitBeingBuiltRetryTimeSeconds is set to a very low value: " + defaultReachabilityCheckPeriodSeconds);
        if (netVerificationErrorRetryTimeSeconds < 1.0f)
            Debug.LogWarning("GUA - custom netVerificationErrorRetryTimeSeconds is set to a very low value: " + defaultReachabilityCheckPeriodSeconds);
        if (netVerificationMismatchRetryTimeSeconds < 0.5f)
            Debug.LogWarning("GUA - custom netVerificationMismatchRetryTimeSeconds is set to a very low value: " + defaultReachabilityCheckPeriodSeconds);
        if (cachedHitSendThrottleTimeSeconds < 0.5f)
            Debug.LogWarning("GUA - custom cachedHitSendThrottleTimeSeconds is set to a very low value: " + defaultReachabilityCheckPeriodSeconds);
#       endif
        defaultReachabilityCheckPeriod = new WaitForSeconds(defaultReachabilityCheckPeriodSeconds);
        hitBeingBuiltRetryTime = new WaitForSeconds(hitBeingBuiltRetryTimeSeconds);
        netVerificationErrorRetryTime = new WaitForSeconds(netVerificationErrorRetryTimeSeconds);
        netVerificationMismatchRetryTime = new WaitForSeconds(netVerificationMismatchRetryTimeSeconds);
        cachedHitSendThrottleTime = new WaitForSeconds(cachedHitSendThrottleTimeSeconds);
    }
#endif // not UNITY_WEBPLAYER

    public int remainingEntriesInOfflineCache
    {
        get
        {
#           if UNITY_WEBPLAYER
            return 0; // no offline cache support with webplayer
#           else
            if (offlineQueueLength == -1)
                offlineQueueLength = PlayerPrefs.GetInt(offlineQueueLengthPrefKey, 0);
            if (offlineQueueSentHits == -1)
                offlineQueueSentHits = PlayerPrefs.GetInt(offlineQueueSentHitsPrefKey, 0);
            return offlineQueueLength - offlineQueueSentHits;
#           endif // not webplayer
        }
    }


    //! Custom headers (e.g. for using custom-built User-Agent).
    #if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
    public Hashtable customHeaders = new Hashtable();
    #else // or Unity 4.5+:
    public Dictionary<string,string> customHeaders = new Dictionary<string,string>();
    #endif

#if UNITY_WEBPLAYER
    private bool addWebReferrerPending = false;
#endif

#if DEBUG_WARNINGS
    private const int sessionHitLimit = 500;
    private int sessionHitCount = 0;
    private bool sessionHitCountResetPending = false;
#endif


    //! Constructs a new analytics helper. You must use initialize() for actual initialization.
    public GoogleUniversalAnalytics()
    {
#       if DEBUG_LOGS
        Debug.Log("GUA constructor (GoogleUniversalAnalytics)");
#       endif
    }

    // Alternative for WWW.escapeURL(string), if choosing not to escape urls.
    private static string returnStringAsIs(string s)
    {
        //Debug.Log("skipped string escape: " + s);
        return s;
    }

#if DEBUG_WARNINGS
    // For debugging, returns string as is, but compares it to an escaped
    // version and logs warnings if the two don't match.
    private static string returnStringAsIs_withEscapingCheck(string s)
    {
        //Debug.Log("skipped string escape with check: " + s);
        string escaped = WWW.EscapeURL(s);
        if (!escaped.Equals(s))
            Debug.LogWarning("GUA string escaping check failed: \"" + s + "\" vs \"" + escaped + "\"");
        return s;
    }
#endif

    private bool isHitBeingBuilt
    {
        get
        {
            return hitType != HitType.None || sb.Length > 0;
        }
    }


    public bool internetReachable
    {
        get
        {
#           if !UNITY_WEBPLAYER
            if (useOfflineCache)
            {
                return netAccessStatus == NetAccessStatus.Functional &&
                    Application.internetReachability != NetworkReachability.NotReachable;
            }
            else
#           endif
                return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }


#if !UNITY_WEBPLAYER
    // Coroutine for net activity polling/updating.
    // (Needs to be started from a MonoBehaviour frontend class.)
    // Used only when offline cache is enabled, to check if we get online
    // and verify net access, or sending queued hits in the backround.
    IEnumerator netActivity()
    {
#       if DEBUG_WARNINGS
        int hitWasBeingBuiltCounter = 0;
#       endif
        NetworkReachability prevReachability = Application.internetReachability;

        if (Application.internetReachability != NetworkReachability.NotReachable)
            netAccessStatus = NetAccessStatus.PendingVerification;
        else
            netAccessStatus = NetAccessStatus.Offline;

#       if DEBUG_LOGS
        Debug.Log("GUA netActivity started, initial status: " + netAccessStatus);
#       endif

        while (useOfflineCache)
        {
#           if DEBUG_LOGS
            Debug.Log("GUA netActivity cycle, offlineQueueLength=" + offlineQueueLength + ", offlineQueueSentHits=" + offlineQueueSentHits);
#           endif
#           if DEBUG_WARNINGS
            if (hitWasBeingBuiltCounter >= 5)
            {
                // Trying to verify connection or send cached hits, but can't
                // do it since app code keeps being middle of building an
                // analytics hit. Show a warning.
                Debug.LogWarning("GUA can't verify connection or send cached hits because \"hit was being built\" over several tries. Did you beginHit() without using sendHit() almost immediately?");
                hitWasBeingBuiltCounter = 0;
            }
#           endif

            if (netAccessStatus == NetAccessStatus.Error)
            {
                yield return netVerificationErrorRetryTime;
                netAccessStatus = NetAccessStatus.PendingVerification;
            }
            else if (netAccessStatus == NetAccessStatus.Mismatch)
            {
                yield return netVerificationMismatchRetryTime;
                netAccessStatus = NetAccessStatus.PendingVerification;
            }

            NetworkReachability networkReachability = Application.internetReachability;
            if (prevReachability != networkReachability)
            {
#               if DEBUG_LOGS
                Debug.Log("GUA updated network reachability: " + networkReachability);
#               endif
                if (networkReachability != NetworkReachability.NotReachable)
                    netAccessStatus = NetAccessStatus.PendingVerification;
                else if (networkReachability == NetworkReachability.NotReachable)
                {
                    netAccessStatus = NetAccessStatus.Offline;
                }
                prevReachability = Application.internetReachability;
            }

            if (netAccessStatus == NetAccessStatus.PendingVerification)
            {
                if (isHitBeingBuilt)
                {
#                   if DEBUG_WARNINGS
                    ++hitWasBeingBuiltCounter;
#                   endif
#                   if DEBUG_LOGS
                    Debug.Log("GUA hit was being built when trying to verify net access, waiting");
#                   endif
                    yield return hitBeingBuiltRetryTime;
                    continue;
                }

#               if DEBUG_LOGS
                Debug.Log("GUA verifying network");
#               endif
#               if DEBUG_WARNINGS
                hitWasBeingBuiltCounter = 0;
#               endif
                beginHit(HitType.Screenview);
                addNonInteractionHit();
                addCommonOptionalHitParams();
                WWW www = finalizeAndSendHit(true);
                yield return www;
                if (www.error != null && www.error.Length > 0)
                {
#                   if DEBUG_LOGS
                    Debug.Log("GUA net verification error: " + www.error);
#                   endif
                    netAccessStatus = NetAccessStatus.Error;
                    continue;
                }
                else
                {
                    byte[] result = www.bytes;
                    if (result != null && result.Length > 3 &&
                        result[0] == 'G' && result[1] == 'I' && result[2] == 'F')
                    {
#                       if DEBUG_LOGS
                        Debug.Log("GUA net verification success");
#                       endif
                        netAccessStatus = NetAccessStatus.Functional; // success
                    }
                    else
                    {
#                       if DEBUG_LOGS
                        Debug.Log("GUA net verification mismatch (network login screen?)");
                        //Debug.Log(www.data); // for debug peeking at data
#                       endif
                        netAccessStatus = NetAccessStatus.Mismatch;
                        continue;
                    }
                } // no www error
            } // netAccessStatus == NetAccessStatus.PendingVerification

            if (pendingQueuedOfflineHits() &&
                netAccessStatus == NetAccessStatus.Functional)
            {
                if (isHitBeingBuilt)
                {
#                   if DEBUG_WARNINGS
                    ++hitWasBeingBuiltCounter;
#                   endif
                    yield return hitBeingBuiltRetryTime;
                    continue;
                }
                else
                {
#                   if DEBUG_WARNINGS
                    hitWasBeingBuiltCounter = 0;
#                   endif
                    sendOnePendingOfflineHit();
                    yield return cachedHitSendThrottleTime;
                    continue;
                }
            }

            yield return defaultReachabilityCheckPeriod;
        } // while useOfflineCache
    } // netActivity
#endif // not webplayer


    //! Initializes an analytics helper object for use. This must be called also for the default Instance.
    /*!
     * \param analyticsHelperBehaviour reference to the Analytics helper behaviour.
     * \param trackingID tracking ID for Google Analytics, like "UA-XXXXXXX-Y".
     * \param anonymousClientID string which identifies current user anonymously.
     *        For example, Unity's SystemInfo.deviceUniqueIdentifier.
     * \param appName App name (Optional, but required for using app tracking view).
     * \param appVersion App version (Optional, but required for using app tracking view).
     * \param useHTTPS Set to true to send analytics using https, or false for http.
     *                 Note that https requests may silently fail when running inside editor.
     * \param offlineCacheFilePath full path and file name to use for caching
     *                             hits to be sent later when offline, or empty
     *                             string ("") to disable offline cache.
     */
    public void initialize(MonoBehaviour analyticsHelperBehaviour,
                           string trackingID,
                           string anonymousClientID,
                           string appName = "",
                           string appVersion = "",
                           bool useHTTPS = false,
                           string offlineCacheFilePath = "")
    {
        this.helperBehaviour = analyticsHelperBehaviour;
        this.trackingID = escapeString(trackingID);

#       if UNITY_EDITOR
#       if DEBUG_WARNINGS
        if (this.trackingID.Equals("UA-8989161-8") &&
            !Application.loadedLevelName.Equals("AnalyticsExample"))
            Debug.LogError("GUA Warning! You are using Analytics Tracking ID of the AnalyticsExample. For own apps you must use your own Tracking ID from the Google Analytics site.");
#       endif // debug warnings
        if (this.trackingID.Equals("UA-XXXXXXX-Y"))
            Debug.LogError("GUA Error! You haven't set Analytics Tracking ID! You must get a Tracking ID from the Google Analytics site.");
#       endif // editor

        this.useHTTPS = useHTTPS;
        // the fields below are always escaped on initialization, just in case...
        clientID = WWW.EscapeURL(anonymousClientID);
        this.appName = (appName != null && appName.Length > 0) ? WWW.EscapeURL(appName) : null;
        this.appVersion = (appVersion != null && appVersion.Length > 0) ? WWW.EscapeURL(appVersion) : null;
        this.analyticsDisabled = false;

#       if DEBUG_WARNINGS
        if (this.appName != null && this.appName.Length > 100)
            Debug.LogWarning("GUA Application Name is too long, max 100 bytes: " + this.appName);
        if (this.appVersion != null && this.appVersion.Length > 100)
            Debug.LogWarning("GUA Application Version is too long, max 100 bytes: " + this.appName);
#       endif // debug warnings

#       if !UNITY_WEBPLAYER
        this.offlineCacheFilePath = offlineCacheFilePath;
        if (offlineCacheFilePath != null && offlineCacheFilePath.Length > 0)
        {
            useOfflineCache = true;
        }
#       endif // not webplayer

#       if DEBUG_LOGS
        // Log tracking ID, anonymous client ID and possible app name
        string loggedAppName = "(none)", loggedAppVersion = "(none)";
        if (appName != null && appName.Length > 0)
            loggedAppName = this.appName;
        if (appVersion != null && appVersion.Length > 0)
            loggedAppVersion = this.appVersion;
        Debug.Log("GUA trackingID:" + this.trackingID +
                  ", anonymousClientID:" + this.clientID +
                  ", appName:" + loggedAppName +
                  ", appVersion:" + loggedAppVersion);
        Debug.Log("GUA offline cache file full path: " + offlineCacheFilePath);
#       endif // debug logs

        // Construct cached string containing default part for ALL hits:
        // (on web player platforms we'll also add the start of enclosing
        //  javascript code which will be handed over to the browser)

        sb.Length = 0;

#       if UNITY_WEBPLAYER
        sb.Append("{var guax=null;guax=new XMLHttpRequest();guax.open(\"POST\",\"");
        sb.Append(useHTTPS ? httpsCollectUrl : httpCollectUrl);
        sb.Append("\",true);guax.setRequestHeader(\"Content-Type\",\"text/plain;charset=UTF-8\");guax.send(\"");
#       endif // webplayer

        SystemLanguage systemLanguage = Application.systemLanguage;
        userLanguage = systemLanguage.ToString();
#if UNITY_WINRT
        // WinRT .NET runtime is missing the GetCultures method so we use simple array
        string[] sysLangCodes = new string[40] {
            "af", "ar", "eu", "be", "bg", "ca", "zh-CHS", "cs", "da", "nl",
            "en", "et", "fo", "fi", "fr", "de", "el", "he", "hu", "is", "id",
            "it", "ja", "ko", "lv", "lt", "no", "pl", "pt", "ro", "ru", "hr",
            "sk", "sl", "es", "sv", "th", "tr", "uk", "vi"
        };
        if ((int)systemLanguage >= 0 && (int)systemLanguage < sysLangCodes.Length)
            userLanguage = sysLangCodes[(int)systemLanguage];
#else // other than WinRT platforms:
        System.Globalization.CultureInfo[] neutralCultures = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.NeutralCultures);
        for (int cidx = 0; cidx < neutralCultures.Length; ++cidx)
        {
            if (userLanguage.Contains(neutralCultures[cidx].EnglishName))
            {
                userLanguage = neutralCultures[cidx].Name;
                break;
            }
        }
#endif // other than WinRT

        sb.Append(guaVersionData);
        sb.Append("&tid="); // tracking ID, required for all hit types
        sb.Append(this.trackingID);
        sb.Append("&cid="); // client ID, required for all hit types
        sb.Append(this.clientID);
        // see also sendHit() and especially addCommonOptionalHitParams()
        defaultHitData = sb.ToString();
        sb.Length = 0;

        // Notes about the string buffer usage:
        // - Could reduce temp objects like this:
        //     sb.CopyTo(...)
        //     System.Text.Encoding.UTF8.GetBytes(char[], charIndex, charCount, byte[], byteIndex)
        //   However, WWW objects don't take byte[] with offset/count, so we can't do that.

#       if UNITY_IPHONE
        // Make custom User-Agent and use custom headers when running under iOS:
        string deviceModel = SystemInfo.deviceModel;
        string operatingSystem = SystemInfo.operatingSystem;
        string iDeviceType = "";
        string iOSVersion = "";
        if (deviceModel.StartsWith("iPhone"))
        {
            iDeviceType = "iPhone";
            iOSVersion = operatingSystem.Replace('.', '_');
        }
        else if (deviceModel.StartsWith("iPad"))
        {
            iDeviceType = "iPad";
            iOSVersion = operatingSystem.Substring(operatingSystem.IndexOf("OS")).Replace('.', '_');
        }
        else if (deviceModel.StartsWith("iPod"))
        {
            iDeviceType = "iPod";
            iOSVersion = operatingSystem.Replace('.', '_');
        }
        if (iDeviceType.Length > 0)
        {
            // detected we're running on iPhone/iPod/iPad, let's use a custom
            // User-Agent which should be detected correctly by Google Analytics
            sb.Append("Mozilla/5.0 (");
            sb.Append(iDeviceType);
            sb.Append("; CPU ");
            sb.Append(iOSVersion);
            sb.Append(" like Mac OS X)");
            // Enable following row if you for some reason want to feed
            // info which makes requests look even more like mobile browser,
            // although this info below might not be technically correct.
            //sb.Append(" AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10B329 Safari/8536.25");
            string userAgent = sb.ToString();
            sb.Length = 0;
            customHeaders.Add("User-Agent", userAgent);
#       if DEBUG_LOGS
            Debug.Log("GUA using iOS-custom-built User-Agent: " + userAgent);
#       endif // debug logs
        }
#       endif // unity iphone (iOS)

#       if UNITY_STANDALONE_OSX
        // Make custom User-Agent and use custom headers when running under OSX:
        sb.Append("Mozilla/5.0 (Macintosh; Intel ");
        sb.Append(SystemInfo.operatingSystem.Replace('.', '_'));
        sb.Append(")");
        // Enable following row if you for some reason want to feed
        // info which makes requests look like default OSX browser Safari,
        // although this info below might not be technically correct.
        //sb.Append(" AppleWebKit/537.13+ (KHTML, like Gecko) Version/5.1.7 Safari/534.57.2");
        string userAgent = sb.ToString();
        sb.Length = 0;
        customHeaders.Add("User-Agent", userAgent);
#       if DEBUG_LOGS
        Debug.Log("GUA using OSX-custom-built User-Agent: " + userAgent);
#       endif // debug logs
#       endif // unity standalone osx

#       if UNITY_STANDALONE_LINUX
        // Make custom User-Agent and use custom headers when running under Linux:
        string operatingSystem = SystemInfo.operatingSystem;
        string[] osTokens = operatingSystem.Split();
        sb.Append("Mozilla/5.0 (X11; ");
        if (osTokens.Length >= 3)
        {
            sb.Append(osTokens[2]);
            sb.Append("; ");
        }
        sb.Append("Linux ");
        if (operatingSystem.Contains("32bit"))
            sb.Append("i686)");
        else
            sb.Append("x86_64)");
        // Could append extra info here which would make the user agent look
        // like a specific browser, e.g. " Gecko/20100101 Firefox/31.0" (Firefox)
        // or " AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.9 Safari/536.5" (Chrome).
        string userAgent = sb.ToString();
        sb.Length = 0;
        customHeaders.Add("User-Agent", userAgent);
#       if DEBUG_LOGS
        Debug.Log("GUA using Linux-custom-built User-Agent: " + userAgent);
#       endif // debug logs
#       endif // unity standalone linux

#       if UNITY_STANDALONE_WIN
        // Make custom User-Agent and use custom headers when running under Windows desktop:
        string operatingSystem = SystemInfo.operatingSystem;
        string[] osTokens = operatingSystem.Split();
        sb.Append("Mozilla/5.0 (Windows NT ");
        bool addedNTVersion = false;
        for (int a = 0; a < osTokens.Length; ++a)
        {
            string token = osTokens[a];
            if (token.Length >= 5 && token[0] == '(' &&
                token[1] >= '0' && token[1] <= '9' &&
                token[2] == '.' &&
                token[3] >= '0' && token[3] <= '9')
            {
                sb.Append(token[1]);
                sb.Append('.');
                sb.Append(token[3]);
                addedNTVersion = true;
            }
        }
        if (!addedNTVersion && osTokens.Length >= 2)
        {
            string ntv = osTokens[1];
            if (ntv.Equals("9"))
                ntv = "6.4";
            else if (ntv.Equals("8.1") || ntv.Equals("2012"))
                ntv = "6.3";
            else if (ntv.Equals("8"))
                ntv = "6.2";
            else if (ntv.Equals("7") || ntv.Equals("2011") || ntv.Equals("2008"))
                ntv = "6.1";
            else if (ntv.Equals("Vista"))
                ntv = "6.0";
            else if (ntv.Equals("2003"))
                ntv = "5.2";
            else if (ntv.Equals("XP"))
                ntv = "5.1";
            else if (ntv.Equals("2000"))
                ntv = "5.0";
            sb.Append(ntv);
        }
        if (operatingSystem.Contains("64bit"))
            sb.Append("; WOW64");
        // Could append info here to make user agent look like Microsoft Internet Explorer
        // e.g. MSIE 11: "; Trident/7.0; rv:11.0) like Gecko" instead of the plain ')' below.
        sb.Append(')');
        // Could append extra info here which would make the user agent look
        // like a specific browser, e.g. " Gecko/20100101 Firefox/31.0" (Firefox)
        // or " AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36" (Chrome).
        string userAgent = sb.ToString();
        sb.Length = 0;
        customHeaders.Add("User-Agent", userAgent);
#       if DEBUG_LOGS
        Debug.Log("GUA using Windows-custom-built User-Agent: " + userAgent);
#       endif // debug logs
#       endif // unity standalone win

#       if !UNITY_WEBPLAYER
        if (useOfflineCache)
            helperBehaviour.StartCoroutine(netActivity());
#       endif // not webplayer
    }


    //! Controls automatic string escaping of text data added to hit parameters.
    /*!
     * All strings for hit parameters are escaped using WWW.EscapeURL, unless
     * you set useStringEscaping to false. Escaping of strings is safer, but
     * generates more temporary strings for garbage collecting.
     *  
     * Certain strings are still escaped even with useStringEscaping set to false.
     * Those special cases can be controlled on case-by-case basis with
     * allowNonEscaped parameter in the relevant add...() methods. If you choose
     * to do that, you should be extra careful not to supply invalid data!
     *
     * \param useStringEscaping When set to false, then some "just in case" string
     *        escapings are disabled. Editor build will still check those strings
     *        and log warnings with Debug.LogWarning if they should've been escaped.
     */
    public void setStringEscaping(bool useStringEscaping)
    {
        if (useStringEscaping)
            escapeString = WWW.EscapeURL;
        else
            escapeString = returnStringAsIs;
#       if DEBUG_WARNINGS
        // debug builds can warn about non-safe strings
        if (!useStringEscaping)
            escapeString = returnStringAsIs_withEscapingCheck;
#       endif // debug warnings
    }

    //! Begins describing a new "Hit" for analytics.
    /*!
     * Tracking ID and client ID are always added automatically for all hits,
     * as well as application name and version if those are specified when
     * initializing the analytics.
     * 
     * Additionally you can call set...() methods to specify app ID and
     * user ID, as well as user IP and user Agent overrides, which will also
     * be always added to all hits if they have been set.
     *
     * After calling beginHit(), you can use add...() methods to add specific
     * data to the hit being described. Finally after you have fully described
     * the hit, you must call sendHit() to actually send the hit.
     * 
     * \note It's best to build a hit and send it right away, i.e. call to
     *       sendHit() should follow beginHit almost immediately. This is so
     *       that net access verification and offline hit sending is not
     *       disrupted (when offline cache usage is enabled).
     *
     * \param hitType type of the hit we're describing, see #HitType.
     * \return true if successful, or false if internet is not reachable and
     *              any tries to continue with the hit description/sending will
     *              silently fail.
     *
     * \sa For more details, see the Measurement Protocol documentation:
     *     https://developers.google.com/analytics/devguides/collection/protocol/v1/
     */
    public bool beginHit(HitType hitType)
    {
#       if DEBUG_WARNINGS
        if (analyticsDisabled)
        {
            Debug.LogWarning("GUA beginHit called when analytics is disabled");
        }
        if (this.hitType != HitType.None)
        {
            Debug.LogWarning("GUA trying to use beginHit when a hit is already being built! (ignoring new hit type)");
            return false;
        }
#       endif // debug warnings

        string hitTypeName = hitTypeNames[(int)hitType];

        // constructing hits is now allowed even when being offline
        // (when queueOfflineHits is enabled)
        if (!internetReachable
#           if !UNITY_WEBPLAYER
            && !useOfflineCache
#           endif
            )
        {
#           if DEBUG_LOGS
            Debug.Log("GUA no internet reachability and no offline hit queueing - cannot beginHit: " + hitTypeName);
#           endif // debug logs
            return false;
        }

        this.hitType = hitType;
        sb.Length = 0;
        sb.Append(defaultHitData);
        sb.Append("&t=");
        sb.Append(hitTypeName);
#       if DEBUG_LOGS
        Debug.Log("GUA beginHit: " + hitTypeName);
#       endif // debug logs
        return true;
    }

    //! Cancels building of a hit.
    /*! Returns to neutral state without calling sendHit() after beginHit().
     */
    public void cancelHit()
    {
        sb.Length = 0;
        hitType = HitType.None;
    }

    //
    // General Measurement Protocol parameters
    //
    // (method for adding cache buster is omitted; it's automatically added)
    // 

    //! (Category:General) Sets flag which forces sender IP to be anonymized in analytics.
    public void setAnonymizeIP(bool enabled = true)
    {
        anonymizeIP = enabled;
    }

    //! (Category:General) Adds queue time in milliseconds for offline/latent hits (must be >= 0).
    /*! \note If ms is over 4 hours (14400000), hit might not be processed.
     */
    public void addQueueTime(int ms)
    {
#       if DEBUG_WARNINGS
        if (ms < 0)
            Debug.LogWarning("GUA Queue Time is invalid (negative): " + ms);
#       endif // debug warnings
        if (ms < 0)
            return;
        sb.Append("&qt=");
        sb.Append(ms);
    }


    //
    // User Measurement Protocol parameters
    //
    // (method for adding client ID is omitted; it's automatically added)
    //

    //! (Category:User) Sets a separate optional user ID for all upcoming hits.
    /*! The userID value should be a unique, persistent, and non-personally
     *  identifiable string identifier that represents a user or signed-in
     *  account across devices.
     * \note If you want to use the user ID, you must set it again each
     *       time your app is started. (By measurement protocol specs, the
     *       user ID is not allowed to be automatically persisted to storage).
     * \param userID user ID for upcoming hits, for example "as8eknlll".
     *               This may not itself be PII (personally identifiable
     *               information). Set to empty string ("") if you want to
     *               remove previously set userID.
     */
    public void setUserID(string userID)
    {
        if (userID == null || userID.Length == 0)
            this.userID = "";
        else
            this.userID = escapeString(userID);
    }


    //
    // Session Measurement Protocol parameters
    //

    //! (Category:Session) Adds forced session control (session start or end).
    /*! \param type When true, a new session start is forced with the hit.
     *              When false, the hit will force end to current session.
     */
    public void addSessionControl(bool type)
    {
        sb.Append(type ? "&sc=start" : "&sc=end");
#       if DEBUG_WARNINGS
        if (type)
            sessionHitCount = 0; // new session - current hit will be 1st one
        else
            sessionHitCountResetPending = true; // end session - pending reset
#       endif // debug warnings
    }

    //! (Category:Session) Sets IP address of the user for all upcoming hits (overrides implicit one).
    /*! This should be a valid IP address. It will always be anonymized just
     *  as though addAnonymizeIP had been used.
     * \param ip user IP address, for example "10.0.0.123". Set to empty
     *           string ("") if you want to remove previously set value.
     */
    public void setIPOverride(string ip)
    {
        if (ip == null || ip.Length == 0)
            this.userIP = "";
        else
        {
            string data = escapeString(ip);
            this.userIP = data;
        }
    }

    //! (Category:Session) Sets user agent of the "browser" for all upcoming hits (overrides implicit one).
    /*! Google Analytics site determines OS and device type from user agent.
     * \note Google has libraries to identify real user agents. Hand crafting
     *       your own agent could break at any time. Debugging any problems
     *       with this feature is up to you.
     * \param userAgent user agent of the "browser". Search the internet for
     *                  various examples of user agent strings. Set to empty
     *                  string ("") if you want to remove previously set value.
     * \param allowNonEscaped userAgent is always escaped unless this is set to true.
     */
    public void setUserAgentOverride(string userAgent, bool allowNonEscaped = false)
    {
        if (userAgent == null || userAgent.Length == 0)
            this.userAgent = "";
        else
        {
            string data = allowNonEscaped ? escapeString(userAgent) : WWW.EscapeURL(userAgent);
            this.userAgent = data;
        }
    }


    //
    // Traffic Sources Measurement Protocol parameters
    //

    //! (Category:Traffic Sources) Adds referral info url.
    /*!
     * \param url document referrer url. For Web player builds "null" is a
     *            special value you can use, in which case the "document.referrer"
     *            value from the web will be used instead.
     * \param allowNonEscaped url is always escaped unless this is set to true.
     */
    public void addDocumentReferrer(string url, bool allowNonEscaped = false)
    {
#       if UNITY_WEBPLAYER
        if (url == null)
        {
            addWebReferrerPending = true;
            return;
        }
#       endif // not webplayer

        string data = allowNonEscaped ? escapeString(url) : WWW.EscapeURL(url);
        sb.Append("&dr=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 2048)
            Debug.LogWarning("GUA Document Referrer is too long, max 2048 bytes");
#       endif // debug warnings
    }

    //! (Category:Traffic Sources) Adds campaign name.
    /*! \param text name of the campaign (max 100 bytes), e.g. "direct".
     */
    public void addCampaignName(string text)
    {
        string data = escapeString(text);
        sb.Append("&cn=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 100)
            Debug.LogWarning("GUA Campaign Name is too long, max 100 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Traffic Sources) Adds campaign source.
    /*! \param text source of the campaign (max 100 bytes), e.g. "direct".
     */
    public void addCampaignSource(string text)
    {
        string data = escapeString(text);
        sb.Append("&cs=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 100)
            Debug.LogWarning("GUA Campaign Source is too long, max 100 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Traffic Sources) Adds campaign medium.
    /*! \param text campaign medium (max 50 bytes), e.g. "organic".
     */
    public void addCampaignMedium(string text)
    {
        string data = escapeString(text);
        sb.Append("&cm=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 50)
            Debug.LogWarning("GUA Campaign Medium is too long, max 50 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Traffic Sources) Adds campaign keyword.
    /*!
     * \param text campaign keyword (max 500 bytes), e.g. "Blue Shoes".
     * \param allowNonEscaped text is always escaped, unless this is set to true.
     *        In that case, the example above should be given in form "Blue%20Shoes".
     */
    public void addCampaignKeyword(string text, bool allowNonEscaped = false)
    {
        string data = allowNonEscaped ? escapeString(text) : WWW.EscapeURL(text);
        sb.Append("&ck=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Campaign Keyword is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Traffic Sources) Adds campaign content.
    /*!
     * \param text campaign content (max 100 bytes).
     * \param allowNonEscaped text is always escaped, unless this is set to true.
     */
    public void addCampaignContent(string text, bool allowNonEscaped = false)
    {
        string data = allowNonEscaped ? escapeString(text) : WWW.EscapeURL(text);
        sb.Append("&cc=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 100)
            Debug.LogWarning("GUA Campaign Content is too long, max 100 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Traffic Sources) Adds campaign ID.
    /*! \param text campaign ID (max 100 bytes).
     */
    public void addCampaignID(string text)
    {
        string data = escapeString(text);
        sb.Append("&ci=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 100)
            Debug.LogWarning("GUA Campaign ID is too long, max 100 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Traffic Sources) Adds Google AdWords ID.
    /*! (Included for API completeness.)
     * \param text Google AdWords ID, e.g. "CL6Q-OXyqKUCFcgK2goddQuoHg".
     */
    public void addGoogleAdWordsID(string text)
    {
        string data = escapeString(text);
        sb.Append("&gclid=");
        sb.Append(data);
    }

    //! (Category:Traffic Sources) Adds Google Display Ads ID.
    /*! (Included for API completeness.)
     * \param text Google Display Ads ID.
     */
    public void addGoogleDisplayAdsID(string text)
    {
        string data = escapeString(text);
        sb.Append("&dclid=");
        sb.Append(data);
    }


    //
    // System Info Measurement Protocol parameters
    //

    //! (Category:System Info) Adds screen resolution.
    /*! \note You can use Unity's Screen.currentResolution width and height.
     * \param width width of the whole screen in pixels.
     * \param height height of the whole screen in pixels.
     */
    public void addScreenResolution(int width, int height)
    {
        // note: max 20 bytes
        sb.Append("&sr=");
        sb.Append(width);
        sb.Append('x');
        sb.Append(height);
#       if DEBUG_WARNINGS
        if (width < 0 || height < 0)
            Debug.LogWarning("GUA Screen Resolution is invalid: " + width + "x" + height);
#       endif // debug warnings
    }

    //! (Category:System Info) Adds viewable area of device or browser.
    /*! \note You can use Unity's Screen.width and height.
     * \param width width of the window or usable screen area.
     * \param height height of the window or usable screen area.
     */
    public void addViewportSize(int width, int height)
    {
        // note: max 20 bytes
        sb.Append("&vp=");
        sb.Append(width);
        sb.Append('x');
        sb.Append(height);
#       if DEBUG_WARNINGS
        if (width < 0 || height < 0)
            Debug.LogWarning("GUA Viewport Size is invalid: " + width + "x" + height);
#       endif // debug warnings
    }

    //! (Category:System Info) Adds definition of character set used for page/document.
    /*! \param text document encoding (max 20 bytes), for example "UTF-8".
     */
    public void addDocumentEncoding(string text)
    {
        string data = escapeString(text);
        sb.Append("&de=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 20)
            Debug.LogWarning("GUA Document Encoding is too long, max 20 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:System Info) Adds screen color depth in bits, for example 24.
    /*! \note You can check Unity's Handheld.use32BitDisplayBuffer to check if display is 32 or 16 bits.
     * \param depthBits screen color depth in bits.
     */
    public void addScreenColors(int depthBits)
    {
        // note: max 20 bytes
        sb.Append("&sd=");
        sb.Append(depthBits);
        sb.Append("-bits");
    }

    //! (Category:System Info) Sets user language code.
    /*! This is auto-initialized to current language and added to all hits.
     * \param text user language code (max 20 bytes), for example "en-us".
     * \note You can use Unity's Application.systemLanguage.ToString() to get
     *       current language as a string, but it gives language in plain text
     *       word like "English" and not like "en-us". The auto-initialized
     *       setting is converted from the English name to a language code.
     */
    public void setUserLanguage(string text)
    {
        if (text == null || text.Length == 0)
            userLanguage = "";
        else
        {
            string data = escapeString(text);
            userLanguage = data;
#           if DEBUG_WARNINGS
            if (data.Length > 20)
                Debug.LogWarning("GUA User Language is too long, max 20 bytes: " + data);
#           endif // debug warnings
        }
    }

    //! (Category:System Info) Adds info about Java availability.
    /*! (Included for API completeness.)
     * \param enabled true or false to determine if Java is enabled or not.
     */
    public void addJavaEnabled(bool enabled)
    {
        sb.Append(enabled ? "&je=1" : "&je=0");
    }

    //! (Category:System Info) Adds info about Flash version.
    /*! (Included for API completeness.)
     * \param major major version of Flash, e.g. 10.
     * \param minor minor version of Flash, e.g. 1.
     * \param revision revision of Flash, e.g. 103.
     */
    public void addFlashVersion(int major, int minor, int revision)
    {
        // note: max 20 bytes
        sb.Append("&fl=");
        sb.Append(major);
        sb.Append("%20");
        sb.Append(minor);
        sb.Append("%20r");
        sb.Append(revision);
    }


    //
    // Hit Measurement Protocol parameters
    //

    //! (Category:Hit) Adds specification that hit should be considered non-interactive.
    public void addNonInteractionHit()
    {
        sb.Append("&ni=1");
    }


    //
    // Content Information Measurement Protocol parameters
    //

    //! (Category:Content Information) Adds document location URL.
    /*! Used to set full URL of current page. Setting document host and path can be
     *  additionally used for overriding. Any private information should be removed
     *  from the data (e.g. user authentication).
     * \param url full URL of current page (max 2048 bytes).
     * \param allowNonEscaped url is always escaped, unless this is set to true.
     */
    public void addDocumentLocationURL(string url, bool allowNonEscaped = false)
    {
        string data = allowNonEscaped ? escapeString(url) : WWW.EscapeURL(url);
        sb.Append("&dl=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 2048)
            Debug.LogWarning("GUA Document Location URL is too long, max 2048 bytes");
#       endif // debug warnings
    }

    //! (Category:Content Information) Adds hostname where content was hosted.
    /*! \param text hostname (max 100 bytes), e.g. "unity3d.com".
     */
    public void addDocumentHostName(string text)
    {
        string data = escapeString(text);
        sb.Append("&dh=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 100)
            Debug.LogWarning("GUA Document Host Name is too long, max 100 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Content Information) Adds path portion of the page URL, should begin with '/'.
    /*!
     * \param text path to use, should begin with '/' (max 2048 bytes).
     * \param allowNonEscaped text is always escaped unless this is set to true.
     *        If you choose to handle escaping by yourself, '/' equals "%2F".
     */
    public void addDocumentPath(string text, bool allowNonEscaped = false)
    {
        string data = allowNonEscaped ? escapeString(text) : WWW.EscapeURL(text);
        sb.Append("&dp=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 2048)
            Debug.LogWarning("GUA Document Path is too long, max 2048 bytes");
#       endif // debug warnings
    }

    //! (Category:Content Information) Adds title of the page or document.
    /*! \param text title of the page or document (max 1500 bytes).
     */
    public void addDocumentTitle(string text)
    {
        string data = escapeString(text);
        sb.Append("&dt=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 1500)
            Debug.LogWarning("GUA Document Title is too long, max 1500 bytes");
#       endif // debug warnings
    }

    //! (Category:Content Information) Adds screen name.
    /*! If this is missing for normal page view tracking, this is taken from
     *  document location or host name & path. The sendAppScreenHit convenience
     *  method uses this to specify name of the app screen.
     * \param text screen name.
     */
    public void addScreenName(string text)
    {
#       if DEBUG_WARNINGS
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Screen Name (Content Description)");
#       endif // debug warnings
        string data = escapeString(text);
        sb.Append("&cd=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 2048)
            Debug.LogWarning("GUA Screen Name is too long, max 2048 bytes");
#       endif // debug warnings
    }

    //! (Category:Content Information) Adds ID of a clicked (DOM) element.
    /*! Used to disambiguate multiple links to the same URL in In-Page Analytics
     *  reports when Enhanced Link Attribution is enabled for the property.
     * \param text screen name (with app tracking), or description of content.
     */
    public void addLinkID(string text)
    {
#       if DEBUG_WARNINGS
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Link ID");
#       endif // debug warnings
        string data = escapeString(text);
        sb.Append("&linkid=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        // Note: Max length is not actually specified in measurement protocol
        //       parameter specs (as of now), so this is self-imposed:
        if (data.Length > 100)
            Debug.LogWarning("GUA Link ID is too long, max 100 bytes");
#       endif // debug warnings
    }



    //
    // App Tracking Measurement Protocol parameters
    //

    //! (Category:App Tracking) Sets application name for all upcoming hits (max 100 bytes).
    /*! Required for app views in your Google Analytics profiles.
     * \param text app name, for example "Herring". Set to empty string ("")
     *             to remove previously set value.
     */
    public void setApplicationName(string text)
    {
        if (text == null || text.Length == 0)
            appName = "";
        else
        {
            string data = escapeString(text);
            appName = data;
#           if DEBUG_WARNINGS
            if (data.Length > 100)
                Debug.LogWarning("GUA Application Name is too long, max 100 bytes: " + data);
#           endif // debug warnings
        }
    }

    //! (Category:App Tracking) Sets application ID for all upcoming hits (max 150 bytes).
    /*! \param text application ID (max 150 bytes), for example
     *              "com.company.app". Set to empty string ("") to remove
     *              previously set value.
     */
    public void setApplicationID(string text)
    {
        if (text == null || text.Length == 0)
            appID = "";
        else
        {
            string data = escapeString(text);
            appID = data;
#           if DEBUG_WARNINGS
            if (data.Length > 150)
                Debug.LogWarning("GUA Application ID is too long, max 150 bytes: " + data);
#           endif // debug warnings
        }
    }

    //! (Category:App Tracking) Sets application version for all upcoming hits (max 100 bytes).
    /*! Required for app views in your Google Analytics profiles.
     * \param text application version (max 100 bytes), for example "1.2".
     *        Set to empty string ("") to remove previously set value.
     */
    public void setApplicationVersion(string text)
    {
        if (text == null || text.Length == 0)
            appVersion = "";
        else
        {
            string data = escapeString(text);
            appVersion = data;
#           if DEBUG_WARNINGS
            if (data.Length > 100)
                Debug.LogWarning("GUA Application Version is too long, max 100 bytes: " + data);
#           endif // debug warnings
        }
    }

    //! (Category:App Tracking) Adds application installer ID (max 150 bytes).
    /*! \note Unlike other setApplication...-named methods, this only affects
     *        the hit currently being built.
     * \param text application installer ID (max 150 bytes), for example
     *             "com.platform.vending".
     */
    public void addApplicationInstallerID(string text)
    {
#       if DEBUG_WARNINGS
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Application Installer ID");
#       endif // debug warnings
        string data = escapeString(text);
        sb.Append("&aiid=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 150)
            Debug.LogWarning("GUA Application Installer ID is too long, max 150 bytes: " + data);
#       endif // debug warnings
    }


    //
    // Event Tracking Measurement Protocol parameters
    //

    //! (Category:Event Tracking) Adds event category.
    /*! \note Only for hits with Event type.
     * \param text category of event (max 150 bytes), must not be empty.
     */
    public void addEventCategory(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Event)
            Debug.LogWarning("GUA trying to add Event Category to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Event Category");
#       endif // debug warnings
        if (hitType != HitType.Event)
            return;
        string data = escapeString(text);
        sb.Append("&ec=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length == 0)
            Debug.LogWarning("GUA Event Category is empty!");
        if (data.Length > 150)
            Debug.LogWarning("GUA Event Category is too long, max 150 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Event Tracking) Adds event action.
    /*! \note Only for hits with Event type.
     * \param text event action (max 500 bytes), must not be empty.
     */
    public void addEventAction(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Event)
            Debug.LogWarning("GUA trying to add Event Action to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Event Category");
#       endif // debug warnings
        if (hitType != HitType.Event)
            return;
        string data = escapeString(text);
        sb.Append("&ea=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Event Action is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Event Tracking) Adds event label.
    /*! \note Only for hits with Event type.
     * \param text label for the event (max 500 bytes).
     */
    public void addEventLabel(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Event)
            Debug.LogWarning("GUA trying to add Event Label to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Event Label");
#       endif // debug warnings
        if (hitType != HitType.Event)
            return;
        string data = escapeString(text);
        sb.Append("&el=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Event Label is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Event Tracking) Adds event value (must be >= 0).
    /*! \note Only for hits with Event type.
     * \param value event value, must be non-negative.
     */
    public void addEventValue(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Event)
            Debug.LogWarning("GUA trying to add Event Value to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (value < 0)
            Debug.LogWarning("GUA Event Value is invalid (negative): " + value);
#       endif // debug warnings
        if (hitType != HitType.Event || value < 0)
            return;
        sb.Append("&ev=");
        sb.Append(value);
    }


    //
    // E-Commerce Measurement Protocol parameters
    //

    //! (Category:E-Commerce & Enhanced E-Commerce) Adds unique transaction identifier. *Required* for Transaction and Item hit types!
    /*! \note When using with E-Commerce category methods, this should be
     *        identical for both Transaction and Item hits associated to a
     *        particular transaction, and is also **required** for both types.
     * \note When using with Enhanced E-Commerce methods, this can be used as
     *       an additional parameter when ProductAction is Purchase or Refund.
     * \param text unique identifier for the transaction (max 500 bytes), for example "13377AA7".
     */
    public void addTransactionID(string text)
    {
        // Extra hit type verifications are now omitted as this can now be also
        // optionally used with the newer Enhanced E-Commerce methods...
#       if DEBUG_WARNINGS
        //if (hitType != HitType.Transaction && hitType != HitType.Item)
        //    Debug.LogWarning("GUA trying to add Transaction ID to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Transaction ID");
#       endif // debug warnings
        //if (hitType != HitType.Transaction && hitType != HitType.Item)
        //    return;
        string data = escapeString(text);
        sb.Append("&ti=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        // MP spec doesn't exactly limit to 500 bytes in context of Enhanced E-Commerce
        if (data.Length > 500)
            Debug.LogWarning("GUA Transaction ID is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:E-Commerce & Enhanced E-Commerce) Adds store or affiliation from which transaction occurred.
    /*! \note When using with E-Commerce category methods, this is only for hits with Transaction type.
     * \note When using with Enhanced E-Commerce methods, this can be used as
     *       an additional parameter when ProductAction is Purchase or Refund.
     * \param text affiliation or store name (max 500 bytes), for example "PlayShop".
     */
    public void addTransactionAffiliation(string text)
    {
        // Extra hit type verifications are now omitted as this can now be also
        // optionally used with the newer Enhanced E-Commerce methods...
//#       if DEBUG_WARNINGS
        //if (hitType != HitType.Transaction)
        //    Debug.LogWarning("GUA trying to add Transaction Affiliation to wrong hit type: " + hitTypeNames[(int)hitType]);
//#       endif // debug warnings
        //if (hitType != HitType.Transaction)
        //    return;
        string data = escapeString(text);
        sb.Append("&ta=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        // MP spec doesn't exactly limit to 500 bytes in context of Enhanced E-Commerce
        if (data.Length > 500)
            Debug.LogWarning("GUA Transaction Affiliation is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:E-Commerce & Enhanced E-Commerce) Adds total revenue for transaction, including tax and shipping costs.
    /*! \note When using with E-Commerce category methods, this is only for hits with Transaction type.
     * \note When using with Enhanced E-Commerce methods, this can be used as
     *       an additional parameter when ProductAction is Purchase or Refund.
     *       Also in that context, if this is not sent, the value will be
     *       automatically calculated using the product quantity and price
     *       fields of all products in the same hit.
     * \param currency total revenue including shipping or tax costs, for example 15.47.
     *                 Precision is up to 6 decimal places.
     */
    public void addTransactionRevenue(double currency)
    {
        // Extra hit type verifications are now omitted as this can now be also
        // optionally used with the newer Enhanced E-Commerce methods...
//#       if DEBUG_WARNINGS
        //if (hitType != HitType.Transaction)
        //    Debug.LogWarning("GUA trying to add Transaction Revenue to wrong hit type: " + hitTypeNames[(int)hitType]);
//#       endif // debug warnings
        //if (hitType != HitType.Transaction)
        //    return;
        sb.Append("&tr=");
        sb.AppendFormat("{0:F6}", currency);
    }

    //! (Category:E-Commerce & Enhanced E-Commerce) Adds total shipping cost associated with the transaction.
    /*!  \note When using with E-Commerce category methods, this is only for hits with Transaction type.
     * \note When using with Enhanced E-Commerce methods, this can be used as
     *       an additional parameter when ProductAction is Purchase or Refund.
     * \param currency total shipping cost of transaction, for example 3.50.
     *                 Precision is up to 6 decimal places.
     */
    public void addTransactionShipping(double currency)
    {
        // Extra hit type verifications are now omitted as this can now be also
        // optionally used with the newer Enhanced E-Commerce methods...
//#       if DEBUG_WARNINGS
        //if (hitType != HitType.Transaction)
        //    Debug.LogWarning("GUA trying to add Transaction Shipping to wrong hit type: " + hitTypeNames[(int)hitType]);
//#       endif // debug warnings
        //if (hitType != HitType.Transaction)
        //    return;
        sb.Append("&ts=");
        sb.AppendFormat("{0:F6}", currency);
    }

    //! (Category:E-Commerce & Enhanced E-Commerce) Adds total tax associated with the transaction.
    /*!  \note When using with E-Commerce category methods, this is only for hits with Transaction type.
     * \note When using with Enhanced E-Commerce methods, this can be used as
     *       an additional parameter when ProductAction is Purchase or Refund.
     * \param currency total tax of transaction, for example 3.71.
     *                 Precision is up to 6 decimal places.
     */
    public void addTransactionTax(double currency)
    {
        // Extra hit type verifications are now omitted as this can now be also
        // optionally used with the newer Enhanced E-Commerce methods...
//#       if DEBUG_WARNINGS
        //if (hitType != HitType.Transaction)
        //    Debug.LogWarning("GUA trying to add Transaction Tax to wrong hit type: " + hitTypeNames[(int)hitType]);
//#       endif // debug warnings
        //if (hitType != HitType.Transaction)
        //    return;
        sb.Append("&tt=");
        sb.AppendFormat("{0:F6}", currency);
    }

    //! (Category:E-Commerce) Adds item name. *Required* for Item hit type!
    /*! \note Only for hits with Item type (**required**).
     * \param text item name (max 500 bytes), for example "Sausage".
     */
    public void addItemName(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Item)
            Debug.LogWarning("GUA trying to add Item Name to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Item Name");
#       endif // debug warnings
        if (hitType != HitType.Item)
            return;
        string data = escapeString(text);
        sb.Append("&in=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Item Name is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:E-Commerce) Adds price for a single item or unit.
    /*! \note Only for hits with Item type.
     * \param currency price for a single item or unit, for example 3.50.
     *                 Precision is up to 6 decimal places.
     */
    public void addItemPrice(double currency)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Item)
            Debug.LogWarning("GUA trying to add Item Price to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Item)
            return;
        sb.Append("&ip=");
        sb.AppendFormat("{0:F6}", currency);
    }

    //! (Category:E-Commerce) Adds number of items purchased.
    /*! \param Only for hits with Item type.
     * \param value number of items purchased, for example 4.
     */
    public void addItemQuantity(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Item)
            Debug.LogWarning("GUA trying to add Item Quantity to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Item)
            return;
        sb.Append("&iq=");
        sb.Append(value);
    }

    //! (Category:E-Commerce) Adds SKU or item code.
    /*! \note Only for hits with Item type.
     * \param text SKU or item code (max 500 bytes), for example "SKU7447".
     */
    public void addItemCode(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Item)
            Debug.LogWarning("GUA trying to add Item Code to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Item)
            return;
        string data = escapeString(text);
        sb.Append("&ic=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Item Code is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:E-Commerce) Adds category that item belongs to.
    /*! \note Only for hits with Item type.
     * \param text item variation/category (max 500 bytes), for example "Blue".
     */
    public void addItemCategory(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Item)
            Debug.LogWarning("GUA trying to add Item Category to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Item)
            return;
        string data = escapeString(text);
        sb.Append("&iv=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Item Category is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:E-Commerce) Adds currency type for all transaction currency values.
    /*! Should be a valid ISO 4217 currency code.
     * \sa http://en.wikipedia.org/wiki/ISO_4217#Active_codes
     * \note Only for hits with Transaction or Item type.
     * \param text currency type as a valid ISO 4217 code (max 10 bytes), for example "EUR".
     */
    public void addCurrencyCode(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Transaction && hitType != HitType.Item)
            Debug.LogWarning("GUA trying to add Currency Code to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Transaction && hitType != HitType.Item)
            return;
        string data = escapeString(text);
        sb.Append("&cu=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 10)
            Debug.LogWarning("GUA Currency Code is too long, max 10 bytes: " + data);
#       endif // debug warnings
    }


    //
    // Enhanced E-Commerce Measurement Protocol parameters
    //

    private int clampEnhancedECommerceIndex(int index)
    {
        int clampedIndex = Mathf.Clamp(index, 1, 200);
#       if DEBUG_WARNINGS
        if (index != clampedIndex)
            Debug.LogWarning("GUA Enhanced E-Commerce index must be between 1 and 200, inclusive (will clamp value " + index + " to " + clampedIndex + ")");
#       endif // debug warnings
        return clampedIndex;
    }

    //! (Category:Enhanced E-Commerce) Adds role of the products included in a hit. All product definitions in the hit will be ignored if this is not specified!
    /*! \note If a product action is not specified, all product definitions included with the hit will be ignored!
     * \param action role of the products included in a hit, one from ProductAction enum.
     */
    public void addProductAction(ProductAction action)
    {
        string actionName = productActionNames[(int)action];
        sb.Append("&pa=");
        sb.Append(actionName);
    }

    //! (Category:Enhanced E-Commerce) Adds the SKU of the product.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text SKU of the product, for example "SKU7447".
     * \note This is part of "Enhanced E-Commerce" - see also addItemCode.
     */
    public void addProductSKU(int productIndex, string text)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("id=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Enhanced E-Commerce product SKU is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Enhanced E-Commerce) Adds name of the product.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text name of the product, for example "Sausage".
     * \note This is part of "Enhanced E-Commerce" - see also addItemName.
     */
    public void addProductName(int productIndex, string text)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("nm=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Enhanced E-Commerce product name is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Enhanced E-Commerce) Adds the brand associated with the product.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text the brand associated with the product, e.g. your company name.
     */
    public void addProductBrand(int productIndex, string text)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("br=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Enhanced E-Commerce product brand is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Enhanced E-Commerce) Adds category to which the product belongs.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text category of the product, for example "LevelPacks".
     * \note This is part of "Enhanced E-Commerce" - see also addItemCategory.
     */
    public void addProductCategory(int productIndex, string text)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("ca=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Enhanced E-Commerce product category is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Enhanced E-Commerce) Adds variant of the product.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text variant of the product, for example "Blue".
     * \note This is part of "Enhanced E-Commerce" - see also addItemCategory.
     */
    public void addProductVariant(int productIndex, string text)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("va=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Enhanced E-Commerce product variant is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Enhanced E-Commerce) Adds price of the product.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param currency the price of the product, for example 3.50.
     *                 Precision is up to 6 decimal places.
     * \note This is part of "Enhanced E-Commerce" - see also addItemPrice.
     */
    public void addProductPrice(int productIndex, double currency)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("pr=");
        sb.AppendFormat("{0:F6}", currency);
    }

    //! (Category:Enhanced E-Commerce) Adds quantity of a product.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param value the quantity of a product, for example 2.
     * \note This is part of "Enhanced E-Commerce" - see also addItemQuantity.
     */
    public void addProductQuantity(int productIndex, int value)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("qt=");
        sb.Append(value);
    }

    //! (Category:Enhanced E-Commerce) Adds coupon code associated with a product.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text the associated coupon code, for example "SUMMER15".
     */
    public void addProductCouponCode(int productIndex, string text)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("cc=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA Enhanced E-Commerce product coupon code is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Enhanced E-Commerce) Adds product's position in a list or collection.
    /*! \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param value product's position in a list or collection, for example 3.
     */
    public void addProductPosition(int productIndex, int value)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("ps=");
        sb.Append(value);
    }

    //! (Category:Enhanced E-Commerce) Adds a product-level custom dimension.
    /*! \note Normal Google Analytics accounts have max 20 custom dimensions (200 for Premium). It's not clear if that limit applies to product-level custom dimensions.
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param dimensionIndex index of the custom dimension (must be between 1 and 200, inclusive).
     * \param text custom dimension text, for example "Member".
     * \note This is part of "Enhanced E-Commerce" - see also addCustomDimension.
     */
    public void addProductCustomDimension(int productIndex, int dimensionIndex, string text)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        dimensionIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("cd");
        sb.Append(dimensionIndex);
        sb.Append('=');
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds a product-level custom metric.
    /*! \note Normal Google Analytics accounts have max 20 custom dimensions (200 for Premium). It's not clear if that limit applies to product-level custom metrics.
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param dimensionIndex index of the custom dimension (must be between 1 and 200, inclusive).
     * \param value custom dimension value as 64-bit signed integer, for example 7447.
     * \note This is part of "Enhanced E-Commerce" - see also addCustomMetric.
     */
    public void addProductCustomMetric(int productIndex, int dimensionIndex, long value)
    {
        productIndex = clampEnhancedECommerceIndex(productIndex);
        dimensionIndex = clampEnhancedECommerceIndex(productIndex);
        sb.Append("&pr");
        sb.Append(productIndex);
        sb.Append("cm");
        sb.Append(dimensionIndex);
        sb.Append('=');
        sb.Append(value);
    }

    //! (Category:Enhanced E-Commerce) Adds coupon code redeemed with the transaction.
    /*! \note This additional parameter can be sent when ProductAction
     *        is Purchase or Refund.
     * \param text the redeemed coupon code, for example "SUMMER15".
     */
    public void addTransactionCouponCode(string text)
    {
        string data = escapeString(text);
        sb.Append("&tcc=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds the list or collection from which a product action occurred.
    /*! \note This additional parameter can be sent when ProductAction
     *        is Detail or Click.
     * \param text the list or collection from which a product action occurred,
     *             for example "SearchResults".
     */
    public void addProductActionList(string text)
    {
        string data = escapeString(text);
        sb.Append("&pal=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds the step number in a checkout funnel.
    /*! \note This additional parameter can be sent when ProductAction is Checkout.
     * \param value step number in a checkout funnel, for example 2.
     */
    public void addCheckoutStep(int value)
    {
        sb.Append("&cos=");
        sb.Append(value);
    }

    //! (Category:Enhanced E-Commerce) Adds additional information about a checkout step.
    /*! \note This additional parameter can be sent when ProductAction is Checkout.
     * \param text additional information about a checkout step, e.g. "Visa".
     */
    public void addCheckoutStepOption(string text)
    {
        string data = escapeString(text);
        sb.Append("&col=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds product impression list name.
    /*! \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param text the list or collection to which a product belongs,
     *             for example "SearchResults".
     */
    public void addProductImpressionListName(int listIndex, string text)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        string data = escapeString(text);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("nm=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds product impression ID or SKU.
    /*! \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text the product ID or SKU, for example "P7447".
     */
    public void addProductImpressionSKU(int listIndex, int productIndex, string text)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("id=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds product impression name.
    /*! \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text name of the product, for example "Sausage".
     */
    public void addProductImpressionName(int listIndex, int productIndex, string text)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("nm=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds product impression brand.
    /*! \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text the brand associated with the product, e.g. your company name.
     */
    public void addProductImpressionBrand(int listIndex, int productIndex, string text)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("br=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds product impression category.
    /*! \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text the category to which the product belongs, for example "LevelPacks".
     */
    public void addProductImpressionCategory(int listIndex, int productIndex, string text)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("ca=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds product impression variant.
    /*! \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param text the variant of the product, for example "Blue".
     */
    public void addProductImpressionVariant(int listIndex, int productIndex, string text)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("va="); // needs to be verified - MP spec has a typo on this (equal to P.I.Category)
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds product impression position in a list or collection.
    /*! \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param value product's position in a list or collection, for example 3.
     */
    public void addProductImpressionPosition(int listIndex, int productIndex, int value)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("ps=");
        sb.Append(value);
    }

    //! (Category:Enhanced E-Commerce) Adds impression price of a product.
    /*! \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param currency the price of the product, for example 3.50.
     *                 Precision is up to 6 decimal places.
     */
    public void addProductImpressionPrice(int listIndex, int productIndex, double currency)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("pr=");
        sb.AppendFormat("{0:F6}", currency);
    }

    //! (Category:Enhanced E-Commerce) Adds an impression product-level custom dimension.
    /*! \note Normal Google Analytics accounts have max 20 custom dimensions (200 for Premium). It's not clear if that limit applies to impression product-level custom dimensions.
     * \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param dimensionIndex index of the custom dimension (must be between 1 and 200, inclusive).
     * \param text custom dimension text, for example "Member".
     */
    public void addProductImpressionCustomDimension(int listIndex, int productIndex, int dimensionIndex, string text)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        dimensionIndex = clampEnhancedECommerceIndex(productIndex);
        string data = escapeString(text);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("cd");
        sb.Append(dimensionIndex);
        sb.Append('=');
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds an impression product-level custom metric.
    /*! \note Normal Google Analytics accounts have max 20 custom dimensions (200 for Premium). It's not clear if that limit applies to impression product-level custom metrics.
     * \param listIndex impression list index (must be a positive integer between 1 and 200, inclusive).
     * \param productIndex product index (must be a positive integer between 1 and 200, inclusive).
     * \param dimensionIndex index of the custom dimension (must be between 1 and 200, inclusive).
     * \param value custom dimension value as 64-bit signed integer, for example 7447.
     */
    public void addProductImpressionCustomMetric(int listIndex, int productIndex, int dimensionIndex, long value)
    {
        listIndex = clampEnhancedECommerceIndex(listIndex);
        productIndex = clampEnhancedECommerceIndex(productIndex);
        dimensionIndex = clampEnhancedECommerceIndex(productIndex);
        sb.Append("&il");
        sb.Append(listIndex);
        sb.Append("pi");
        sb.Append(productIndex);
        sb.Append("cm");
        sb.Append(dimensionIndex);
        sb.Append('=');
        sb.Append(value);
    }

    //! (Category:Enhanced E-Commerce) Specifies the role of the promotions included in a hit.
    /*! \note If a promotion action is not specified, the default promotion
     *        action "view" is assumed.
     * \param text the role of the promotions included in a hit. To measure a
     *             user click on a promotion, set to "promo_click".
     * \note Measurement Protocol specs are a bit sparse and do not seem to have
     *       a clear list of possible promotion actions for now.
     */
    public void addPromotionAction(string text)
    {
        string data = escapeString(text);
        sb.Append("&promoa=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds promotion ID.
    /*! \param promotionIndex promotion index (must be a positive integer between 1 and 200, inclusive).
     * \param text the promotion ID, for example "SHIP".
     */
    public void addPromotionID(int promotionIndex, string text)
    {
        promotionIndex = clampEnhancedECommerceIndex(promotionIndex);
        string data = escapeString(text);
        sb.Append("&promo");
        sb.Append(promotionIndex);
        sb.Append("id=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds promotion name.
    /*! \param promotionIndex promotion index (must be a positive integer between 1 and 200, inclusive).
     * \param text the name of the promotion, for example "FreeShipping".
     */
    public void addPromotionName(int promotionIndex, string text)
    {
        promotionIndex = clampEnhancedECommerceIndex(promotionIndex);
        string data = escapeString(text);
        sb.Append("&promo");
        sb.Append(promotionIndex);
        sb.Append("nm=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds a creative associated with the promotion.
    /*! \param promotionIndex promotion index (must be a positive integer between 1 and 200, inclusive).
     * \param text the creative associated with the promotion, for example "ShippingBanner".
     */
    public void addPromotionCreative(int promotionIndex, string text)
    {
        promotionIndex = clampEnhancedECommerceIndex(promotionIndex);
        string data = escapeString(text);
        sb.Append("&promo");
        sb.Append(promotionIndex);
        sb.Append("cr=");
        sb.Append(data);
    }

    //! (Category:Enhanced E-Commerce) Adds position of the promotion creative.
    /*! \param promotionIndex promotion index (must be a positive integer between 1 and 200, inclusive).
     * \param text the position of the promotion creative.
     */
    public void addPromotionPosition(int promotionIndex, string text)
    {
        promotionIndex = clampEnhancedECommerceIndex(promotionIndex);
        string data = escapeString(text);
        sb.Append("&promo");
        sb.Append(promotionIndex);
        sb.Append("ps=");
        sb.Append(data);
    }

    // Note:
    // There is no separate methods for following list of Enhanced E-Commerce
    // parameters (which are listed separately in the Measurement Protocol
    // specification). Instead you can use the existing E-Commerce
    // addTransaction...() methods.
    // - Transaction ID, Affiliation, Revenue, Tax, Shipping


    //
    // Social Interactions Measurement Protocol parameters
    //

    //! (Category:Social Interactions) Adds social network type. *Required* for Social hit type!
    /*! \note Only for hits with Social type (**required**).
     * \param text social network type (max 50 bytes), for example "Facebook" or "GooglePlus".
     */
    public void addSocialNetwork(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Social)
            Debug.LogWarning("GUA trying to add Social Network to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Social Network");
#       endif // debug warnings
        if (hitType != HitType.Social)
            return;
        string data = escapeString(text);
        sb.Append("&sn=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 50)
            Debug.LogWarning("GUA Social Network is too long, max 50 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Social Interactions) Adds social interaction type (max 50 bytes). *Required* for Social hit type!
    /*! \note Only for hits with Social type (**required**).
     * For example "like", or when user clicks +1 button on Google Plus, the action is "plus".
     * \param text social interaction type.
     */
    public void addSocialAction(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Social)
            Debug.LogWarning("GUA trying to add Social Action to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Social Action");
#       endif // debug warnings
        if (hitType != HitType.Social)
            return;
        string data = escapeString(text);
        sb.Append("&sa=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 50)
            Debug.LogWarning("GUA Social Action is too long, max 50 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Social Interactions) Adds target of social interaction. *Required* for Social hit type!
    /*! \note Only for hits with Social type (**required**).
     * \param text target of social interaction (max 2048 bytes), for example URL, but can be any text.
     * \param allowNonEscaped text is always escaped unless this is set to true.
     */
    public void addSocialActionTarget(string text, bool allowNonEscaped = false)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Social)
            Debug.LogWarning("GUA trying to add Social Action Target to wrong hit type: " + hitTypeNames[(int)hitType]);
        if (text == null || text.Length == 0)
            Debug.LogWarning("GUA trying to add null or empty Social Action Target");
#       endif // debug warnings
        if (hitType != HitType.Social)
            return;
        string data = allowNonEscaped ? escapeString(text) : WWW.EscapeURL(text);
        sb.Append("&st=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 2048)
            Debug.LogWarning("GUA Social Action Target is too long, max 2048 bytes");
#       endif // debug warnings
    }


    //
    // Timing Measurement Protocol parameters
    //

    //! (Category:Timing) Adds user timing category.
    /*! \note Only for hits with Timing type.
     * \param text user timing category (max 150 bytes).
     */
    public void addUserTimingCategory(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add User Timing Category to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        string data = escapeString(text);
        sb.Append("&utc=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 150)
            Debug.LogWarning("GUA User Timing Category is too long, max 150 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Timing) Adds user timing variable.
    /*! \note Only for hits with Timing type.
     * \param text user timing variable (max 500 bytes).
     */
    public void addUserTimingVariableName(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add User Timing Variable Name to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        string data = escapeString(text);
        sb.Append("&utv=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA User Timing Variable Name is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Timing) Adds user timing value in milliseconds.
    /*! \note Only for hits with Timing type.
     * \param value user timing value in milliseconds, e.g. 123.
     */
    public void addUserTimingTime(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add User Timing Time to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        sb.Append("&utt=");
        sb.Append(value);
    }

    //! (Category:Timing) Adds user timing label.
    /*! \note Only for hits with Timing type.
     * \param text user timing label (max 500 bytes).
     */
    public void addUserTimingLabel(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add User Timing Label to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        string data = escapeString(text);
        sb.Append("&utl=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 500)
            Debug.LogWarning("GUA User Timing Label is too long, max 500 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Timing) Adds time it took for a page to load.
    /*! \note Only for hits with Timing type.
     * \param value time it took for a page to load, in milliseconds, for example 1337.
     */
    public void addPageLoadTime(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add Page Load Time to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        sb.Append("&plt=");
        sb.Append(value);
    }

    //! (Category:Timing) Adds time it took to do a DNS lookup.
    /*! \note Only for hits with Timing type.
     * \param value time it took to do a DNS lookup, in milliseconds, for example 43.
     */
    public void addDNSTime(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add DNS Time to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        sb.Append("&dns=");
        sb.Append(value);
    }

    //! (Category:Timing) Adds time it took for a page to be downloaded.
    /*! \note Only for hits with Timing type.
     * \param value time it took for a page to be downloaded, in milliseconds, for example 7447.
     */
    public void addPageDownloadTime(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add Page Download Time to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        sb.Append("&pdt=");
        sb.Append(value);
    }

    //! (Category:Timing) Adds time it took for any redirects to happen.
    /*! \note Only for hits with Timing type.
     * \param time it took for any redirects to happen, in milliseconds, for example 500.
     */
    public void addRedirectResponseTime(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add Redirect Response Time to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        sb.Append("&rrt=");
        sb.Append(value);
    }

    //! (Category:Timing) Adds time it took for a TCP connection to be made.
    /*! \note Only for hits with Timing type.
     * \param value time it took for a TCP connection to be made, in milliseconds, for example 500.
     */
    public void addTCPConnectTime(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add TCP Connect Time to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        sb.Append("&tcp=");
        sb.Append(value);
    }

    //! (Category:Timing) Adds time it took for the server to respond after the connection time.
    /*! \note Only for hits with Timing type.
     * \param time it took for the server to respond after the connection time, in milliseconds, for example 500.
     */
    public void addServerResponseTime(int value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Timing)
            Debug.LogWarning("GUA trying to add Server Response Time to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Timing)
            return;
        sb.Append("&srt=");
        sb.Append(value);
    }


    //
    // Exceptions Measurement Protocol parameters
    //

    //! (Category:Exceptions) Adds description of an exception.
    /*! \note Only for hits with Exception type.
     * \param text exception description (max 150 bytes), for example "DatabaseError".
     */
    public void addExceptionDescription(string text)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Exception)
            Debug.LogWarning("GUA trying to add Exception Description to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Exception)
            return;
        string data = escapeString(text);
        sb.Append("&exd=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 150)
            Debug.LogWarning("GUA Exception Description is too long, max 150 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Exceptions) Adds info whether exception was fatal or not.
    /*! \note Only for hits with Exception type.
     * \param value true if exception is considered fatal, or false if not.
     */
    public void addExceptionIsFatal(bool value)
    {
#       if DEBUG_WARNINGS
        if (hitType != HitType.Exception)
            Debug.LogWarning("GUA trying to add Exception Is Fatal to wrong hit type: " + hitTypeNames[(int)hitType]);
#       endif // debug warnings
        if (hitType != HitType.Exception)
            return;
        sb.Append(value ? "&exf=1" : "&exf=0");
    }


    //
    // Custom Dimensions / Metrics Measurement Protocol parameters
    //

    //! (Category:Custom Dimensions / Metrics) Adds a custom dimension with associated index.
    /*! \note Normal Google Analytics accounts have max 20 dimensions (200 for Premium).
     * \param index index of the custom dimension, starting from 1. Must be between 1 and 200, inclusive.
     * \param text custom dimension text (max 150 bytes), for example "Sports".
     */
    public void addCustomDimension(int index, string text)
    {
#       if DEBUG_WARNINGS
        if (index < 1 || index > 200)
            Debug.LogWarning("GUA trying to add Custom Dimension with illegal index (must be 1-200): " + index);
#       endif // debug warnings
        if (index < 1 || index > 200)
            return;
        string data = escapeString(text);
        sb.Append("&cd");
        sb.Append(index);
        sb.Append('=');
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 150)
            Debug.LogWarning("GUA Custom Dimension is too long, max 150 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Custom Dimensions / Metrics) Adds a custom metric with associated index.
    /*! \note Normal Google Analytics accounts have max 20 dimensions (200 for Premium).
     * \param index index of the custom dimension, starting from 1. Must be between 1 and 200, inclusive.
     * \param value custom dimension value as 64-bit signed integer, for example 7447.
     */
    public void addCustomMetric(int index, long value)
    {
#       if DEBUG_WARNINGS
        if (index < 1 || index > 200)
            Debug.LogWarning("GUA trying to add Custom Metric with illegal index (must be 1-200): " + index);
#       endif // debug warnings
        if (index < 1 || index > 200)
            return;
        sb.Append("&cm");
        sb.Append(index);
        sb.Append('=');
        sb.Append(value);
    }


    //
    // Content Experiments Measurement Protocol parameters
    //

    //! (Category:Content Experiments) Adds a content experiment ID. Should be used with experiment variant.
    /*! \note Specifies that this user has been exposed to an experiment with
     *        given ID. Should be used in conjunction with addExperimentVariant().
     * \param text experiment ID (max 40 bytes), for example "gNNgUNYYVGFRR2014".
     */
    public void addExperimentID(string text)
    {
        string data = escapeString(text);
        sb.Append("&xid=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        if (data.Length > 40)
            Debug.LogWarning("GUA Experiment ID is too long, max 40 bytes: " + data);
#       endif // debug warnings
    }

    //! (Category:Content Experiments) Adds a content experiment variant. Should be used with experiment ID.
    /*! \note Specifies that this user has been exposed to a particular variation
     *        of an experiment. Should be used in conjunction with addExperimentID().
     * \param text experiment variant, for example "1".
     */
    public void addExperimentVariant(string text)
    {
        string data = escapeString(text);
        sb.Append("&xvar=");
        sb.Append(data);
#       if DEBUG_WARNINGS
        // Note: Max length is not actually specified in measurement protocol
        //       parameter specs (as of now), so this is self-imposed:
        if (data.Length > 40)
            Debug.LogWarning("GUA Experiment Variant is too long, max 40 bytes: " + data);
#       endif // debug warnings
    }



    //
    // Helpers for Common/typical Hit Types
    //

    //! Helper for sending a typical Pageview hit.
    /*! \sa addDocumentHostName
     * \sa addDocumentPath
     * \sa addDocumentTitle
     */
    public void sendPageViewHit(string documentHostName, string documentPath, string documentTitle)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Pageview);
        addDocumentHostName(documentHostName);
        addDocumentPath(documentPath);
        addDocumentTitle(documentTitle);
        sendHit();
    }

    //! Helper for sending a typical Event hit.
    /*! \note eventCategory and eventAction are **required**!
     * \sa addEventCategory
     * \sa addEventAction
     * \sa addEventLabel
     * \sa addEventValue
     */
    public void sendEventHit(string eventCategory, string eventAction, string eventLabel = null, int eventValue = -1)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Event);
        addEventCategory(eventCategory);
        addEventAction(eventAction);
        if (eventLabel != null)
            addEventLabel(eventLabel);
        if (eventValue >= 0)
            addEventValue(eventValue);
        sendHit();
    }

    //! Helper for sending a typical Transaction hit.
    /*! \note transactionID is **required**!
     * \sa addTransactionID
     * \sa addTransactionAffiliation
     * \sa addTransactionRevenue
     * \sa addTransactionShipping
     * \sa addTransactionTax
     * \sa addCurrencyCode
     */
    public void sendTransactionHit(string transactionID, string affiliation, double revenue, double shipping, double tax, string currencyCode)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Transaction);
        addTransactionID(transactionID);
        if (affiliation != null && affiliation.Length > 0)
            addTransactionAffiliation(affiliation);
        if (revenue != 0)
            addTransactionRevenue(revenue);
        if (shipping != 0)
            addTransactionShipping(shipping);
        if (tax != 0)
            addTransactionTax(tax);
        if (currencyCode != null && currencyCode.Length > 0)
            addCurrencyCode(currencyCode);
        sendHit();
    }

    //! Helper for sending a typical Item hit.
    /*! \note transactionID and itemName are **required**!
     * \sa addTransactionID
     * \sa addItemName
     * \sa addItemPrice
     * \sa addItemQuantity
     * \sa addItemCode
     * \sa addItemCategory
     * \sa addCurrencyCode
     */
    public void sendItemHit(string transactionID, string itemName, double price, int quantity, string itemCode, string itemCategory, string currencyCode)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Item);
        addTransactionID(transactionID);
        addItemName(itemName);
        if (price != 0)
            addItemPrice(price);
        if (quantity != 0)
            addItemQuantity(quantity);
        if (itemCode != null && itemCode.Length > 0)
            addItemCode(itemCode);
        if (itemCategory != null && itemCategory.Length > 0)
            addItemCategory(itemCategory);
        if (currencyCode != null && currencyCode.Length > 0)
            addCurrencyCode(currencyCode);
        sendHit();
    }

    //! Helper for sending a typical Social hit.
    /*! \note All parameters are **required**!
     * \sa addSocialNetwork
     * \sa addSocialAction
     * \sa addSocialActionTarget
     */
    public void sendSocialHit(string network, string action, string target)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Social);
        addSocialNetwork(network);
        addSocialAction(action);
        addSocialActionTarget(target);
        sendHit();
    }

    //! Helper for sending a typical Exception hit.
    /*! \sa addExceptionDescription
     * \sa addExceptionIsFatal
     */
    public void sendExceptionHit(string description, bool isFatal)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Exception);
        if (description != null && description.Length > 0)
            addExceptionDescription(description);
        addExceptionIsFatal(isFatal);
        sendHit();
    }

    //! Helper for sending a typical user Timing hit.
    /*! \sa addUserTimingCategory
     * \sa addUserTimingVariableName
     * \sa addUserTimingTime
     * \sa addUserTimingLabel
     */
    public void sendUserTimingHit(string category, string variable, int time, string label)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Timing);
        if (category != null && category.Length > 0)
            addUserTimingCategory(category);
        if (variable != null && variable.Length > 0)
            addUserTimingVariableName(variable);
        if (time >= 0)
            addUserTimingTime(time);
        if (label != null && label.Length > 0)
            addUserTimingLabel(label);
        sendHit();
    }

    //! Helper for sending a browser Timing hit.
    /*! \sa addDNSTime
     * \sa addPageDownloadTime
     * \sa addRedirectResponseTime
     * \sa addTCPConnectTime
     * \sa addServerResponseTime
     */
    public void sendBrowserTimingHit(int dnsTime, int pageDownloadTime, int redirectTime, int tcpConnectTime, int serverResponseTime)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Timing);
        if (dnsTime >= 0)
            addDNSTime(dnsTime);
        if (pageDownloadTime >= 0)
            addPageDownloadTime(pageDownloadTime);
        if (redirectTime >= 0)
            addRedirectResponseTime(redirectTime);
        if (tcpConnectTime >= 0)
            addTCPConnectTime(tcpConnectTime);
        if (serverResponseTime >= 0)
            addServerResponseTime(serverResponseTime);
        sendHit();
    }

    //! Helper for sending a typical app screen hit.
    /*! \sa addScreenName
     */
    public void sendAppScreenHit(string screenName)
    {
        if (analyticsDisabled)
            return;

        beginHit(HitType.Screenview);
        addScreenName(screenName);
        sendHit();
    }


    //
    // Methods which handle queue of hits cached when being offline
    //

#if !UNITY_WEBPLAYER
    private System.DateTime? epoch = null;

    private long getPOSIXTimeMilliseconds()
    {
        if (!epoch.HasValue)
            epoch = new System.DateTime(1970, 1, 1);
        return (long)(System.DateTime.UtcNow - epoch.Value).TotalMilliseconds;
    }

    private void stopOfflineCacheReader()
    {
        if (offlineCacheReader != null)
        {
            #if !NETFX_CORE
            offlineCacheReader.Close();
            #endif
            offlineCacheReader.Dispose();
            offlineCacheReader = null;
        }
    }

    private void stopOfflineCacheWriter()
    {
        if (offlineCacheWriter != null)
        {
            #if !NETFX_CORE
            try
            {
                offlineCacheWriter.Close();
            }
            catch (Exception)
            {
            }
            #endif
            offlineCacheWriter.Dispose();
            offlineCacheWriter = null;
        }
    }

    //! Closes offline cache file (if it's open for reading or writing).
    public void closeOfflineCacheFile()
    {
        stopOfflineCacheReader();
        stopOfflineCacheWriter();
        PlayerPrefs.Save();
    }

    private static string offlineQueueLengthPrefKey = "GoogleUniversalAnalytics_offlineQueueLength";
    private static string offlineQueueSentHitsPrefKey = "GoogleUniversalAnalytics_offlineQueueSentHits";

    private void increasePlayerPrefOfflineQueueLength()
    {
        if (offlineQueueLength == -1)
            offlineQueueLength = PlayerPrefs.GetInt(offlineQueueLengthPrefKey, 0);
        ++offlineQueueLength;
        PlayerPrefs.SetInt(offlineQueueLengthPrefKey, offlineQueueLength);
    }

    private void increasePlayerPrefOfflineQueueSentHits()
    {
        if (offlineQueueSentHits == -1)
            offlineQueueSentHits = PlayerPrefs.GetInt(offlineQueueSentHitsPrefKey, 0);
        ++offlineQueueSentHits;
        PlayerPrefs.SetInt(offlineQueueSentHitsPrefKey, offlineQueueSentHits);
    }

    private void clearOfflineQueue()
    {
        stopOfflineCacheReader();
        stopOfflineCacheWriter();
        try
        {
            File.Delete(offlineCacheFilePath);
        }
        catch (Exception
#              if DEBUG_WARNINGS
               ex
#              endif
               )
        {
#           if DEBUG_WARNINGS
            Debug.LogWarning("GUA can't delete offline cache file " + offlineCacheFilePath + ": " + ex.ToString());
#           endif // debug warnings
            // in unlikely case of error deleting file, we won't remove the 
            // offline queue player prefs (things should still keep working
            // for further cached hits if the file is usable - but we reset
            // the sent hits amount to length, i.e. all "have been sent")
            offlineQueueSentHits = offlineQueueLength;
            PlayerPrefs.SetInt(offlineQueueSentHitsPrefKey, offlineQueueSentHits);
            PlayerPrefs.Save();
            return;
        }
        PlayerPrefs.DeleteKey(offlineQueueSentHitsPrefKey);
        PlayerPrefs.DeleteKey(offlineQueueLengthPrefKey);
        PlayerPrefs.Save();
        offlineQueueLength = 0;
        offlineQueueSentHits = 0;
    }

    private bool saveHitToOfflineQueue(string hitData)
    {
        // must not have the reader open
        stopOfflineCacheReader();

        if (offlineCacheWriter == null)
        {
            try
            {
                offlineCacheWriter = File.AppendText(offlineCacheFilePath);
            }
            catch (Exception
#                  if DEBUG_WARNINGS
                   ex
#                  endif
                  )
            {
#               if DEBUG_WARNINGS
                Debug.LogWarning("GUA can't open (append) offline cache file " + offlineCacheFilePath + ": " + ex.ToString());
#               endif // debug warnings
                sb.Length = 0; // can't help, have to discard after all
                hitType = HitType.None;
                return false;
            }
            offlineCacheWriter.AutoFlush = false;
            offlineCacheWriter.NewLine = "\n";
        }

        try
        {
            long posixTimeMilliseconds = getPOSIXTimeMilliseconds();
#           if DEBUG_LOGS
            Debug.Log("GUA saveHitToOfflineQueue time and data: " + posixTimeMilliseconds + " " + hitData);
#           endif // debug logs

            offlineCacheWriter.Write("0\t"); // version
            offlineCacheWriter.Write(posixTimeMilliseconds);
            offlineCacheWriter.Write('\t');
            offlineCacheWriter.WriteLine(hitData);
            offlineCacheWriter.Flush();

            increasePlayerPrefOfflineQueueLength();
        }
        catch (Exception
#              if DEBUG_WARNINGS
               ex
#              endif
              )
        {
#           if DEBUG_WARNINGS
            Debug.LogWarning("GUA can't write to offline cache file " + offlineCacheFilePath + ": " + ex.ToString());
#           endif // debug warnings
            return false;
        }

        return true;
    }

    //! Returns true if we have pending hits queued when being offline and could send them now.
    public bool pendingQueuedOfflineHits()
    {
        if (!internetReachable || analyticsDisabled)
            return false;
        if (offlineQueueLength == -1)
            offlineQueueLength = PlayerPrefs.GetInt(offlineQueueLengthPrefKey, 0);
        if (offlineQueueSentHits == -1)
            offlineQueueSentHits = PlayerPrefs.GetInt(offlineQueueSentHitsPrefKey, 0);
        return offlineQueueLength > offlineQueueSentHits;
    }

    private static char[] tabRowSplitter = new char[] { '\t' };

    //! Sends one pending hit from the offline queue, if possible.
    public bool sendOnePendingOfflineHit()
    {
        if (!pendingQueuedOfflineHits())
            return false;
        if (isHitBeingBuilt)
        {
#           if DEBUG_WARNINGS
            Debug.LogWarning("GUA can't try to sendOnePendingOfflineHit() because a new hit was already being built and not sent yet");
#           endif // debug warnings
            return false;
        }

        // must not have the writer open
        stopOfflineCacheWriter();

        if (offlineCacheReader == null)
        {
            try
            {
                offlineCacheReader = File.OpenText(offlineCacheFilePath);
            }
            catch (Exception
#                  if DEBUG_WARNINGS
                   ex
#                  endif
                  )
            {
#               if DEBUG_WARNINGS
                Debug.LogWarning("GUA can't open (read) offline cache file " + offlineCacheFilePath + ": " + ex.ToString());
#               endif // debug warnings
                return false;
            }

            // skip entries from the queue file which have already been sent
            try
            {
                for (int a = 0; a < offlineQueueSentHits; ++a)
                {
                    offlineCacheReader.ReadLine(); // not interested in return value
                }
            }
            catch (Exception /*ex*/)
            {
#               if DEBUG_WARNINGS
                Debug.LogWarning("GUA offline cache file had less hits than expected (clearing whole cache)");
#               endif // debug warnings
                // there was less stuff than expected, just clean up
                // (there won't be anything to send)
                clearOfflineQueue();
                return false;
            }
        }

        long posixTimeMillisecondsNow = getPOSIXTimeMilliseconds();

        string line = null;
        bool stopAndClear = false;

        try
        {
            line = offlineCacheReader.ReadLine();
        }
        catch (Exception /*ex*/
#              if DEBUG_WARNINGS
               ex
#              endif
              )
        {
#           if DEBUG_WARNINGS
            Debug.LogWarning("GUA can't read from offline cache file " + offlineCacheFilePath + ": " + ex.ToString());
#           endif // debug warnings
            // there was less stuff than expected, just clean up
            // (there won't be anything to send)
            stopAndClear = true;
        }

        if (line == null || line.Length < 20)
        {
#           if DEBUG_LOGS
            Debug.Log("GUA null or too short line in offline cache - end of data: " + line);
#           endif // debug logs
            stopAndClear = true;
        }

        if (stopAndClear)
        {
            clearOfflineQueue();
            return false;
        }
        string[] tokens = line.Split(tabRowSplitter);

#       if DEBUG_WARNINGS
        if (tokens.Length != 3)
        {
            Debug.LogWarning("GUA offline queue file has erroneous row: " + line);
        }
#       endif // debug warnings
        if (tokens.Length < 3 ||
            !tokens[0].Equals("0")) // cached entry version check as 1st token
        {
            increasePlayerPrefOfflineQueueSentHits(); // skipping row entry
            return false;
        }

        // 2nd token is timestamp when hit was made
        long posixTimeMillisecondsThen;
        if (!Int64.TryParse(tokens[1], out posixTimeMillisecondsThen))
        {
#           if DEBUG_WARNINGS
            Debug.LogWarning("GUA offline queue hit has erroneous time: " + line);
#           endif // debug warnings
            increasePlayerPrefOfflineQueueSentHits(); // skipping row entry
            return false;
        }

        // 3rd token is the hit data
        sb.Append(tokens[2]);

        bool cancelQueuedHit = false;

        // verify w/queue time if we can consider the hit to be valid anymore at all
        long maxHitAgeMs = 3 * (24 * 60 * 60 * 1000) / 2; // max age 1.5 days (optimistic, in reality it's less)
        long queueTime = posixTimeMillisecondsNow - posixTimeMillisecondsThen;
        if (queueTime > 0)
        {
            if (queueTime > maxHitAgeMs)
            {
                cancelQueuedHit = true; // cached hit is too old (GA wouldn't process it)
#               if DEBUG_LOGS
                Debug.Log("GUA not sending too old cached hit with age " + queueTime);
#               endif
            }
            else
                addQueueTime((int)queueTime); // add time delta as queue time
        }
        else if (queueTime < -maxHitAgeMs)
        {
            // Saved time is negative = "in the future". Shouldn't happen,
            // but of course can happen e.g. if device has had wrong clock
            // setting which has been fixed. Here we cancel the hit if it
            // seems to be too far away (more than max age "in the future").
            cancelQueuedHit = true;
#           if DEBUG_LOGS
            Debug.Log("GUA canceling cached hit with time too far in the \"future\": " + queueTime);
#           endif
        }

        increasePlayerPrefOfflineQueueSentHits();

#       if DEBUG_LOGS
        Debug.Log("GUA.sendOnePendingOfflineHit (useHTTPS:" + useHTTPS + ")");
#       endif // debug logs

        if (cancelQueuedHit)
        {
            cancelHit();
        }
        else
            finalizeAndSendHit();

        if (offlineQueueSentHits >= offlineQueueLength)
        {
#           if DEBUG_LOGS
            Debug.Log("GUA offline queue done");
#           endif // debug logs
            clearOfflineQueue();
        }

        return true;
    }

#endif // not webplayer
    // (end of purely offline cache specific methods)


    void addCommonOptionalHitParams()
    {
        // append app name if we have one
        if (appName != null && appName.Length > 0)
        {
            sb.Append("&an=");
            sb.Append(appName);
        }

        // append app ID if we have one
        if (appID != null && appID.Length > 0)
        {
            sb.Append("&aid=");
            sb.Append(appID);
        }

        // append app version if we have one
        if (appVersion != null && appVersion.Length > 0)
        {
            sb.Append("&av=");
            sb.Append(appVersion);
        }

        // append user ID if we have one
        if (userID != null && userID.Length > 0)
        {
            sb.Append("&uid=");
            sb.Append(userID);
        }

        // append user language code if we have one
        if (userLanguage != null && userLanguage.Length > 0)
        {
            sb.Append("&ul=");
            sb.Append(userLanguage);
        }

        // append user IP override if we have one
        if (userIP != null && userIP.Length > 0)
        {
            sb.Append("&uip=");
            sb.Append(userIP);
        }

        // anonymize IP if requested
        if (anonymizeIP)
        {
            sb.Append("&aip=1");
        }

        // append user agent override if we have one
        if (userAgent != null && userAgent.Length > 0)
        {
            sb.Append("&ua=");
            sb.Append(userAgent);
        }

#if UNITY_WEBPLAYER
        if (addWebReferrerPending)
        {
            sb.Append("&dr=\"+escape(document.referrer)+\"");
            addWebReferrerPending = false;
        }
#endif
    }

    //! Finalizes the "Hit" being described, and tries to send or cache it.
    /*! The request is processed in background (either by an UnityEngine.WWW
     *  object, or by the browser when using web player in browser).
     *  If internet is not reachable and offline hits queueing is enabled,
     *  the hit is saved to a queue file and sending of it will be tried
     *  later.
     * \return true when successful (hit is sent, or cached when being offline
     *         and queueing is enabled), or false in error case (e.g. no Hit
     *         being constructed or we're offline with queueing disabled).
     */
    public bool sendHit()
    {
        if (analyticsDisabled)
        {
            // Analytics disabled (e.g. through user opt-out): discard hit.
            cancelHit();
            return false;
        }

#       if DEBUG_WARNINGS
        if (hitType == HitType.None)
            Debug.LogWarning("GUA trying to sendHit of type None (without beginHit?)");
        ++sessionHitCount;
        if (sessionHitCount > sessionHitLimit)
            Debug.LogWarning("GUA sending too many hits per session (limit:" + sessionHitLimit + ", sent:" + sessionHitCount + ")");
        if (sessionHitCountResetPending)
        {
            sessionHitCount = 0;
            sessionHitCountResetPending = false;
        }
#       endif // debug warnings

        if (hitType == HitType.None)
            return false;

        addCommonOptionalHitParams();

        // if internet is not reachable, hit is queued or discarded
        if (!internetReachable)
        {
#           if UNITY_WEBPLAYER
            return false; // no offline queueing with webplayer
#           else // or not webplayer:
            if (!useOfflineCache)
            {
                // shouldn't really get here, but handle this just in case
                sb.Length = 0; // discard
                hitType = HitType.None;
                return false;
            }
            string hitData = sb.ToString();
            sb.Length = 0;
            hitType = HitType.None;
            return saveHitToOfflineQueue(hitData);
#           endif // not webplayer
        }

#       if DEBUG_LOGS
        Debug.Log("GUA.sendHit (useHTTPS:" + useHTTPS + ")");
#       endif // debug logs

        finalizeAndSendHit();

        return true;
    }

#if !UNITY_WEBPLAYER
    private WWW beginWWWRequest(string postDataString)
    {
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(postDataString);
#       if DEBUG_WARNINGS
        if (postData.Length >= 8192)
            Debug.LogWarning("GUA hit post data is at or over maximum length of 8192 bytes! (" + postData.Length + " bytes)");
#       endif // debug warnings

        if (customHeaders != null)
            return new WWW(useHTTPS ? httpsCollectUrl : httpCollectUrl, postData, customHeaders);
        else
            return new WWW(useHTTPS ? httpsCollectUrl : httpCollectUrl, postData);
    }

    private IEnumerator doWWWRequestAndCheckResult(string postDataString)
    {
        bool sendFailed_saveToOffline;

        WWW www = beginWWWRequest(postDataString);
        yield return www;

        if (www.error != null && www.error.Length > 0)
        {
#           if DEBUG_LOGS
            Debug.Log("GUA hit sending failed (lost network? saving to offline cache), www.error: " + www.error);
#           endif
            netAccessStatus = NetAccessStatus.Error;
            sendFailed_saveToOffline = true;
        }
        else
        {
            byte[] result = www.bytes;
            if (result != null && result.Length > 3 &&
                result[0] == 'G' && result[1] == 'I' && result[2] == 'F')
            {
#               if DEBUG_LOGS
                Debug.Log("GUA hit was sent successfully");
#               endif
                sendFailed_saveToOffline = false;
            }
            else
            {
#               if DEBUG_LOGS
                Debug.Log("GUA hit sending returned erroneous response (network login screen?)");
                //Debug.Log(www.data); // for debug peeking at data
#               endif
                netAccessStatus = NetAccessStatus.Mismatch;
                sendFailed_saveToOffline = true;
            }
        }

        if (sendFailed_saveToOffline)
        {
            // note: when this one will eventually be sent again, there will be two cache busters but that's ok
            saveHitToOfflineQueue(postDataString);
        }
    }
#endif // not webplayer

    // Returns a WWW object only if needReturnWWW is true and not running in a web player.
    private WWW finalizeAndSendHit(bool needReturnWWW = false)
    {
        // append cache buster (should be last)
        sb.Append("&z=");
        sb.Append(UnityEngine.Random.Range(0, 0x7fffffff) ^ 0x13377AA7);

#       if UNITY_WEBPLAYER
        sb.Append("\");}");
#       endif // webplayer

        string postDataString = sb.ToString();
#       if DEBUG_LOGS
        Debug.Log("GUA postDataString: " + postDataString);
#       endif // debug logs

        // reset string builder and currently built hit type
        sb.Length = 0;
        hitType = HitType.None;

#       if UNITY_WEBPLAYER

        Application.ExternalEval(postDataString);
        return null;

#       else // else other than webplayer platform:

        if (useOfflineCache && !needReturnWWW)
        {
            helperBehaviour.StartCoroutine(doWWWRequestAndCheckResult(postDataString));
            return null;
        }
        else // caller needs the returned WWW object:
        {
            return beginWWWRequest(postDataString);
        }

#       endif // not webplayer
    }
}
