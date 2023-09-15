using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/AppSettings")]
public class AppSettings : ScriptableObject
{
    [Header("Desktop")]
    [SerializeField] int targetFrameRate_desktop = 60;
    [SerializeField] int vSyncCount_desktop = 0;

    [Header("WebGL")]
    [SerializeField] int targetFrameRate_webGl = -1;
    [SerializeField] int vSyncCount_webGl = 0;


    void OnValidate()
    {
        UpdateSettings();
    }

    void UpdateSettings()
    {
        int targetFrameRate;
        int vSyncCount;
#if UNITY_WEBGL
        // https://docs.unity3d.com/ScriptReference/QualitySettings-vSyncCount.html
        vSyncCount = vSyncCount_webGl;
        // https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html
        targetFrameRate = targetFrameRate_webGl;
        Debug.Log("AppSettings: Using Webgl settings ");
#else
                    vSyncCount = vSyncCount_desktop;
                    targetFrameRate = targetFrameRate_desktop;
                    Debug.Log("AppSettings: Using Desktop settings ");
#endif

        QualitySettings.vSyncCount = vSyncCount;
        Application.targetFrameRate = targetFrameRate;
        Debug.Log("AppSettings: vSyncCount: " + vSyncCount + " | targetFrameRate: " + targetFrameRate);
    }

}
