using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public abstract class DDPlayerCardFinish
{
    public abstract EPlayerCardFinish PlayerCardFinish { get; }
    public abstract ECardExecutionTime CardExecutionTime { get; }

    [SerializeField, Multiline] private string description;
    
    // Need to add something about effect or layer or something here.
    [SerializeField] private Material material;
    
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
    public override ECardExecutionTime CardExecutionTime => ECardExecutionTime.Drawn;

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
    public override ECardExecutionTime CardExecutionTime => ECardExecutionTime.Drawn;

    public override IEnumerator ExecuteFinish(DDCardBase card)
    {
        DDGamePlaySingletonHolder.Instance.Player.AddToMomentum(1);

        yield return new WaitForSeconds(0.1f);
    }
}

public class DDCardFinishCoarse : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Coarse;
    public override ECardExecutionTime CardExecutionTime => ECardExecutionTime.None;
}

public class DDCardFinishWeighty : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Weighty;
    public override ECardExecutionTime CardExecutionTime => ECardExecutionTime.None;
}

// Neutral
public class DDCardFinishSticky : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Sticky;
    public override ECardExecutionTime CardExecutionTime => ECardExecutionTime.None;
}

public class DDCardFinishFleeting : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Fleeting;
    public override ECardExecutionTime CardExecutionTime => ECardExecutionTime.Discarded;
    
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
    public override ECardExecutionTime CardExecutionTime => ECardExecutionTime.None;
}

public class DDCardFinishSiphon : DDPlayerCardFinish
{
    public override EPlayerCardFinish PlayerCardFinish => EPlayerCardFinish.Siphon;
    public override ECardExecutionTime CardExecutionTime => ECardExecutionTime.None;
}