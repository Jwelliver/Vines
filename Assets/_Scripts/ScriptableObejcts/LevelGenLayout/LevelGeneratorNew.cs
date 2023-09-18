using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyAssets/ScriptableObjects/Layouts/Level Generator (layout version)")]
public class LevelGeneratorNew : ScriptableObject
{
    [Header("Level")]
    [SerializeField] string proceduralLevelContainerName = "ProceduralLevelContainer";
    [SerializeField] LevelSettings levelSettings;

    [Header("MainTree Layout")]
    [SerializeField] ProceduralObjectLayout mainTreeLayout; 

    [Header("Background Layout")]
    [SerializeField] BackgroundSpriteLayoutGroup backgroundLayoutGroup;

    [Header("Prefabs")]

    [SerializeField] Transform winPlatformPrefab;

    Section levelSection;

    private void initLevelSection()
    {
        levelSection = new Section
        {
            startPos = levelSettings.startPos,
            length = RNG.RandomRange(levelSettings.minLength, levelSettings.maxLength),
            startOffset = levelSettings.globalStartOffset,
            endOffset = levelSettings.globalEndOffset
        };
    }

    public void generateLevel()
    {
        initLevelSection();
        addBackgroundLayerSection(levelSection);
        addTreeLayerSection(levelSection, levelSettings.treeLayoutParams);
        addWinPlatform(new Vector2(levelSection.length, levelSettings.startPos.y - 1.5f));
    }

    void addTreeLayerSection(Section section, ProceduralLayoutParams layoutParams)
    {
        string treeLayerParentPath = proceduralLevelContainerName + "/" + "TreeContainer";
        Transform parent = GameObject.Find(treeLayerParentPath).transform ?? null;
        mainTreeLayout.layoutParams = layoutParams;
        mainTreeLayout.GenerateSection(section, parent);
    }

    void addBackgroundLayerSection(Section section)
    {
        backgroundLayoutGroup.GenerateAllLayouts(section);
    }

    void addWinPlatform(Vector2 pos)
    {
        GameObject.Instantiate(winPlatformPrefab, pos, Quaternion.identity);
    }


}
