using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCSCardBase : DDCardBase
{
    public int basicDefense;

    public virtual int GetDefenseNumber()
    {
        return basicDefense;
    }

    public IEnumerator ExecuteCard(bool offense)
    {
        yield return null;
    }

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return null;
    }

    public override void DisplayInformation(DDCardInHand cardInHand)
    {
        cardInHand.Image.texture = image;
        cardInHand.NameText.text = name;
        cardInHand.DescText.text = description;

        cardInHand.DefenseNumber.text = basicDefense.ToString();
    }
}
