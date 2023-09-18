using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PalmLeafSpriteDef
{
    public Sprite sprite;
    public bool flipXOver180; // if the sprite's rotation is greater than 180, then flip X axis;
    public bool flipXUnder180; // if rotation<180, then flipX
}

// public struct PalmLeafSpriteDefCollection {
//     List<PalmLeafSpriteDef> sprites;
// }

public class AngleRangeCheck
{

    bool isOverlapRange;
    // public int nDegrees;
    int lo1;
    int hi1;
    //lo2 and hi2 are used for splitting range when angle is near reset (0/360)
    int lo2;
    int hi2;
    public AngleRangeCheck(int centerAngle, int nDegrees)
    {
        Debug.Log(centerAngle);
        if (centerAngle != 180 && centerAngle != 0)
        {
            throw new Exception("This class isn't working with degrees other than 0 or 180");
        }
        int split = (int)nDegrees / 2;
        if (centerAngle + split > 359 || centerAngle - split < 0)
            InitOverlapRange(centerAngle, nDegrees);
        else
            InitRange(centerAngle, nDegrees);
    }

    private void InitRange(int centerAngle, int nDegrees)
    {
        int split = (int)nDegrees / 2;
        lo1 = centerAngle - split;
        hi1 = centerAngle + split;
    }

    private void InitOverlapRange(int centerAngle, int nDegrees)
    {
        //test1 (centerAngle>180) centerAngle: 355; nDegrees: 20; split:10; correctRange: lo1: 355-split=345; hi1:359; lo2: 0; hi2: 355+split=5; 
        // ... vars: lo1:centerAngle-split; hi1: 359; lo2:0; hi2: centerAngle+split;
        //test2 (centerAngle<180) centerAngle: 5; nDegrees: 20; split:10; correctRange: lo1:5-0=5; hi1:5+split=15; lo2:360-(5-split)=; hi:359
        //  .... vars: lo1:centerAngle; hi1: centerAngle+split; lo2: 360-abs((centerAngle-split)); hi:359
        isOverlapRange = true;
        int split = (int)nDegrees / 2;
        if (centerAngle > 180)
        {
            lo1 = centerAngle - split;
            hi1 = 359;
            lo2 = 0;
            hi2 = centerAngle + split;
        }
        else
        {
            lo1 = centerAngle;
            hi1 = centerAngle + split;
            lo2 = 360 - Mathf.Abs(centerAngle - split);
            hi2 = 359;
        }
    }

    public bool IsInRange(int angle)
    {
        bool inRange = false;
        if (angle > lo1 && angle < hi1)
        {
            inRange = true;
        }
        if (isOverlapRange && angle < lo2 && angle > hi2)
        {
            inRange = false;
        }
        return inRange;
    }
}


public class PalmLeaf : MonoBehaviour
{

    //need palm sprites

    [SerializeField] int nRotations = 10;
    [SerializeField] float minScale;
    [SerializeField] float maxScale;
    [SerializeField] Transform palmLeafPrefab;
    [SerializeField] List<PalmLeafSpriteDef> sprites = new List<PalmLeafSpriteDef>();

    // Start is called before the first frame update
    void Start()
    {
        CreatePalmLeaves();
    }


    void CreatePalmLeaves()
    {
        // 150-210, 30-0, 330-359 don't use
        //
        //

        // int bottomSpacingDegrees = 60;
        // int topSpacingDegrees = 60;
        // AngleRangeCheck topExcludeRange = new AngleRangeCheck(0, topSpacingDegrees);
        // AngleRangeCheck bottomExcludeRange = new AngleRangeCheck(180, bottomSpacingDegrees);

        int rotationsCompleted = 0;
        int rotationIncrement = 32;
        for (int angle = 0; rotationsCompleted < nRotations; angle += rotationIncrement)
        {
            // if angle is greater than 360, reset the angle to zero and increment rotationsCompleted;
            if (angle >= 360)
            {
                angle = 0;
                rotationsCompleted++;
                continue;
            }

            // Except for a small pct chance, skip over the following ranges (prevents palms from being too much on top or bottom)
            if ((angle >= 150 && angle <= 210) || (angle >= 0 && angle <= 30) || (angle >= 330 && angle <= 359))
            // if (topExcludeRange.IsInRange(angle) || bottomExcludeRange.IsInRange(angle))
            {
                if (!RNG.SampleProbability(0.05f)) { continue; }
            }
            // get a random palmLeaf
            PalmLeafSpriteDef palmLeafSpriteDef = RNG.RandomChoice(sprites);
            Sprite rndPalmSprite = palmLeafSpriteDef.sprite;
            Transform newPalmLeaf = GameObject.Instantiate(palmLeafPrefab, transform.position, Quaternion.identity);
            // apply random scale
            float scale = RNG.RandomRange(minScale, maxScale);
            newPalmLeaf.localScale = new Vector2(scale, scale);
            // float randomRotation = RNG.RandomRange(0, 365);
            newPalmLeaf.rotation = Quaternion.Euler(0, 0, angle);
            SpriteRenderer palmSpriteRenderer = newPalmLeaf.gameObject.GetComponent<SpriteRenderer>();
            palmSpriteRenderer.sprite = rndPalmSprite;
            //flip sprite per the palmLeafSpriteDef based on the current rotationangle;
            if (angle < 180 && palmLeafSpriteDef.flipXUnder180)
            {
                palmSpriteRenderer.flipX = true;
            }
            else if (angle > 180 && palmLeafSpriteDef.flipXOver180)
            {
                palmSpriteRenderer.flipX = true;
            }
            //set Palms as parent;
            newPalmLeaf.SetParent(transform);

            // set rotationIncrement to random 
            rotationIncrement = RNG.RandomRange(20, 50);
        }
    }


    //==== orig
    // void CreatePalmLeaves()
    // {
    //     int rotationIncrement = 25;
    //     for (int i = 0; i / 360 < nRotations; i += rotationIncrement)
    //     {
    //         Sprite rndPalmSprite = RNG.RandomChoice(sprites).sprite;
    //         Transform newPalmLeaf = GameObject.Instantiate(palmLeafPrefab, transform.position, Quaternion.identity);
    //         float scale = RNG.RandomRange(minScale, maxScale);
    //         newPalmLeaf.localScale = new Vector2(scale, scale);
    //         // float randomRotation = RNG.RandomRange(0, 365);
    //         newPalmLeaf.rotation = Quaternion.Euler(0, 0, i);
    //         SpriteRenderer palmSpriteRenderer = newPalmLeaf.gameObject.GetComponent<SpriteRenderer>();
    //         palmSpriteRenderer.sprite = rndPalmSprite;
    //         newPalmLeaf.SetParent(transform);
    //     }
    // }
}
