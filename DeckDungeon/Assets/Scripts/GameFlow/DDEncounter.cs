using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class DDEncounter : MonoBehaviour
{
    private EEncounterPhase currentEncounterPhase = EEncounterPhase.EncounterStart;
    public EEncounterPhase CurrentPhase => currentEncounterPhase;

    private Coroutine changingPhaseCoroutine;
    private bool changingPhase = false;
    private EEncounterPhase? queuedChangingPhase = null;
    
    private List<DDEnemyOnBoard> enemies = new List<DDEnemyOnBoard>();
    public List<DDEnemyOnBoard> AllEnemies => enemies;

    [SerializeField] private DDPlayerMatch player;

    public IEnumeratorHelper.EventHandler PhaseChanged;
    public class DDPhaseChangeEventArgs : System.EventArgs
    {
        public EEncounterPhase Phase;
    }
    
    [SerializeField] private DDEncounterEnemyTimeline timeline;
    
    [Header("Testing")] [SerializeField] private TMPro.TextMeshProUGUI phaseDebug;

    [SerializeField] private RectTransform completedBanner;
    
    private DDDungeonCardEncounter currentEncounter;

    private bool playersTurnEnding;

    public UnityEngine.Events.UnityEvent<DDAffixManager, EAffixType, int, int?> AffixModified;

    private ECombatTier lastCombatTier = ECombatTier.Intro;
    private Dictionary<EEncounterType, HashSet<string>> usedEncounterSetups = new();

    [SerializeField] private DDEncounterDebugUI encounterDebugUI;
    
    public void SetUpEncounter(DDDungeonCardEncounter encounter)
    {
        encounterDebugUI.ClearAndOff();
        
        changingPhase = false;
        queuedChangingPhase = null;
        playersTurnEnding = false;

        currentEncounter = encounter;
        // Make this coroutine to do spawning animations eventually
        if (lastCombatTier != DDGamePlaySingletonHolder.Instance.Dungeon.DungeonStats.CombatTier)
        {
            usedEncounterSetups.Clear();
            lastCombatTier = DDGamePlaySingletonHolder.Instance.Dungeon.DungeonStats.CombatTier;
        }

        gameObject.SetActive(true);

        if (usedEncounterSetups.TryGetValue(encounter.EncounterType, out var setups))
        {
            currentEncounter.StartEncounter(ref setups);
        }
        else
        {
            HashSet<string> usedSetups = new HashSet<string>();
            usedEncounterSetups.Add(encounter.EncounterType, usedSetups);
            currentEncounter.StartEncounter(ref usedSetups);
        }

        player.EncounterStarted();
        player.ShuffleInDeck();
        ChangeCurrentPhase(EEncounterPhase.EncounterStart);
    }

    public void RegisterEnemy(DDEnemyOnBoard enemy)
    {
        enemies.Add(enemy);
    }

    public void EnemyDefeated(DDEnemyOnBoard enemy)
    {
        if (enemies.Remove(enemy))
        {
            // Some combats want to end if a specific character is destroyed or objective met
            if (enemies.Count == 0 || currentEncounter.ShouldEndEarly())
            {
                ChangeCurrentPhase(EEncounterPhase.EncounterEnd);
            }
            else
            {
                bool allFriendly = true;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (!enemies[i].CurrentEnemy.Friendly)
                    {
                        allFriendly = false;
                        break;
                    }
                }

                if (allFriendly)
                {
                    ChangeCurrentPhase(EEncounterPhase.EncounterEnd);
                }
            }
        }
    }

    public IEnumerator CheckDestroyedEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].CurrentEnemyState == EEnemyState.Dead)
            {
                // Death animations happen here.
                // On death effects happen.
                yield return enemies[i].DoDeath();
                // Remove from timeline
                timeline.RemoveEnemyFromTimeline(enemies[i]);
                // Destroy object.
                Destroy(enemies[i].gameObject);
                
                // Go through full enemy defeated
                EnemyDefeated(enemies[i--]);
            }
        }
    }

    public void ClearAllEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            // Remove from timeline
            timeline.RemoveEnemyFromTimeline(enemies[i]);
            // Destroy object.
            Destroy(enemies[i].gameObject);
            // Remove from enemies.
            enemies.RemoveAt(i);
        }
    }

    private void Update()
    {
        if (changingPhase)
        {
            return;
        }
        
        if(queuedChangingPhase != null)
        {
            ChangeCurrentPhase(queuedChangingPhase.Value);
            return;
        }
        
        switch (currentEncounterPhase)
        {
            case EEncounterPhase.EncounterStart:
                DoEncounterStart();
                break;
            case EEncounterPhase.MonsterForecast:
                DoMonsterForecast();
                break;
            case EEncounterPhase.PlayersTurn:
                DoPlayersTurn();
                break;
        }

        phaseDebug.text = currentEncounterPhase.ToString();

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            encounterDebugUI.ToggleWindow();
        }
    }

    private void DoEncounterStart()
    {
        // Do checks
        // Wait for things to come online
        ChangeCurrentPhase(EEncounterPhase.MonsterSpawn);
    }

    private IEnumerator DoMonsterSpawn()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            DDEnemyOnBoard enemy = enemies[i];
            
            yield return enemy.DoSpawn();
        }

        if (currentEncounterPhase != EEncounterPhase.EncounterEnd)
        {
            ChangeCurrentPhase(EEncounterPhase.MonsterForecast);
        }
    }

    private void DoMonsterForecast()
    {
        // Monster Priority?
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].ForecastActions(enemies.Count - i);
        }
        
        timeline.SetTimeline(enemies);
        
        ChangeCurrentPhase(EEncounterPhase.PlayersStartTurn);
    }

    private void DoPlayersTurn()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (enemies.Count > 0)
            {
                DDEnemyOnBoard enemy = enemies[0];
                enemy.TakeDamage(9999, ERangeType.Pure, true, false);
                if (enemies.Count > 0 && CurrentPhase != EEncounterPhase.EncounterEnd)
                { 
                    StartCoroutine(CheckDestroyedEnemies());
                }
            }
        }
    }

    private IEnumerator DoPlayersStartTurn()
    {
        yield return null;

        int bleedAmount = player.GetAffixValue(EAffixType.Bleed);
        if (bleedAmount > 0)
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DoDamage(bleedAmount);
            player.ModifyAffix(EAffixType.Bleed, -1, false);
        }

        yield return null;
        
        ChangeCurrentPhase(EEncounterPhase.PlayersTurn);
    }
    
    private IEnumerator DoPlayersEndTurn()
    {
        playersTurnEnding = true;
        yield return player.EndOfTurn();
        yield return player.DiscardHand();
        ChangeCurrentPhase(EEncounterPhase.MonstersStartTurn);
    }

    private IEnumerator DoMonstersAct()
    {
        // Something destroyed this enemy mid action (probably bombs exploding for now).
        // So we are going backwards for now for safety
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            // If an enemy dies to retaliate it'll be removed from this list as part of it's action
            DDEnemyOnBoard enemy = enemies[i];
            
            yield return enemy.DoActions();

            timeline.RemoveEnemyFromTimeline(enemy);
            
            // Sometimes enemies destroy themselves.
            yield return CheckDestroyedEnemies();
        }

        if (currentEncounterPhase != EEncounterPhase.EncounterEnd)
        {
            ChangeCurrentPhase(EEncounterPhase.MonsterForecast);
        }
    }

    private IEnumerator DoMonstersStartTurn()
    {
        // make this co routine but also not used so later
        DDGamePlaySingletonHolder.Instance.Board.DoAllEffects();

        yield return DoMonsterAffixes();
        
        ChangeCurrentPhase(EEncounterPhase.MonstersAct);
    }
    
    private IEnumerator DoMonsterAffixes()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            yield return enemies[i].DoAffixes();
            yield return CheckDestroyedEnemies();
        }
    }

    private IEnumerator DoEncounterEnd()
    {
        // Clear all dead enemies
        yield return CheckDestroyedEnemies();
        
        // Sometimes there are left over enemies like if we have all friendlies or a combat ended on a specific event
        // Just clear all enemies instantly with no death effects
        ClearAllEnemies();

        DDGamePlaySingletonHolder.Instance.Board.ClearAllEffects();
        
        StopCoroutine(changingPhaseCoroutine);

        Vector3 startPos = -(Vector3.right * Screen.width / 2);
        startPos.x -= 300;
        completedBanner.localPosition = startPos;
        completedBanner.gameObject.SetActive(true);

        Vector3 endPos = startPos * -1;

        var seq = LSequence.Create();
        seq.Append(LMotion.Create(startPos, Vector3.zero, .75f).WithEase(Ease.OutSine).BindToLocalPosition(completedBanner));
        seq.AppendInterval(.25f);
        seq.Append(LMotion.Create(Vector3.zero, endPos, .75f).WithEase(Ease.OutSine).BindToLocalPosition(completedBanner));
        seq.AppendInterval(.25f);
        var handle = seq.Run();
        yield return handle.ToYieldInstruction();
        
        completedBanner.gameObject.SetActive(false);
        gameObject.SetActive(false);
        
        DDGamePlaySingletonHolder.Instance.Dungeon.EncounterCompleted(currentEncounter);
    }

    public void PlayerEndedTurn()
    {
        if (currentEncounterPhase == EEncounterPhase.PlayersTurn && !playersTurnEnding)
        {
            ChangeCurrentPhase(EEncounterPhase.PlayersEndTurn);
        }
    }

    private void ChangeCurrentPhase(EEncounterPhase toPhase)
    {
        if (changingPhase)
        {
            if (queuedChangingPhase != null)
            {
                if (queuedChangingPhase.Value == EEncounterPhase.EncounterEnd)
                {
                    // This happens right now if something kills enemies at the end of players turn like bomb finish
                    // Or something kills all enemies at the MonstersStartTurn
                }
                else
                {
                    Debug.LogError("We are changing phase and also have a queued phase changed. " + currentEncounterPhase + " | " + queuedChangingPhase.Value + " | " + toPhase);
                }
            }
            else
            {
                Debug.Log("Queueing phase: " + toPhase);
                queuedChangingPhase = toPhase;                
            }
        }
        else
        {
            Debug.Log("Changing phase: " + toPhase);
            changingPhaseCoroutine = StartCoroutine(ChangeCurrentPhaseOverTime(toPhase));
        }
    }

    // This being an enum is awkward becuase phases change phases during their change 
    // Think we have to queue up changes so that way the first one finishes and then if one is queued up to start that after the first coroutine is done
    
    private IEnumerator ChangeCurrentPhaseOverTime(EEncounterPhase toPhase)
    {
        changingPhase = true;
        
        currentEncounterPhase = toPhase;
        queuedChangingPhase = null;
        
        switch (currentEncounterPhase)
        {
            case EEncounterPhase.EncounterStart:
                player.SetHandSizeToDefault();
                break;
            case EEncounterPhase.MonsterSpawn:
                yield return DoMonsterSpawn();
                break;
            case EEncounterPhase.MonsterForecast:
                break;
            case EEncounterPhase.PlayersStartTurn:
                yield return DoPlayersStartTurn();
                break;
            case EEncounterPhase.PlayersTurn:
                DDGamePlaySingletonHolder.Instance.PlayerSelector.SetToPlayerCard();
                player.ResetMomentum();
                yield return player.DrawFullHand();
                break;
            case EEncounterPhase.PlayersEndTurn:
                yield return DoPlayersEndTurn();
                break;
            case EEncounterPhase.MonstersStartTurn:
                yield return DoMonstersStartTurn();
                break;
            case EEncounterPhase.MonstersAct:
                playersTurnEnding = false;
                yield return DoMonstersAct();
                break;
            case EEncounterPhase.EncounterEnd:
                yield return DoEncounterEnd();
                break;
            default:
                break;
        }

        DDPhaseChangeEventArgs eventArgs = new DDPhaseChangeEventArgs() { Phase = currentEncounterPhase };
        yield return PhaseChanged.Occured(this, eventArgs);

        changingPhase = false;
    }
}