using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adventurer specific cards in the Adventure Data right now
// Maybe this class should hold generic cards if that is ever a thing
public class DDCardLibrary : MonoBehaviour
{
    // something something we need different rarities some where
    [SerializeField]
    private List<DDCardBase> valkyrieCards = new List<DDCardBase>();

    public List<DDCardBase> GenerateValkyrieCards(int amount)
    {
        List<DDCardBase> cards = new List<DDCardBase>(amount);

        for (int i = 0; i < amount; i++)
        {
            int randNum = Random.Range(0, valkyrieCards.Count);

            while (cards.Contains(valkyrieCards[randNum]))
            {
                randNum = Random.Range(0, valkyrieCards.Count);
            }

            cards.Add(valkyrieCards[randNum]);
        }

        return cards;
    }
}
