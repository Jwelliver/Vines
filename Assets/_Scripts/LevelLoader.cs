using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] float transitionTime = 2f;
    [SerializeField] bool allowNextLevelKey;
    [SerializeField] KeyCode nextLevelKeyCode = KeyCode.Space;

    public void loadNextLevel()
    {
        StartCoroutine(loadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void reloadCurrentLevel()
    {
        Resources.UnloadUnusedAssets();//added 090623 in attempt to address mem issue and crash
        StartCoroutine(loadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    private IEnumerator loadLevel(int levelIndex)
    {
        animator.SetTrigger("startCrossfade");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    void Update()
    {
        if (allowNextLevelKey && Input.GetKeyDown(nextLevelKeyCode))
        {
            loadNextLevel();
        }
    }


}


//====091223 attempt to refactor with individual fadeIn and fadeout methods

// public class LevelLoader : MonoBehaviour
// {

//     [SerializeField] Animator animator;
//     [SerializeField] float transitionTime = 2f;
//     [SerializeField] bool allowNextLevelKey;
//     [SerializeField] KeyCode nextLevelKeyCode = KeyCode.Space;

//     private bool isTransitioning;
//     // private bool isFaded; // state tracking whether scene is currently faded to black


//     void Start()
//     {
//         SceneManager.sceneLoaded += OnSceneLoaded;
//         DontDestroyOnLoad(gameObject);
//     }

//     void Update()
//     {
//         if (!isTransitioning && allowNextLevelKey && Input.GetKeyDown(nextLevelKeyCode))
//         {
//             fadeToNextScene();
//         }
//     }

//     void OnDestroy()
//     {
//         StopAllCoroutines();
//     }

//     // public void loadNextLevel() {
//     //     StartCoroutine(loadLevel(SceneManager.GetActiveScene().buildIndex+1));
//     // }


//     // public void loadNextLevel() {
//     //     int nextSceneIndex = SceneManager.GetActiveScene().buildIndex+1;
//     //     SceneManager.LoadScene(nextSceneIndex);
//     // }

//     int getNextSceneIndex()
//     {
//         return SceneManager.GetActiveScene().buildIndex + 1;
//     }

//     public void fadeToSceneIndex(int targetSceneIndex)
//     {
//         StartCoroutine(fadeOutAndLoadScene(targetSceneIndex));
//     }

//     public void fadeToNextScene()
//     {
//         fadeToSceneIndex(getNextSceneIndex());
//     }
// ;
//     void fadeOutCurrentScene()
//     {
//         Debug.Log("FadeOut");
//         animator.SetTrigger("fadeOut");
//     }

//     void fadeInCurrentScene()
//     {
//         Debug.Log("FadeIn");
//         animator.SetTrigger("fadeIn");
//     }

//     public void reloadCurrentLevel()
//     {
//         Resources.UnloadUnusedAssets();//added 090623 in attempt to address mem issue and crash
//         StartCoroutine(fadeOutAndLoadScene(SceneManager.GetActiveScene().buildIndex));
//     }

//     private IEnumerator fadeOutAndLoadScene(int levelIndex)
//     {
//         isTransitioning = true;
//         // animator.SetTrigger("startCrossfade");
//         fadeOutCurrentScene();
//         yield return new WaitForSeconds(transitionTime);
//         SceneManager.LoadScene(levelIndex);
//         yield return new WaitForSeconds(transitionTime);
//         isTransitioning = false;
//         yield break;
//     }

//     // called second
//     void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         Debug.Log("OnSceneLoaded > sceneindex: " + scene.buildIndex + " | isTransitioning: " + isTransitioning);
//         if (isTransitioning)
//         {
//             fadeInCurrentScene();
//         }
//     }

// }