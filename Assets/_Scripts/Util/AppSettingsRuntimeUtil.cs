using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppSettingsRuntimeUtil : MonoBehaviour
{
    // [SerializeField] KeyCode devCommand = KeyCode.Period;
    // [SerializeField] KeyCode vSyncCount = KeyCode.V;
    // [SerializeField] KeyCode targetFrameRateToggle = KeyCode.T;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            QualitySettings.vSyncCount = 1;
            Debug.Log("New VsyncCount: " + QualitySettings.vSyncCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            QualitySettings.vSyncCount = 2;
            Debug.Log("New VsyncCount: " + QualitySettings.vSyncCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            QualitySettings.vSyncCount = 3;
            Debug.Log("New VsyncCount: " + QualitySettings.vSyncCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            QualitySettings.vSyncCount = 4;
            Debug.Log("New VsyncCount: " + QualitySettings.vSyncCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            QualitySettings.vSyncCount = 0;
            Debug.Log("New VsyncCount: " + QualitySettings.vSyncCount);

        }
    }


}
