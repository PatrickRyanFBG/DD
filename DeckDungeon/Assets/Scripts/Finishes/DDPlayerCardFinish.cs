using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public abstract class DDPlayerCardFinish
{
    public abstract EPlayerCardFinish PlayerCardFinish { get; }
    public abstract EPlayerCardLifeTime PlayerCardLifeTime { get; }
    public virtual EPlayerCardFinishPriority PlayerCardFinishPriority => EPlayerCardFinishPriority.None;

    [SerializeField, Multiline] private string description;
    
    // This needs to go from icon to shader/material for actual finish
    [SerializeField] private Texture icon;
    public Texture Icon => icon;
    
    public virtual IEnumerator ExecuteFinish(DDCardBase card)
    {
        yield return null;
    }

    public string GetDescription()
    {
        return description;
    }
}

// Positive
public class DDCardFinishSerrated : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Serrated;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.Drawn;

    [SerializeField] private int numberOfBleeds = 2;
    [SerializeField] private int bleedAmount = 2;

    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        List<DDEnemyOnBoard> enemies = new List<DDEnemyOnBoard>();
        DDGamePlaySingletonHolder.Instance.Board.GetAllEnemies(ref enemies);
        enemies.Shuffle();

        for (int i = 0; i < numberOfBleeds; i++)
        {
            if (i >= enemies.Count)
            {
                break;
            }
            
            enemies[i].ModifyAffix(EAffixType.Bleed, bleedAmount, false);
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}

public class DDCardFinishEnergized : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Energized;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.Drawn;

    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        DDGamePlaySingletonHolder.Instance.Player.AddToMomentum(1);

        yield return new WaitForSeconds(0.1f);
    }
}

public class DDCardFinishSharp : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Sharp;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.None;
}

public class DDCardFinishWeighty : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Weighty;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.None;
}

public class DDCardFinishSticky : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Sticky;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.None;
}

public class DDCardFinishExplosive : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Explosive;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.Discarded;

    [SerializeField] private int damage;
    
    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        List<DDEnemyOnBoard> allEnemies = new();
        DDGamePlaySingletonHolder.Instance.Board.GetAllEnemies(ref allEnemies);
        DDEnemyOnBoard randomEnemy = allEnemies.GetRandomElement();
        
        // ANIMATE SHIT HERE
        DDGamePlaySingletonHolder.Instance.Player.DealDamageToEnemy(damage, ERangeType.None, randomEnemy, false);
        
        yield return new WaitForSeconds(0.1f);
    }
}

// Neutral
public class DDCardFinishFleeting : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Fleeting;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.Discarded;
    public override EPlayerCardFinishPriority PlayerCardFinishPriority => EPlayerCardFinishPriority.Last;

    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        // Animate discard shit
        GameObject.Destroy(card.CardInHand.gameObject);
        
        yield return new WaitForSeconds(0.1f);
    }
}

// Negative
public class DDCardFinishFragile : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Fragile;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.None;
}

public class DDCardFinishSiphon : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Siphon;
    public override EPlayerCardLifeTime PlayerCardLifeTime => EPlayerCardLifeTime.None;
}