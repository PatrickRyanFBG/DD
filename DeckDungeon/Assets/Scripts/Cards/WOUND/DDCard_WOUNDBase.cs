using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DDCard_WOUNDBase : DDCardBase
{
    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return null;
    }

    public override bool SelectCard()
    {
        return false;
    }
}
