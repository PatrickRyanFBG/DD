using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonData : DDScriptableObject
{
    [SerializeField]
    private string dungeonName;

    [SerializeField]
    private List<DDDungeonCardBase> cards;
    public List<DDDungeonCardBase> Cards { get => cards; }
}
