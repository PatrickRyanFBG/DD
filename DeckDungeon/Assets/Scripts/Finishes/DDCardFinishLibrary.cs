using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCardFinishLibrary : DDScriptableObject
{
    [SerializeReferenceDropdown, SerializeReference]
    private List<DDPlayerCardFinish> playerCardFishes = new();

    [System.NonSerialized] Dictionary<EPlayerCardFinish, DDPlayerCardFinish> playerCardFinishDictionary = null;

    public DDPlayerCardFinish GetFinishByType(EPlayerCardFinish targetFinish)
    {
        if (playerCardFinishDictionary == null)
        {
            playerCardFinishDictionary = new (playerCardFishes.Count);

            foreach (DDPlayerCardFinish finish in playerCardFishes)
            {
                playerCardFinishDictionary.Add(finish.PlayerCardFinish, finish);
            }
        }

        playerCardFinishDictionary.TryGetValue(targetFinish, out DDPlayerCardFinish outFinish);
        return outFinish;
    }
}