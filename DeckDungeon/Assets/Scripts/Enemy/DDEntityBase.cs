using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DDEntityBase : DDScriptableObject
{
    [SerializeField]
    private Texture image;
    public Texture Image { get { return image; } }

    public int StartingHealth;

    public int StartingArmor;

    [SerializeField]
    private string enemyName;
    public string EnemyName { get => enemyName; }

    [SerializeField]
    private bool immovable = false;
    public bool Immovable { get { return immovable; } }

    [SerializeField]
    private bool friendly;
    public bool Friendly { get { return friendly; } }

    public abstract List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy);
}