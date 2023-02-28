using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Contains multiple animator controllers and syncs them
*/
public class MultiAnimator : MonoBehaviour
{
    [SerializeField] List<Animator> animators = new List<Animator>();

    public void SetBool(string name, bool value) {
        foreach(Animator animator in animators) {
            animator.SetBool(name,value);
        }
    }

    public void SetTrigger(string name) {
        foreach(Animator animator in animators) {
            animator.SetTrigger(name);
        }
    }
}
