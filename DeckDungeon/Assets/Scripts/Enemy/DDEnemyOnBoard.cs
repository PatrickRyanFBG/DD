using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DDEnemyOnBoard : DDSelection
{
    private int maxHealth;
    private int currentHealth;

    private DDEnemyBase currentEnemy;
    public DDEnemyBase CurrentEnemy => currentEnemy;

    private DDAffixManager affixManager;

    private int turnNumber;
    public int TurnNumber => turnNumber;

    [Header("Testing")]
    [SerializeField]
    private TMPro.TextMeshProUGUI healthUI;

    [SerializeField]
    private TMPro.TextMeshProUGUI turnUI;

    [SerializeField]
    private RawImage image;

    [SerializeField]
    private RawImage actionOneImage;
    [SerializeField]
    private TMPro.TextMeshProUGUI actionOneText;

    [SerializeField]
    private RawImage actionTwoImage;
    [SerializeField]
    private TMPro.TextMeshProUGUI actionTwoText;

    [SerializeField]
    private Renderer hoveredRenderer;

    private List<DDEnemyActionBase> nextActions = new List<DDEnemyActionBase>();

    private DDLocation currentLocation;
    public DDLocation CurrentLocaton => currentLocation;

    [SerializeField]
    private GameObject attackPrefab;
    public GameObject AttackPrefab => attackPrefab;

    [SerializeField]
    private RawImage dexIcon;
    [SerializeField]
    private TMPro.TextMeshProUGUI dexText;

    [SerializeField]
    private RawImage armorIcon;
    [SerializeField]
    private TMPro.TextMeshProUGUI armorText;

    public void SetUpEnemy(DDEnemyBase enemyBase)
    {
        currentEnemy = enemyBase;
        image.texture = enemyBase.Image;
        maxHealth = enemyBase.StartingHealth;
        currentHealth = maxHealth;
        UpdateHealthUI();
        affixManager = new DDAffixManager();
        affixManager.AffixAdjusted.AddListener(AffixAdjusted);
        affixManager.ModifyValueOfAffix(EAffixType.Armor, enemyBase.StartingArmor, true);
    }

    private void AffixAdjusted(EAffixType changedAffix)
    {
        switch (changedAffix)
        {
            case EAffixType.Expertise:
                UpdateDexterityUI();
                break;
            case EAffixType.Armor:
                UpdateArmorUI();
                break;
        }
    }

    private void OnDisable()
    {
        Unhovered();
        NonActionableUnhover();
    }

    public void SnapLocation(DDLocation location)
    {
        currentLocation = location;
        if (currentLocation != null)
        {
            transform.parent = currentLocation.transform;
            transform.localPosition = Vector3.zero;
        }
    }

    public IEnumerator SetLocation(DDLocation location)
    {
        currentLocation = location;
        if (currentLocation != null)
        {
            transform.parent = currentLocation.transform;
            yield return StartCoroutine(MoveToLocation());
        }
    }

    public IEnumerator MoveToLocation()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = CurrentLocaton.transform.position;

        //targetPos.y = startPos.y;

        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            yield return null;
        }
    }

    public IEnumerator MoveToLocationAndBack(Vector3 pos)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = Vector3.Lerp(startPos, pos, .75f);

        float time = 0;
        while (time < .5f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / .5f);
            yield return null;
        }

        time = 0;
        while (time < .5f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(targetPos, startPos, time / .5f);
            yield return null;
        }
    }

    public void DoDamage(int amount, ERangeType rangeType = ERangeType.None)
    {
        int totalDamage = amount + DDGamePlaySingletonHolder.Instance.Board.GetMeleeRangedBonus(rangeType, currentLocation.Coord.y);
        int reducedArmorAmount = affixManager.ModifyValueOfAffix(EAffixType.Armor, -totalDamage, false);

        if (reducedArmorAmount <= 0)
        {
            amount = reducedArmorAmount * -1;

            if (amount > 0)
            {
                currentHealth -= amount;

                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    DDGamePlaySingletonHolder.Instance.Encounter.EnemyDefeated(this);
                    Destroy(gameObject);
                }
                else
                {
                    // Take Damage Feedback.
                }
                UpdateHealthUI();
            }
        }
    }

    public void ModifyAffix(EAffixType affixType, int amount, bool shouldSet)
    {
        affixManager.ModifyValueOfAffix(affixType, amount, shouldSet);
    }

    public int GetAffixValue(EAffixType affixType)
    {
        int? value = affixManager.TryGetAffixValue(affixType);
        return value == null ? 0 : value.Value;
    }

    // TODO :: Gotta make a system that handles UI for affixes
    private void UpdateDexterityUI()
    {
        int? dexValue = affixManager.TryGetAffixValue(EAffixType.Expertise);
        if (dexValue != null && dexValue.Value > 0)
        {
            if (!dexIcon.enabled)
            {
                dexIcon.enabled = true;
                dexText.enabled = true;
            }

            dexText.text = dexValue.Value.ToString();
        }
        else if (dexIcon.enabled)
        {
            dexIcon.enabled = false;
            dexText.enabled = false;
        }
    }

    private void UpdateArmorUI()
    {
        int? armorValue = affixManager.TryGetAffixValue(EAffixType.Armor);
        if (armorValue != null && armorValue.Value > 0)
        {
            if (!armorIcon.enabled)
            {
                armorIcon.enabled = true;
                armorText.enabled = true;
            }

            armorText.text = armorValue.Value.ToString();
        }
        else if (armorIcon.enabled)
        {
            armorIcon.enabled = false;
            armorText.enabled = false;
        }
    }

    public void DoHeal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    public bool IsDamaged()
    {
        return currentHealth < maxHealth;
    }

    private void UpdateHealthUI()
    {
        healthUI.text = string.Format("{0}/{1}", currentHealth, maxHealth);
    }

    public void ForecastActions(int turnNumber)
    {
        nextActions = currentEnemy.CalculateActions(2, this);

        actionOneImage.enabled = false;
        actionOneText.enabled = false;
        actionTwoImage.enabled = false;
        actionTwoText.enabled = false;

        if (nextActions.Count > 0)
        {
            nextActions[0].DisplayInformation(actionOneImage, actionOneText);
        }

        if (nextActions.Count > 1)
        {
            nextActions[1].DisplayInformation(actionTwoImage, actionTwoText);
        }

        turnUI.text = "#" + turnNumber;
    }

    public IEnumerator DoActions()
    {
        for (int i = 0; i < nextActions.Count; i++)
        {
            Vector2Int loc = Vector2Int.zero;
            if (nextActions[i].HasLocationBasedEffects(ref loc))
            {
                DDGamePlaySingletonHolder.Instance.Board.GetLocation(loc).Unhighlight();
            }
            yield return nextActions[i].ExecuteAction(this);
        }
    }

    public override void Hovered()
    {
        hoveredRenderer.enabled = true;
    }

    public override void Unhovered()
    {
        hoveredRenderer.enabled = false;
    }

    public override void FillEnemyList(ref List<DDEnemyOnBoard> enemies)
    {
        enemies.Add(this);
    }

    public void NonActionableHover()
    {
        string actionDesc = "";
        for (int i = 0; i < nextActions.Count; i++)
        {
            Vector2Int loc = Vector2Int.zero;
            if (nextActions[i].HasLocationBasedEffects(ref loc))
            {
                DDGamePlaySingletonHolder.Instance.Board.GetLocation(loc).Highlight();
            }
            actionDesc += nextActions[i].GetDescription();

            if (i != nextActions.Count - 1)
            {
                actionDesc += "\r\n";
            }
        }

        DDGamePlaySingletonHolder.Instance.Encounter.SetActionDescription(actionDesc);
    }

    public void NonActionableUnhover()
    {
        for (int i = 0; i < nextActions.Count; i++)
        {
            Vector2Int loc = Vector2Int.zero;
            if (nextActions[i].HasLocationBasedEffects(ref loc))
            {
                DDGamePlaySingletonHolder.Instance.Board.GetLocation(loc).Unhighlight();
            }
        }

        DDGamePlaySingletonHolder.Instance.Encounter.SetActionDescription("");
    }

    public bool IsPlanningToMove()
    {
        if (nextActions.Count > 0)
        {
            DDEnemyAction_Move move = nextActions[0] as DDEnemyAction_Move;
            return move != null;
        }

        return false;
    }
}