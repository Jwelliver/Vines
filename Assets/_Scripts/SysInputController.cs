/*
092223

Manages Key inputs like restart, escape, etc.
*/

//! 092223 not in use (no time; going to handle in gamemanager for now)
using UnityEngine;
using UnityEngine.SceneManagement;

public class SysInputController : MonoBehaviour
{
    [Header("KeyCodes")]
    [SerializeField] KeyCode escape = KeyCode.Escape;
    [SerializeField] KeyCode restartLevel = KeyCode.R;
    [Header("Object References")]
    [SerializeField] LevelLoader levelLoader;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(restartLevel))
        {
            RestartLevel();
        }
        if (Input.GetKeyDown(escape))
        {
            HandleEscape();
        }
    }

    void RestartLevel()
    {
        //if current scene is the game, then ask levelLoader to restart
        if (SceneManager.GetActiveScene().name == "GameMain")
        {
            levelLoader.reloadCurrentLevel();
        }
    }

    void HandleEscape()
    {
        //if current scene is game main, show pause menu;

    }

}
