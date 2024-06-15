using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class DDEnemyBase : DDScriptableObject
{
    [SerializeField]
    private Texture image;
    public Texture Image { get { return image; } }


    public int StartingHealth;

    [SerializeField]
    private string enemyName;
    public string EnemyName { get => enemyName; }


    public abstract List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy);
}