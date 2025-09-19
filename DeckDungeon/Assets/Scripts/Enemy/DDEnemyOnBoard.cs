using System;
using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DDEnemyOnBoard : DDSelection
{
    private EEnemyState currentEnemyState = EEnemyState.PreSpawn;
    public EEnemyState CurrentEnemyState => currentEnemyState;
    
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

    [SerializeField] private DDEnemyActionIcon actionOneIcon;
    [SerializeField] private DDEnemyActionIcon actionTwoIcon;

    [SerializeField] private RawImage hoveredImage;

    private List<DDEnemyActionBase> nextActions = new List<DDEnemyActionBase>();
    public int ActionCount => nextActions.Count;
    public List<IEnumerator> DeathActions = new();

    private DDLocation currentLocation;
    public DDLocation CurrentLocaton => currentLocation;

    [SerializeField] private GameObject attackPrefab;
    public GameObject AttackPrefab => attackPrefab;

    [HideInInspector] public DDEncounterEnemyIcon MatchingIcon;

    private bool canNonActionableHover = true;

    private MotionHandle colorFlashHandle;

    private bool hasSpawned;
    
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

        if (currentEnemy.StartSpawned)
        {
            hasSpawned = true;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }

    public IEnumerator DoSpawn()
    {
        currentEnemyState = EEnemyState.Spawning;
        
        if (!hasSpawned)
        {
            //spawn anims here later
            var handle = LMotion.Create(Vector3.zero, Vector3.one, .75f).WithEase(Ease.OutBack).BindToLocalScale(transform);
            yield return handle.ToYieldInstruction();
        }
        
        // spawn effects here

        currentEnemyState = EEnemyState.Alive;
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
        if (currentLocation)
        {
            transform.parent = currentLocation.transform;
            transform.localPosition = Vector3.zero;
        }
    }

    public IEnumerator SetLocation(DDLocation location)
    {
        currentLocation = location;
        if (currentLocation)
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

        var handle = LMotion.Create(startPos, targetPos, 1f).WithEase(Ease.InOutSine).BindToPosition(transform);
        yield return handle.ToYieldInstruction();
    }

    public IEnumerator MoveToLocationAndBack(Vector3 pos, float midWayPause, Action midWayAction = null)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = Vector3.Lerp(startPos, pos, .75f);
        
        var handle = LMotion.Create(startPos, targetPos, .5f).WithEase(Ease.InSine).BindToPosition(transform);
        yield return handle.ToYieldInstruction();
        
        midWayAction?.Invoke();

        yield return new WaitForSeconds(midWayPause);
        
        handle = LMotion.Create(targetPos, startPos, .5f).WithEase(Ease.InOutSine).BindToPosition(transform);
        yield return handle.ToYieldInstruction();
    }

    // Probably make this enumerator? +1 for this?
    public void TakeDamage(int amount, ERangeType rangeType, bool bypassArmor, bool fromPlayer = true)
    {
        int rowBonus = DDGamePlaySingletonHolder.Instance.Board.GetMeleeRangedBonus(rangeType, currentLocation.Coord.y);
        int totalDamage = amount + rowBonus;
        if (!bypassArmor)
        {
            totalDamage = affixManager.ModifyValueOfAffix(EAffixType.Armor, -totalDamage, false) ?? -totalDamage;
        }
        else
        {
            totalDamage *= -1;
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
                    currentEnemyState = EEnemyState.Dead;                    
                }
                else
                {
                    TakeDamageFeedback();
                }

                UpdateHealthUI();
            }
        }

        if (fromPlayer)
        {
            int retaliate = GetAffixValue(EAffixType.Retaliate);
            if (retaliate > 0)
            {
                DDGamePlaySingletonHolder.Instance.Player.DealDamageInLane(retaliate, currentLocation.Coord.y);
            }
        }
    }

    public void TakeDamageFeedback()
    {
        // Take Damage Feedback.
        image.color = Color.white;
        colorFlashHandle.TryComplete();
        colorFlashHandle = LMotion.Create(Color.white, Color.red, .2f).WithLoops(4, LoopType.Yoyo).BindToColor(image);
        
        LMotion.Punch.Create(Vector3.one, Vector3.one * .25f, .25f).WithFrequency(5).BindToLocalScale(transform);
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

        actionOneIcon.Clear();
        actionTwoIcon.Clear();

        if (nextActions.Count > 0)
        {
            actionOneIcon.SetUpAction(nextActions[0]);
        }

        if (nextActions.Count > 1)
        {
            actionTwoIcon.SetUpAction(nextActions[1]);
        }

        turnUI.text = "#" + turnNumber;
    }

    public IEnumerator DoActions()
    {
        canNonActionableHover = false;
        
        for (int i = 0; i < nextActions.Count; i++)
        {
            nextActions[i].HideLocationBasedEffects();

            yield return nextActions[i].ExecuteAction(this);
        }
        
        canNonActionableHover = true;
    }

    public override void Hovered(bool fromAnotherSelection = false)
    {
        base.Hovered(fromAnotherSelection);
        hoveredImage.enabled = true;
    }

    public override void Unhovered(bool fromAnotherSelection = false)
    {
        hoveredImage.enabled = false;
    }

    public override void FillEnemyList(ref List<DDEnemyOnBoard> enemies)
    {
        enemies.Add(this);
    }

    public void NonActionableHover()
    {
        if (!canNonActionableHover)
        {
            return;
        }
        
        for (int i = 0; i < nextActions.Count; i++)
        {
            nextActions[i].ShowLocationBasedEffects();
        }

        if (MatchingIcon)
        {
            MatchingIcon.PersonalHover();
        }
    }

    public void NonActionableUnhover()
    {
        for (int i = 0; i < nextActions.Count; i++)
        {
            nextActions[i].HideLocationBasedEffects();
        }

        if (MatchingIcon)
        {
            MatchingIcon.PersonalUnhover();
        }

#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
#endif
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
            TakeDamage(bleedValue.Value, ERangeType.Pure, true);

            yield return new WaitForSeconds(.25f);

            affixManager.ModifyValueOfAffix(EAffixType.Bleed, -1, false);
        }
    }

    public IEnumerator DoDeath()
    {
        currentEnemyState = EEnemyState.Dying;

        foreach (var action in DeathActions)
        {
            yield return action;
        }

        var handle = LMotion.Create(Vector3.one, Vector3.zero, .5f).WithEase(Ease.InBack).BindToLocalScale(transform);
        yield return handle.ToYieldInstruction();
         
        colorFlashHandle.TryComplete();
        
        // Poof effects here
        
        yield return CurrentEnemy.OnDeath();
    }
}