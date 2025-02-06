using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DDEntityBase : DDScriptableObject
{
    [SerializeField]
    private Texture image;
    public Texture Image => image;

    public int StartingHealth;

    public int StartingArmor;

    [SerializeField]
    private string enemyName;
    public string EnemyName => enemyName;

    [SerializeField]
    private bool immovable = false;
    public bool Immovable => immovable;

    [SerializeField]
    private bool friendly;
    public bool Friendly => friendly;

    [SerializeField]
    private ERangeType rangeType = ERangeType.None;
    public ERangeType RangeType => rangeType;

    public abstract List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy);
}