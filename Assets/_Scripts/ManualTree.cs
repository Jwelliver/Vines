using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualTree : MonoBehaviour
{
    [SerializeField] TreeFactory treeFactory;
    [SerializeField] TreeFactoryConfig treeFactoryConfig;
    [SerializeField] VineFactoryConfig vineFactoryConfig;
    [SerializeField] LevelGenerator levelGenerator;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 pos = new Vector2(transform.position.x, levelGenerator.GetCurrentSection().startPos.y);
        treeFactory.GenerateTree(pos, transform, treeFactoryConfig, vineFactoryConfig);
    }
}
