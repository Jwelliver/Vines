using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitApplication : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    void HandleEscape()
    {
#if UNITY_WEBGL
Debug.Log("ExitApplication.cs > HandleEscape: On WebGl; Ignoring.");
            return; //ignore escape on webgl
#endif

        if (SceneManager.GetActiveScene().name == "GameMain")
        {
            Debug.Log("ExitApplication > HandleEscape: In Main Game; Ignoring.");
            return; //ignore escape in main game (will be handled by gamemanager)
        }

        Application.Quit();

    }
}
