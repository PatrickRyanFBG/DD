using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_VALK_QuickThinking : DDCard_VALKBase
{
    [Header("Quick Thinking")]
    [SerializeField]
    private int drawAmount;

    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        yield return base.Execute(selections);

        for (int i = 0; i < drawAmount; i++)
        {
            DDGamePlaySingletonHolder.Instance.Player.DrawACard();
            yield return new WaitForSeconds(.25f);
        }
    }
}
