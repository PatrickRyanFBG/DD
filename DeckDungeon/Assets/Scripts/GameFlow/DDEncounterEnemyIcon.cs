using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DDEncounterEnemyIcon : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private RawImage hoverImage; 
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    
    private DDEnemyOnBoard currentEnemy;
    public DDEnemyOnBoard CurrentEnemy => currentEnemy;
    
    public void Setup(DDEnemyOnBoard enemy)
    {
        currentEnemy = enemy;
        image.texture = currentEnemy.CurrentEnemy.Image;
        nameText.text = currentEnemy.CurrentEnemy.EntityName;

        currentEnemy.MatchingIcon = this;
    }

    public void Hovered()
    {
        currentEnemy.Hovered(true);
        PersonalHover();
    }

    public void PersonalHover()
    {
        hoverImage.enabled = true;
        nameText.enabled = true;
    }

    public void Unhovered()
    {
        if(currentEnemy)
        {
            currentEnemy.Unhovered(true);
        }
        PersonalUnhover();
    }

    public void PersonalUnhover()
    {
        hoverImage.enabled = false;
        nameText.enabled = false;
    }

    private void OnDisable()
    {
        Unhovered();
        currentEnemy.MatchingIcon = null;
    }
}
