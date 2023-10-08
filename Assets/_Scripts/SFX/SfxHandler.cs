using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxHandler : MonoBehaviour
{
    public static PlayerSFX playerSFX;
    public static VineSFX vineSFX;
    public static ArrowSFX arrowSFX;

    void Awake()
    {
        playerSFX = gameObject.GetComponentInChildren<PlayerSFX>();
        vineSFX = gameObject.GetComponentInChildren<VineSFX>();
        arrowSFX = gameObject.GetComponentInChildren<ArrowSFX>();

    }
}
