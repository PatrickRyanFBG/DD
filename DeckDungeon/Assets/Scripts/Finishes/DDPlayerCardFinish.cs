using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public abstract class DDPlayerCardFinish
{
    public abstract EPlayerCardFinish PlayerCardFinish { get; }
    public virtual EPlayerCardLifeTime[] PlayerCardLifeTimes => new EPlayerCardLifeTime[]{};
    public virtual EPlayerCardFinishPriority PlayerCardFinishPriority => EPlayerCardFinishPriority.None;

    [FormerlySerializedAs("playerCardFinishType")] [SerializeField] private EPlayerCardFinishImpact playerCardFinishImpact;
    public EPlayerCardFinishImpact PlayerCardFinishImpact => playerCardFinishImpact;
    
    [SerializeField] private float weight;
    public float Weight => weight;
    
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
    public override EPlayerCardLifeTime[] PlayerCardLifeTimes => new[]{ EPlayerCardLifeTime.Drawn };

    [SerializeField] private int numberOfBleeds = 2;
    [SerializeField] private int bleedAmount = 2;

    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        List<DDEnemyOnBoard> enemies = DDGamePlaySingletonHolder.Instance.Encounter.AllEnemies.GetRandomElements(numberOfBleeds);

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
    public override EPlayerCardLifeTime[] PlayerCardLifeTimes => new[]{ EPlayerCardLifeTime.Drawn };

    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        yield return DDGamePlaySingletonHolder.Instance.Player.AddToMomentum(1);

        yield return new WaitForSeconds(0.1f);
    }
}

public class DDCardFinishSharp : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Sharp;
}

public class DDCardFinishWeighty : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Weighty;
}

public class DDCardFinishExplosive : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Explosive;
    public override EPlayerCardLifeTime[] PlayerCardLifeTimes => new[]{ EPlayerCardLifeTime.Discarded };

    [SerializeField] private int damage;
    
    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        DDEnemyOnBoard randomEnemy = DDGamePlaySingletonHolder.Instance.Encounter.AllEnemies.GetRandomElement();
        
        // ANIMATE SHIT HERE
        DDGamePlaySingletonHolder.Instance.Player.DealDamageToEnemy(damage, ERangeType.Pure, randomEnemy, false);
        
        yield return new WaitForSeconds(0.1f);
    }
}

public class DDCardFinishReplicating : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Replicating;
    public override EPlayerCardLifeTime[] PlayerCardLifeTimes => new[]{ EPlayerCardLifeTime.PostPlayed };

    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        DDCardBase duplicated = card.Clone(false);
        duplicated.RemoveCardFinish(EPlayerCardFinish.Replicating);
        duplicated.AddCardFinish(EPlayerCardFinish.Fleeting);
        
        // ANIMATE SHIT HERE
        // Maybe add card in the same index as this was?
        yield return DDGamePlaySingletonHolder.Instance.Player.AddCardTo(duplicated, null, ECardLocation.Hand, true);
        
        yield return new WaitForSeconds(0.1f);
    }
}

// Neutral
public class DDCardFinishSticky : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Sticky;
}

public class DDCardFinishFleeting : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Fleeting;
    public override EPlayerCardLifeTime[] PlayerCardLifeTimes => new[]{ EPlayerCardLifeTime.Discarded , EPlayerCardLifeTime.PostPlayed };
    public override EPlayerCardFinishPriority PlayerCardFinishPriority => EPlayerCardFinishPriority.Last;

    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        // Animate fading out shit
        GameObject.Destroy(card.CardInHand.gameObject);
        
        yield return new WaitForSeconds(0.1f);
    }
}

// Negative
public class DDCardFinishFragile : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Fragile;
}

public class DDCardFinishSiphon : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Siphon;
}