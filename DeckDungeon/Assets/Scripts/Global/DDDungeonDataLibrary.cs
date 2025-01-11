using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonDataLibrary : MonoBehaviour
{
    [SerializeField]
    private List<DDDungeonMetaData> metaDatas;
    public List<DDDungeonMetaData> MetaDatas { get => metaDatas; }

    [SerializeField]
    private List<DDDungeonSideQuestData> sideQuestDatas;
    public List<DDDungeonSideQuestData> SideQuestDatas { get => sideQuestDatas; }
}
