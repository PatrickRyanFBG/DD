using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DDAdventurerCardData
{
    [SerializeField] private List<DDCardBase> starterCards;
    [SerializeField] private List<DDCardBase> commonCards;
    [SerializeField] private List<DDCardBase> uncommonCards;
    [SerializeField] private List<DDCardBase> rareCards;

    Dictionary<string, DDCardBase> allCards = new Dictionary<string, DDCardBase>();

    public void Init()
    {
        AddListToAllCards(starterCards);
        AddListToAllCards(commonCards);
        AddListToAllCards(uncommonCards);
        AddListToAllCards(rareCards);
    }

    private void AddListToAllCards(List<DDCardBase> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (allCards.ContainsKey(cards[i].name))
            {
                Debug.Log(allCards[cards[i].name].name + " : " + cards[i].name);
            }

            allCards.Add(cards[i].GUID, cards[i]);
        }
    }

    public DDCardBase GetCardByGUID(string guid)
    {
        if (allCards.TryGetValue(guid, out DDCardBase card))
        {
            return card;
        }

        return null;
    }

    public List<DDCardBase> GenerateCards(int amount)
    {
        Dictionary<string, DDCardBase> cards = new(amount);

        for (int i = 0; i < amount; i++)
        {
            int randNum = Random.Range(0, commonCards.Count);

            DDCardBase card = commonCards[randNum];

            while (cards.ContainsKey(card.GUID))
            {
                randNum = Random.Range(0, commonCards.Count);
                card = commonCards[randNum];
            }

            cards.Add(card.GUID, card.Clone(true));
        }

        foreach (KeyValuePair<string, DDCardBase> card in cards)
        {
            card.Value.AddRandomFinish();
        }

        return new List<DDCardBase>(cards.Values);
    }
}