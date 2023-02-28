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

    public void loadNextLevel() {
        StartCoroutine(loadLevel(SceneManager.GetActiveScene().buildIndex+1));
    }

    public void reloadCurrentLevel() {
        StartCoroutine(loadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    private IEnumerator loadLevel(int levelIndex) {
        animator.SetTrigger("startCrossfade");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    void Update() {
        if(allowNextLevelKey && Input.GetKeyDown(nextLevelKeyCode)) {
            loadNextLevel();
        }
    }


}
