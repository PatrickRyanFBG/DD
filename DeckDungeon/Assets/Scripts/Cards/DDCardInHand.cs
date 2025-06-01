using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDCardInHand : DDSelection
{
    [SerializeField] private float hoverDistance = 160f;

    [SerializeField] private float hoverTime = 1;
    private bool canHover = true;

    [SerializeField] private float selectedTime = 1;

    [SerializeField] private RawImage image;
    public RawImage Image => image;

    [SerializeField] private TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText => nameText;

    [SerializeField] private TextMeshProUGUI descText;
    public TextMeshProUGUI DescText => descText;

    [SerializeField] private TextMeshProUGUI typesText;
    public TextMeshProUGUI TypesText => typesText;

    [SerializeField] private TextMeshProUGUI momentumNumber;
    public TextMeshProUGUI MomentumNumber => momentumNumber;

    [SerializeField] protected Transform finishParent;
    [SerializeField] protected DDCardFinishIcon iconPrefab;

    protected Dictionary<EPlayerCardFinish, DDCardFinishIcon> finishIcons =
        new Dictionary<EPlayerCardFinish, DDCardFinishIcon>();

    protected DDCardBase currentCard;
    public DDCardBase CurrentCard => currentCard;

    private Coroutine moveUpCoroutine;
    private Coroutine moveDownCoroutine;

    private Coroutine selectedCoroutine;
    private Coroutine deselectedCoroutine;

    private bool selected = false;
    private Vector3 prevLocation;

    public virtual void SetUpCard(DDCardBase cardBase, bool hover = true)
    {
        currentCard = cardBase;

        transform.name = "CIH: " + currentCard.CardName;

        UpdateDisplayInformation();

        canHover = hover;

        gameObject.SetActive(true);
    }

    public void SetCanHover(bool hover)
    {
        canHover = hover;

        if (!canHover && moveUpCoroutine != null)
        {
            StopCoroutine(moveUpCoroutine);
            Vector3 localPos = transform.localPosition;
            localPos.y = 0;
            transform.localPosition = localPos;
        }
    }

    public void UpdateDisplayInformation()
    {
        currentCard.SetCardInHand(this);
        foreach (var finish in currentCard.AllCardFinishes)
        {
            if (!finishIcons.ContainsKey(finish.Key))
            {
                DDCardFinishIcon icon = Instantiate(iconPrefab, finishParent);
                icon.SetUp(finish.Value);
                icon.gameObject.SetActive(true);
                finishIcons[finish.Key] = icon;
            }
        }
    }

    protected virtual void OnDisable() { }

    public List<DDCardTargetInfo> GetCardTarget()
    {
        return currentCard.GetTargets();
    }

    public bool IsSelectionValid(List<DDSelection> selections, DDSelection selection, int targetIndex)
    {
        // This only happens if by some reason a card is selecting itself
        if (selection == this)
        {
            return false;
        }
        
        return currentCard.IsSelectionValid(selections, selection, targetIndex);
    }

    public bool ShouldExecuteEarly(List<DDSelection> selections)
    {
        return currentCard.ShouldExecuteEarly(selections);
    }

    // Called from UI
    public virtual void UI_Clicked()
    {
        // Put some check here?
        // Also don't like how we are sort of jacking this callback
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected?.Invoke(this);
    }

    // Called from UI
    public override void Hovered(bool fromAnotherSelection = false)
    {
        if (selected || !canHover)
        {
            return;
        }

        if (moveDownCoroutine != null)
        {
            StopCoroutine(moveDownCoroutine);
            moveDownCoroutine = null;
        }

        DDGlobalManager.Instance.ClipLibrary.HoverCard.PlayNow();

        moveUpCoroutine = StartCoroutine(MoveUp());
    }

    // Called from UI
    public override void Unhovered(bool fromAnotherSelection = false)
    {
        if (selected || !canHover)
        {
            return;
        }

        if (moveUpCoroutine != null)
        {
            StopCoroutine(moveUpCoroutine);
            moveUpCoroutine = null;
        }

        moveDownCoroutine = StartCoroutine(MoveDown());
    }

    public IEnumerator DrawCard()
    {
        yield return currentCard.DrawCard();

        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged?.Invoke(this, EPlayerCardLifeTime.Drawn);
    }

    public IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        selected = false;
        yield return currentCard.ExecuteCard(selections);
    }

    public IEnumerator ExecuteFinishes(EPlayerCardLifeTime lifeTime)
    {
        yield return currentCard.ExecuteFinishes(lifeTime);
    }

    public IEnumerator EndOfTurn ()
    {
        yield return currentCard.EndOfTurn();
    }

    public IEnumerator DiscardCard(bool endOfTurn)
    {
        yield return currentCard.DiscardCard(endOfTurn);
        
        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged?.Invoke(this, EPlayerCardLifeTime.Discarded);
    }

    private IEnumerator MoveUp()
    {
        // Maybe add canHover in here?
        while (deselectedCoroutine != null)
        {
            yield return null;
        }

        Vector3 pos = transform.localPosition;
        float time = (pos.y / hoverDistance) * hoverTime;

        while (time < hoverTime)
        {
            time += Time.deltaTime;

            pos.y = Mathf.Lerp(0f, hoverDistance, time / hoverTime);
            transform.localPosition = pos;

            yield return null;
        }

        moveUpCoroutine = null;
    }

    private IEnumerator MoveDown()
    {
        Vector3 pos = transform.localPosition;
        float time = (1 - (pos.y / hoverDistance)) * hoverTime;

        while (time < hoverTime)
        {
            time += Time.deltaTime;

            pos.y = Mathf.Lerp(hoverDistance, 0f, time / hoverTime);
            transform.localPosition = pos;

            yield return null;
        }

        moveDownCoroutine = null;
    }

    public bool CardSelected(Vector3 location)
    {
        if (!currentCard.SelectCard())
        {
            return false;
        }

        if (moveUpCoroutine != null)
        {
            StopCoroutine(moveUpCoroutine);
            moveUpCoroutine = null;
        }

        if (moveDownCoroutine != null)
        {
            StopCoroutine(moveDownCoroutine);
            moveDownCoroutine = null;
        }

        DDGlobalManager.Instance.ClipLibrary.SelectCard.PlayNow();

        prevLocation = transform.localPosition;
        prevLocation.y = 0;

        selected = true;
        selectedCoroutine = StartCoroutine(CardSelectedOverTime(location));

        return true;
    }

    private IEnumerator CardSelectedOverTime(Vector3 location)
    {
        canHover = false;

        Vector3 pos = transform.position;
        float time = 0;

        while (time < selectedTime)
        {
            time += Time.deltaTime;

            transform.position = Vector3.Lerp(pos, location, time / selectedTime);

            yield return null;
        }

        selectedCoroutine = null;
    }

    public void CardDeselected()
    {
        if (selectedCoroutine != null)
        {
            StopCoroutine(selectedCoroutine);
        }

        selected = false;
        deselectedCoroutine = StartCoroutine(CardDeselectedOverTime());
    }

    private IEnumerator CardDeselectedOverTime()
    {
        Vector3 pos = transform.localPosition;
        float time = 0;

        while (time < selectedTime)
        {
            time += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(pos, prevLocation, time / selectedTime);

            yield return null;
        }

        transform.localPosition = prevLocation;

        deselectedCoroutine = null;

        canHover = true;
    }
}