﻿using UnityEngine;
using System;


class LocalNotification
{
    /// <summary>
    /// Inexact uses `set` method
    /// Exact uses `setExact` method
    /// ExactAndAllowWhileIdle uses `setAndAllowWhileIdle` method
    /// Documentation: https://developer.android.com/intl/ru/reference/android/app/AlarmManager.html
    /// </summary>
    public enum NotificationExecuteMode
    {
        Inexact = 0,
        Exact = 1,
        ExactAndAllowWhileIdle = 2
    }

    private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";
    private static string mainActivityClassName = "com.localytics.android.unity.LocalyticsUnityPlayerActivity";

    public static void SendNotification(int id, TimeSpan delay, string title, string message)
    {
#if UNITY_ANDROID
        SendNotification(id, (int)delay.TotalSeconds, title, message, Color.white);
#elif UNITY_IOS
        var k = new UnityEngine.iOS.LocalNotification
        {
            fireDate = DateTime.Now.AddSeconds(delay.TotalSeconds),
            alertBody = message
        };

        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(k);
#endif
    }

    public static void SendNotification(int id, long delay, string title, string message, Color32 bgColor,   bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        Debug.Log("pluginClass != null =="+ (pluginClass != null));

        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", id, delay * 1000L,
                    title, message, message, sound ? 1 : 0, 
                    vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, 
                    "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, 
                    (int)executeMode, mainActivityClassName);
        }
#endif
    }

    public static void CancelNotification(int id)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("CancelNotification", id);
        }
#endif
    } 

    public static void CancelAllNotifications()
    {
#if UNITY_IOS
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif
#if UNITY_ANDROID
        for (int j = 0; j < 10; j++)
            CancelNotification(j);
#endif
    }

    //public static void CancelAllNotifications()
    //{
    //#if UNITY_ANDROID && !UNITY_EDITOR
    //    AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
    //    if (pluginClass != null)
    //        pluginClass.CallStatic("CancelAll");
    //#endif
    //}
}