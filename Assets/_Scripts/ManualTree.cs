using UnityEngine;

public class ManualTree : MonoBehaviour
{
    [SerializeField] TreeFactory treeFactory;
    [SerializeField] TreeFactoryConfig treeFactoryConfig;
    [SerializeField] VineFactoryConfig vineFactoryConfig;
    bool isInitialized; // this is set to false until called externally; 

    public void AttemptInit()
    {
        if (isInitialized) { return; }
        Vector2 pos = new Vector2(transform.position.x, LevelGenerator.Instance.GetCurrentSection().startPos.y);
        treeFactory.GenerateTree(pos, transform, treeFactoryConfig, vineFactoryConfig);
        isInitialized = true;
    }
}
