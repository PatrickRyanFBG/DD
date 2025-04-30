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
    }

    public void UpdateDisplayInformation()
    {
        currentCard.SetCardInHand(this);
        foreach (var finish in currentCard.AllCardFinishes)
        {
            DDCardFinishIcon icon = Instantiate(iconPrefab, finishParent);
            icon.SetUp(finish.Value);
            icon.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        // pool these
        for (int i = 1; i < finishParent.childCount; i++)
        {
            Destroy(finishParent.GetChild(i).gameObject);
        }
    }

    public List<ETargetType> GetCardTarget()
    {
        return currentCard.GetTargets();
    }

    public bool IsSelectionValid(DDSelection selection, int targetIndex)
    {
        return currentCard.IsSelectionValid(selection, targetIndex);
    }

    // Called from UI
    public virtual void UI_Clicked()
    {
        // Put some check here?
        // Also don't like how we are sort of jacking this callback
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected?.Invoke(this);
    }
    
    // Called from UI
    public override void Hovered()
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

        moveUpCoroutine = StartCoroutine(MoveUp());
    }

    // Called from UI
    public override void Unhovered()
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

    public IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        selected = false;
        yield return currentCard.ExecuteCard(selections);
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

        prevLocation = transform.localPosition;
        prevLocation.y = 0;

        selected = true;
        selectedCoroutine = StartCoroutine(CardSelectedOverTime(location));

        return true;
    }

    private IEnumerator CardSelectedOverTime(Vector3 location)
    {
        canHover = false;
        
        Vector3 pos = transform.localPosition;
        float time = 0;

        while (time < selectedTime)
        {
            time += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(pos, location, time / selectedTime);

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