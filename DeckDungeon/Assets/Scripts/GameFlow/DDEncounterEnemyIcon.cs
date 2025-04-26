using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDEncounterEnemyIcon : MonoBehaviour
{
    [SerializeField] private RawImage image;

    private DDEnemyOnBoard currentEnemy;
    public DDEnemyOnBoard CurrentEnemy => currentEnemy;
    
    public void Setup(DDEnemyOnBoard enemy)
    {
        currentEnemy = enemy;
        image.texture = currentEnemy.CurrentEnemy.Image;
    }

    public void Hovered()
    {
        currentEnemy.Hovered();
    }

    public void Unhovered()
    {
        if(currentEnemy)
        {
            currentEnemy.Unhovered();
        }
    }

    private void OnDisable()
    {
        Unhovered();
    }
}
