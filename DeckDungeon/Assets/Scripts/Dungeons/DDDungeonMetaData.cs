using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is temp? Maybe move this somewhere else
[System.Serializable]
public class DDDungeonMetaData : DDScriptableObject
{
    [SerializeField]
    private string dungeonChainName;

    [SerializeField, Multiline]
    private string dungeonDecsription;

    [SerializeField]
    private Texture heroShot;

    [SerializeField]
    private List<DDDungeonData> dungeonOrder;
    public List<DDDungeonData> DungeonOrder { get => dungeonOrder; }

    // In here dungeon specific events
    // Artifacts
    // ect
    public void SetUpUI(DDDungeonSelection selectionUI)
    {
        selectionUI.DungeonName.text = dungeonChainName;
        selectionUI.DungeonDescription.text = dungeonDecsription;
        selectionUI.HeroShotImage.texture = heroShot;
    }
}
