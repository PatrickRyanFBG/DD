using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DDEnemyOnBoard : DDSelection
{
    private int maxHealth;
    private int currentHealth;
    public int CurrentHealth => currentHealth;

    private DDEnemyBase currentEnemy;
    public DDEnemyBase CurrentEnemy => currentEnemy;

    private DDAffixManager affixManager;

    private int turnNumber;
    public int TurnNumber => turnNumber;

    [SerializeField] private DDAffixVisualsManager affixVisualsManager;

    [Header("Testing")] [SerializeField] private TMPro.TextMeshProUGUI healthUI;

    [SerializeField] private TMPro.TextMeshProUGUI turnUI;

    [SerializeField] private RawImage image;

    [SerializeField] private RawImage actionOneImage;
    [SerializeField] private TMPro.TextMeshProUGUI actionOneText;

    [SerializeField] private RawImage actionTwoImage;
    [SerializeField] private TMPro.TextMeshProUGUI actionTwoText;

    [SerializeField] private RawImage hoveredImage;

    private List<DDEnemyActionBase> nextActions = new List<DDEnemyActionBase>();

    private DDLocation currentLocation;
    public DDLocation CurrentLocaton => currentLocation;

    [SerializeField] private GameObject attackPrefab;
    public GameObject AttackPrefab => attackPrefab;

    public void SetUpEnemy(DDEnemyBase enemyBase)
    {
        currentEnemy = enemyBase;
        image.texture = enemyBase.Image;
        maxHealth = enemyBase.StartingHealth;
        currentHealth = maxHealth;
        UpdateHealthUI();
        affixManager = new DDAffixManager(affixVisualsManager, EAffixOwner.Enemy);
        affixManager.AffixAdjusted.AddListener(AffixAdjusted);
        affixManager.ModifyValueOfAffix(EAffixType.Armor, enemyBase.StartingArmor, true);
    }

    private void AffixAdjusted(EAffixType changedAffix)
    {
        switch (changedAffix)
        {
            case EAffixType.Expertise:
                break;
            case EAffixType.Armor:
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

    // Probably make this enumerator?
    public void TakeDamage(int amount, ERangeType rangeType, bool bypassArmor)
    {
        int rowBonus = DDGamePlaySingletonHolder.Instance.Board.GetMeleeRangedBonus(rangeType, currentLocation.Coord.y);
        int totalDamage = amount + rowBonus;
        if (!bypassArmor)
        {
            totalDamage = affixManager.ModifyValueOfAffix(EAffixType.Armor, -totalDamage, false) ?? -totalDamage;
        }

        if (totalDamage <= 0)
        {
            totalDamage = totalDamage * -1;

            if (totalDamage > 0)
            {
                currentHealth -= totalDamage;

                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    DDGamePlaySingletonHolder.Instance.Encounter.EnemyDefeated(this);
                }
                else
                {
                    // Take Damage Feedback.
                    image.DOColor(Color.red, .2f).SetLoops(4, LoopType.Yoyo);
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
        return value ?? 0;
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
        healthUI.text = $"{currentHealth}/{maxHealth}";
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

    public override bool Hovered()
    {
        hoveredImage.enabled = true;
        return true;
    }

    public override void Unhovered()
    {
        hoveredImage.enabled = false;
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

        DDGlobalManager.Instance.ToolTip.SetText(actionDesc);
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

        DDGlobalManager.Instance.ToolTip.SetText("");
    }

    public bool IsPlanningToMove()
    {
        if (nextActions.Count > 0)
        {
            DDEnemyActionMove move = nextActions[0] as DDEnemyActionMove;
            return move != null;
        }

        return false;
    }

    public float CurrentHealthPercent()
    {
        return currentHealth / (float)maxHealth;
    }

    public IEnumerator DoAffixes()
    {
        // Bleed
        int? bleedValue = affixManager.TryGetAffixValue(EAffixType.Bleed);
        if (bleedValue != null)
        {
            TakeDamage(bleedValue.Value, ERangeType.None, true);

            yield return null;

            affixManager.ModifyValueOfAffix(EAffixType.Bleed, -1, false);
        }
    }
}