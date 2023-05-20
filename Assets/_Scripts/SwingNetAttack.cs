using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingNetAttack : MonoBehaviour
{
    [SerializeField] Transform normalPlayerArm;
    [SerializeField] Transform attackPlayerArm;
    [SerializeField] Net net;
    [SerializeField] float attackTime;


    public void attack() {
        syncAttackArmToNormalArm();
        toggleAttackArm(true);
        StartCoroutine(waitForAttack());
    }

    IEnumerator waitForAttack() {
        yield return new WaitForSeconds(1f);
        syncNormalArmToAttackArm();
        // net.clearCapturedObjects();
        toggleAttackArm(false);
        
    }

    void syncAttackArmToNormalArm() {
        attackPlayerArm.position = normalPlayerArm.position;
        attackPlayerArm.rotation = normalPlayerArm.rotation;
    }

    void syncNormalArmToAttackArm() {
        normalPlayerArm.position = attackPlayerArm.position;
        normalPlayerArm.rotation = attackPlayerArm.rotation;
    }

    void toggleAttackArm(bool state) {
        normalPlayerArm.gameObject.SetActive(!state);
        attackPlayerArm.gameObject.SetActive(state);
    }

}
