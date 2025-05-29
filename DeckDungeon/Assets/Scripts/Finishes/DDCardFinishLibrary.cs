using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DDCardFinishLibrary : DDScriptableObject
{
    [SerializeReferenceDropdown, SerializeReference]
    private List<DDPlayerCardFinish> playerCardFishes = new();
    
    // Goes from Finish Enum to FinishImpact Enum
    [System.NonSerialized] Dictionary<EPlayerCardFinish, EPlayerCardFinishImpact> playerCardFinishDictionary = null;

    // FinishImpact > Finish > Finish Object
    [System.NonSerialized] private Dictionary<EPlayerCardFinishImpact, Dictionary<EPlayerCardFinish, DDPlayerCardFinish>>
        playerCardFinishDictionaryByType = null;

    public DDPlayerCardFinish GetFinishByType(EPlayerCardFinish targetFinish)
    {
        if (playerCardFinishDictionary == null)
        {
            Init();
        }

        DDPlayerCardFinish outFinish = null;

        if (playerCardFinishDictionary.TryGetValue(targetFinish, out EPlayerCardFinishImpact outFinishType))
        {
            playerCardFinishDictionaryByType[outFinishType].TryGetValue(targetFinish, out outFinish);
        }

        return outFinish;
    }

    public DDPlayerCardFinish GetRandomFinishByImpact(EPlayerCardFinishImpact finishImpact)
    {
        if (playerCardFinishDictionary == null)
        {
            Init();
        }
        
        float totalWeights = 0;

        foreach (var finish in playerCardFinishDictionaryByType[finishImpact])
        {
            totalWeights += finish.Value.Weight;
        }
        
        float randValue = UnityEngine.Random.Range(0, totalWeights);

        foreach (var finish in playerCardFinishDictionaryByType[finishImpact])
        {
            randValue -= finish.Value.Weight;
            if (randValue <= 0)
            {
                return finish.Value;
            }
        }
        
        /*
        outFinish = playerCardFinishDictionaryByType[finishImpact]
            .ElementAt(UnityEngine.Random.Range(0, playerCardFinishDictionaryByType[finishImpact].Count)).Value;
        */

        throw new Exception("The weights were fucked up.");
    }

    public float GetTotalWeightsByImpact(EPlayerCardFinishImpact finishImpact)
    {
        if (playerCardFinishDictionary == null)
        {
            Init();
        }
        
        float totalWeights = 0;

        foreach (var finish in playerCardFinishDictionaryByType[finishImpact])
        {
            totalWeights += finish.Value.Weight;
        }

        return totalWeights;
    }

    private void Init()
    {
        playerCardFinishDictionary = new(playerCardFishes.Count);
        playerCardFinishDictionaryByType = new(4);

        int numberOfFinishTypes = Enum.GetValues(typeof(EPlayerCardFinishImpact)).Cast<int>().Max();
        for (int i = 0; i <= numberOfFinishTypes; ++i)
        {
            playerCardFinishDictionaryByType[(EPlayerCardFinishImpact)i] = new(32);
        }

        foreach (DDPlayerCardFinish finish in playerCardFishes)
        {
            playerCardFinishDictionary.Add(finish.PlayerCardFinish, finish.PlayerCardFinishImpact);
            playerCardFinishDictionaryByType[finish.PlayerCardFinishImpact].Add(finish.PlayerCardFinish, finish);
        }
    }
}