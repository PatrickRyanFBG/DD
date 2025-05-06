using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class DDEntityBase : DDScriptableObject
{
    [SerializeField]
    private Texture image;
    public Texture Image => image;

    public int StartingHealth;

    public int StartingArmor;

    [FormerlySerializedAs("enemyName")] [SerializeField]
    private string entityName;
    public string EntityName => entityName;

    [SerializeField]
    private bool immovable = false;
    public bool Immovable => immovable;

    [SerializeField]
    private bool friendly;
    public bool Friendly => friendly;

    [SerializeField]
    private ERangeType rangeType = ERangeType.Pure;
    public ERangeType RangeType => rangeType;

    public abstract List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy);

    public virtual IEnumerator OnDeath()
    {
        yield return null;
    }
}