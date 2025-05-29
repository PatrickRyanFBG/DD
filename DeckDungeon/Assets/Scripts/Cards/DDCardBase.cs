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

    [SerializeField] protected ERangeType rangeType = ERangeType.Pure;
    public ERangeType RangeType => rangeType;

    [SerializeField] EPlayerCardFinish[] defaultCardFinishes;

    [SerializeField] private Vector2 price = new Vector2(100, 200);
    public int Price => (int)Random.Range(price.x, price.y);

    protected List<DDCardTargetInfo> targets = null;

    // Run-time
    [System.NonSerialized] protected DDCardInHand cardInHand;
    public DDCardInHand CardInHand => cardInHand;
    
    [System.NonSerialized] protected bool doneInit = false;
    
    public Dictionary<EPlayerCardFinish, DDPlayerCardFinish> AllCardFinishes { get; private set; }
    [System.NonSerialized] protected Dictionary<EPlayerCardLifeTime, List<DDPlayerCardFinish>> cardExecutionActions;

    public DDCardBase Clone(bool withInit)
    {
        DDCardBase clone = Instantiate(this);
        clone.name = name;

        if (withInit || !doneInit)
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
            AddCardFinish(defaultCardFinishes[i]);
        }
        
        doneInit = true;
    }

    public virtual void SetCardInHand(DDCardInHand inHand)
    {
        cardInHand = inHand;

        cardInHand.Image.texture = image;
        cardInHand.NameText.text = cardName;
        cardInHand.DescText.text = description;
        cardInHand.TypesText.text = cardType.ToString();
        if (rangeType != ERangeType.None)
        {
            cardInHand.TypesText.text += " | " + rangeType.ToString();
        }
    }

    public virtual void RemoveCardFinish(EPlayerCardFinish eFinish)
    {
        if (AllCardFinishes.TryGetValue(eFinish, out var finish))
        {
            foreach (EPlayerCardLifeTime lifeTime in finish.PlayerCardLifeTimes)
            {
                if (cardExecutionActions.TryGetValue(lifeTime, out List<DDPlayerCardFinish> finishes))
                {
                    finishes.Remove(finish);
                }
            }
            
            AllCardFinishes.Remove(eFinish);
        }
    }
    
    public virtual void AddRandomFinishByImpact(EPlayerCardFinishImpact finishImpact)
    {
        // Attempt finite number of times. Right now finishes can't stack.
        for (int i = 1; i < 5; i++)
        {
            DDPlayerCardFinish finish = DDGlobalManager.Instance.CardFinishLibrary.GetRandomFinishByImpact(finishImpact);
            if (AddCardFinish(finish))
            {
                return;
            }
        }
    }

    // Some cards might not be able to have certain finishes
    public virtual bool AddCardFinish(EPlayerCardFinish finishType)
    {
        DDPlayerCardFinish finish =
            DDGlobalManager.Instance.CardFinishLibrary.GetFinishByType(finishType);

        return AddCardFinish(finish);
    }

    public virtual bool AddCardFinish(DDPlayerCardFinish finish)
    {
        // Cards can only have 1 type of finish
        if (AllCardFinishes.ContainsKey(finish.PlayerCardFinish))
        {
            return false;
        }
        
        AllCardFinishes.Add(finish.PlayerCardFinish, finish);


        foreach (EPlayerCardLifeTime lifeTime in finish.PlayerCardLifeTimes)
        {
            if (cardExecutionActions.TryGetValue(lifeTime, out List<DDPlayerCardFinish> finishes))
            {
                bool added = false;
                for (int i = 0; i < finishes.Count; i++)
                {
                    if (finish.PlayerCardFinishPriority < finishes[i].PlayerCardFinishPriority)
                    {
                        finishes.Insert(i, finish);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    finishes.Add(finish);
                }
            }
            else
            {
                cardExecutionActions.Add(lifeTime, new() { finish });
            }
        }

        return true;
    }

    public IEnumerator ExecuteFinishes(EPlayerCardLifeTime lifeTime)
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

    public virtual bool IsSelectionValid(List<DDSelection> selections, DDSelection selection, int targetIndex)
    {
        return true;
    }

    public virtual bool ShouldExecuteEarly(List<DDSelection> selections)
    {
        return false;
    }
    
    public virtual List<DDCardTargetInfo> GetTargets()
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