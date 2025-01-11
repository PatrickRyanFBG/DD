using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAdventurerDataLibrary : MonoBehaviour
{
    [SerializeField]
    private List<DDAdventurerData> adventureDatas;
    public List<DDAdventurerData> AdventureDatas { get => adventureDatas; }
}
