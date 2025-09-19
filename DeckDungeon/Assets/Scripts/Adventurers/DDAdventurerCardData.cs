using System.Collections;
using System.Collections.Immutable;
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

    private Dictionary<string, DDCardBase> allCards = new Dictionary<string, DDCardBase>();
    // Make this read only or immutable?
    public Dictionary<string, DDCardBase> AllCards => allCards;
    
    private Dictionary<ECardType, List<DDCardBase>> cardsByType = new Dictionary<ECardType, List<DDCardBase>>();

    // Probablyt just a big list where we do a linq "Where" so we can look for specifs like "Action" + "Common"

    public void Init()
    {
        AddListToAllCards(starterCards, false);
        AddListToAllCards(commonCards, true);
        AddListToAllCards(uncommonCards, true);
        AddListToAllCards(rareCards, true);
    }

    private void AddListToAllCards(List<DDCardBase> cards, bool lootable)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (allCards.ContainsKey(cards[i].name))
            {
                Debug.Log(allCards[cards[i].name].name + " : " + cards[i].name);
            }

            allCards.Add(cards[i].GUID, cards[i]);

            if (!lootable) continue;
            
            if (cardsByType.TryGetValue(cards[i].CardType, out List<DDCardBase> list))
            {
                list.Add(cards[i]);
            }
            else
            {
                cardsByType.Add(cards[i].CardType, new List<DDCardBase>() { cards[i] });
            }
        }
    }

    public DDCardBase GetCardByGuid(string guid)
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
            card.Value.AddRandomFinishByImpact(EPlayerCardFinishImpact.Positive);
        }

        return new List<DDCardBase>(cards.Values);
    }

    public List<DDCardBase> GenerateCards(ref List<ECardType?> requestedTypes)
    {
        Dictionary<string, DDCardBase> cards = new(requestedTypes.Count);

        for (int i = 0; i < requestedTypes.Count; i++)
        {
            bool found = false;
            if (requestedTypes[i].HasValue)
            {
                if (cardsByType.TryGetValue(requestedTypes[i].Value, out List<DDCardBase> byType))
                {
                    DDCardBase typedCard = byType.GetRandomElement();

                    while (cards.ContainsKey(typedCard.GUID))
                    {
                        typedCard = byType.GetRandomElement();
                    }

                    cards.Add(typedCard.GUID, typedCard.Clone(true));
                    found = true;
                }
            }

            if (!requestedTypes[i].HasValue || !found)
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
        }

        foreach (KeyValuePair<string, DDCardBase> card in cards)
        {
            card.Value.AddRandomFinishByImpact(EPlayerCardFinishImpact.Positive);
        }

        return new List<DDCardBase>(cards.Values);
    }
}