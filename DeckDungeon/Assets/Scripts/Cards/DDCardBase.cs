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

    [System.NonSerialized] protected Dictionary<EPlayerCardFinish, DDPlayerCardFinish> allCardFinishes;
    [System.NonSerialized] protected Dictionary<ECardExecutionTime, List<DDPlayerCardFinish>> cardExecutionActions;

    public virtual void RuntimeInit(DDCardInHand inHand)
    {
        cardInHand = inHand;

        allCardFinishes = new();
        cardExecutionActions = new();

        for (int i = 0; i < defaultCardFinishes.Length; i++)
        {
            AddCardFinishByType(defaultCardFinishes[i]);
        }

        CardInHand.Image.texture = image;
        CardInHand.CardTypeText.text = cardType.ToString();
        CardInHand.NameText.text = CardName;
        CardInHand.DescText.text = description;
    }

    public virtual bool AddCardFinishByType(EPlayerCardFinish finishType)
    {
        // Cards can only have 1 type of finish
        if (allCardFinishes.ContainsKey(finishType))
        {
            return false;
        }
        
        DDPlayerCardFinish finish =
            DDGlobalManager.Instance.CardFinishLibrary.GetFinishByType(finishType);
        allCardFinishes.Add(finishType, finish);

        if (cardExecutionActions.TryGetValue(finish.CardExecutionTime, out List<DDPlayerCardFinish> finishes))
        {
            finishes.Add(finish);
        }
        
        return true;
    }

    private IEnumerator ExecuteFinishes(ECardExecutionTime executionTime)
    {
        if (cardExecutionActions.TryGetValue(executionTime, out List<DDPlayerCardFinish> finishes))
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
        yield return ExecuteFinishes(ECardExecutionTime.Drawn);
        
        yield return null;
    }

    public IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return PreExecute(selections);

        yield return Execute(selections);

        yield return ExecuteFinishes(ECardExecutionTime.Played);
        
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
        yield return ExecuteFinishes(ECardExecutionTime.EndOfRound);
        
        yield return null;
    }
    
    public virtual IEnumerator DiscardCard(bool endOfTurn)
    {
        yield return ExecuteFinishes(ECardExecutionTime.Discarded);
        
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

        if (string.IsNullOrWhiteSpace(cardName))
        {
#if UNITY_EDITOR
            cardName = name;
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