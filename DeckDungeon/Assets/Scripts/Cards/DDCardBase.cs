using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public abstract class DDCardBase : DDScriptableObject
{
    [field: SerializeField]
    [field: ReadOnly]
    public string GUID { get; private set; }

    [Header("Base")] [SerializeField] private ECardType cardType;
    public ECardType CardType => cardType;

    [SerializeField] private ECardRarity cardRarity;

    [SerializeField] private Texture image;
    public Texture Image => image;

    [SerializeField] private string cardName;
    public string CardName => cardName;

    [SerializeField, Multiline] private string description;

    [SerializeField] protected ERangeType rangeType = ERangeType.None;
    public ERangeType RangeType => rangeType;

    [SerializeField] EPlayerCardFinish[] defaultCardFinishes;

    [SerializeField] private Vector2 price = new Vector2(100, 200);
    public int Price => (int)Random.Range(price.x, price.y);

    protected List<ETargetType> targets = null;

    // Run-time
    [System.NonSerialized] protected DDCardInHand cardInHand;
    public DDCardInHand CardInHand => cardInHand;

    public Dictionary<EPlayerCardFinish, DDPlayerCardFinish> AllCardFinishes { get; private set; }
    [System.NonSerialized] protected Dictionary<EPlayerCardLifeTime, List<DDPlayerCardFinish>> cardExecutionActions;

    public DDCardBase Clone(bool withInit)
    {
        DDCardBase clone = UnityEngine.Object.Instantiate(this);
        clone.name = name;

        if (withInit)
        {
            clone.RuntimeInit();
        }
        else
        {
            clone.AllCardFinishes = new Dictionary<EPlayerCardFinish, DDPlayerCardFinish>(AllCardFinishes);
            clone.cardExecutionActions =
                new Dictionary<EPlayerCardLifeTime, List<DDPlayerCardFinish>>(cardExecutionActions);
        }

        return clone;
    }

    public virtual void RuntimeInit()
    {
        AllCardFinishes = new();
        cardExecutionActions = new();

        for (int i = 0; i < defaultCardFinishes.Length; i++)
        {
            AddCardFinishByType(defaultCardFinishes[i]);
        }
    }

    public virtual void SetCardInHand(DDCardInHand inHand)
    {
        cardInHand = inHand;

        cardInHand.Image.texture = image;
        cardInHand.CardTypeText.text = cardType.ToString();
        cardInHand.NameText.text = CardName;
        cardInHand.DescText.text = description;
    }

    public virtual void AddRandomFinish()
    {
        for (int i = 1; i <= (int)EPlayerCardFinish.Weighty; i++)
        {
            if (AddCardFinishByType((EPlayerCardFinish)Random.Range(1, (int)(EPlayerCardFinish.Weighty + 1))))
            {
                return;
            }
        }
    }

    public virtual bool AddCardFinishByType(EPlayerCardFinish finishType)
    {
        // Cards can only have 1 type of finish
        if (AllCardFinishes.ContainsKey(finishType))
        {
            return false;
        }

        DDPlayerCardFinish finish =
            DDGlobalManager.Instance.CardFinishLibrary.GetFinishByType(finishType);
        AllCardFinishes.Add(finishType, finish);

        if (cardExecutionActions.TryGetValue(finish.PlayerCardLifeTime, out List<DDPlayerCardFinish> finishes))
        {
            finishes.Add(finish);
        }
        else
        {
            cardExecutionActions.Add(finish.PlayerCardLifeTime, new() { finish });
        }

        return true;
    }

    private IEnumerator ExecuteFinishes(EPlayerCardLifeTime lifeTime)
    {
        if (cardExecutionActions.TryGetValue(lifeTime, out List<DDPlayerCardFinish> finishes))
        {
            foreach (var finish in finishes)
            {
                yield return finish.ExecuteFinish(this);
            }
        }
    }

    public virtual bool SelectCard()
    {
        return true;
    }

    public virtual void UnselectCard()
    {
    }

    public virtual IEnumerator DrawCard()
    {
        yield return ExecuteFinishes(EPlayerCardLifeTime.Drawn);

        yield return null;
    }

    public IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return PreExecute(selections);

        yield return Execute(selections);

        yield return ExecuteFinishes(EPlayerCardLifeTime.Played);

        yield return PostExecute(selections);
    }

    protected virtual IEnumerator PreExecute(List<DDSelection> selections)
    {
        yield return null;
    }

    protected abstract IEnumerator Execute(List<DDSelection> selections);

    protected virtual IEnumerator PostExecute(List<DDSelection> selections)
    {
        yield return null;
    }

    public virtual IEnumerator EndOfTurn()
    {
        yield return ExecuteFinishes(EPlayerCardLifeTime.EndOfRound);

        yield return null;
    }

    public virtual IEnumerator DiscardCard(bool endOfTurn)
    {
        yield return ExecuteFinishes(EPlayerCardLifeTime.Discarded);

        yield return null;
    }

    public virtual IEnumerator DestroyCard()
    {
        yield return null;
    }

    public virtual bool IsSelectionValid(DDSelection selection, int targetIndex)
    {
        return true;
    }

    public virtual List<ETargetType> GetTargets()
    {
        return targets;
    }

    #region GUID

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(GUID))
        {
            AssignNewGUID();
        }

        if (string.IsNullOrWhiteSpace(cardName) || cardName.Contains("DDCardValk"))
        {
#if UNITY_EDITOR
            cardName = name.Remove(0, 10);
            UnityEditor.EditorUtility.SetDirty(this);
#else
            cardName = "MISSING NAME";
#endif
        }
    }

    private void AssignNewGUID()
    {
#if UNITY_EDITOR
        GUID = UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(this));
        UnityEditor.EditorUtility.SetDirty(this);
#else
        GUID = Guid.NewGuid().ToString();
#endif
    }

    #endregion
}