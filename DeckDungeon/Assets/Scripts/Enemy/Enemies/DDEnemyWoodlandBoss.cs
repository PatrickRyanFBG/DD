using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDEnemyWoodlandBoss : DDEnemyBase
{
    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        // If enemy is shown,
        // If 3x3 foliage < 4
            // Fill 3x3
            // Hide
        // else
            // Attack
            // Hide
        // if hidden
            // Reveal (and something?)
            // Attack
        throw new System.NotImplementedException();
    }
}

public class DDEnemyActionHideInEnemy : DDEnemyActionBase
{
    
    
    public override Texture GetIcon()
    {
        throw new System.NotImplementedException();
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        throw new System.NotImplementedException();
    }

    public override string GetDescription()
    {
        throw new System.NotImplementedException();
    }
}
