using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDCardInHand : DDSelection
{
    [SerializeField] private float hoverTime = 1;
    private bool canHover = true;

    [SerializeField] private float selectedTime = 1;

    [SerializeField] private RawImage image;
    public RawImage Image => image;

    [SerializeField] private TMPro.TextMeshProUGUI cardTypeText;
    public TextMeshProUGUI CardTypeText => cardTypeText;

    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText => nameText;

    [SerializeField] private TMPro.TextMeshProUGUI descText;
    public TextMeshProUGUI DescText => descText;

    [SerializeField] private TMPro.TextMeshProUGUI momentumNumber;
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

    public void SetUpCard(DDCardBase cardBase, bool hover = true)
    {
        currentCard = cardBase;
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

    public List<ETargetType> GetCardTarget()
    {
        return currentCard.GetTargets();
    }

    public bool IsSelectionValid(DDSelection selection, int targetIndex)
    {
        return currentCard.IsSelectionValid(selection, targetIndex);
    }

    public override bool Hovered()
    {
        if (selected)
        {
            return false;
        }

        if (moveDownCoroutine != null)
        {
            StopCoroutine(moveDownCoroutine);
            moveDownCoroutine = null;
        }

        moveUpCoroutine = StartCoroutine(MoveUp());
        return true;
    }

    public override void Unhovered()
    {
        if (selected)
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
        return currentCard.ExecuteCard(selections);
    }

    private IEnumerator MoveUp()
    {
        while (deselectedCoroutine != null)
        {
            yield return null;
        }

        Vector3 pos = transform.localPosition;
        float time = (pos.z / 2f) * hoverTime;

        while (time < hoverTime)
        {
            time += Time.deltaTime;

            pos.z = Mathf.Lerp(0f, 2f, time / hoverTime);
            transform.localPosition = pos;

            yield return null;
        }

        moveUpCoroutine = null;
    }

    private IEnumerator MoveDown()
    {
        Vector3 pos = transform.localPosition;
        float time = (1 - (pos.z / 2f)) * hoverTime;

        while (time < hoverTime)
        {
            time += Time.deltaTime;

            pos.z = Mathf.Lerp(2f, 0f, time / hoverTime);
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
        prevLocation.z = 0;

        selected = true;
        selectedCoroutine = StartCoroutine(CardSelectedOverTime(location));

        return true;
    }

    private IEnumerator CardSelectedOverTime(Vector3 location)
    {
        Vector3 pos = transform.localPosition;
        float time = 0; //(1 - (pos.z / 2f)) * selectedTime;

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
        float time = 0; //(1 - (pos.z / 2f)) * selectedTime;

        while (time < selectedTime)
        {
            time += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(pos, prevLocation, time / selectedTime);

            yield return null;
        }

        deselectedCoroutine = null;
    }
}