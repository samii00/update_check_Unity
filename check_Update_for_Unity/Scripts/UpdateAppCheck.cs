using Google.Play.AppUpdate;
using Google.Play.Common;
using MyGooglePlaySave;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/**
 * 
 * 
 _ this is a script for check update using a service conection to store.
 _ dont forget to add .aar library to Assets/plagin/Android 
 * 
 * 
 */
public class UpdateAppCheck 
{
    int storeId;
    readonly int Bazaar = 0;
    readonly int Myket = 1;

    private static UpdateAppCheck pointer;
    public static UpdateAppCheck Instacne
    {
        get
        {
            if (pointer == null)
                pointer = FindObjectOfType<UpdateAppCheck>();

            if (pointer == null)
            {
                GameObject UpdateServiceObject = new GameObject("_UpdateCheckService_", typeof(UpdateAppCheck));
                //GooglePlayServiceObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                DontDestroyOnLoad(UpdateServiceObject);
                pointer = UpdateServiceObject.GetComponent<UpdateAppCheck>();

                Debug.Log("UpdateCheckService created");
            }

            return pointer;
        }
    }

    /*private void Start()
    {
        updateCheck();
    }*/
    public void updateCheck()
    {
        if (storeId == Bazaar)
        {
            StartCoroutine(StartCheckUpdate_Bazaar_Myket("ir.unides.updatecheck.UpdateCheckManger"));
        }
        else if (storeId == Myket)
        {
            StartCoroutine(StartCheckUpdate_Bazaar_Myket("ir.myket.developerapi.UpdateCheckManger"));
        }
    }

    IEnumerator StartCheckUpdate_Bazaar_Myket(string PluginPackagMame)
    {
        //Debug.Log("start check ");
        using (AndroidJavaClass UpdateCheckManger = new AndroidJavaClass(PluginPackagMame))
        using (AndroidJavaObject ActivityInstance = UpdateCheckManger.CallStatic<AndroidJavaObject>("ActivityInstance"))
        {
            ActivityInstance.Call("initService");
            // Wait until condition is met
            yield return new WaitUntil(() => ActivityInstance.Call<long>("vCode") != 0);
            long availableVersion = ActivityInstance.Call<long>("vCode");
            if (availableVersion > GetVersionCode())
            {
                // update Ui now (show update button)
                // Or save a pref to enable update Buton in Ui then
            }
            ActivityInstance.Call("releaseService"); //stop service connection to store to prevent memory leak
            Debug.Log("end check ");

            pointer = null;
            Destroy(gameObject);
        }

    }

    int GetVersionCode()
    {
        using (AndroidJavaClass contextCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject context = contextCls.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject packageMngr = context.Call<AndroidJavaObject>("getPackageManager"))
        {
            string packageName = context.Call<string>("getPackageName");
            AndroidJavaObject packageInfo = packageMngr.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
            return packageInfo.Get<int>("versionCode");
        }
    }

}
