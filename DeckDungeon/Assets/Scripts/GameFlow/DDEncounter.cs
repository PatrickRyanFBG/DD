using System.Collections;
using System.Collections.Generic;
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

    private List<DDEnemyOnBoard> destroyedEnemies = new();

    [SerializeField] private DDPlayerMatch player;

    public IEnumeratorHelper.EventHandler PhaseChanged;
    public class DDPhaseChangeEventArgs : System.EventArgs
    {
        public EEncounterPhase Phase;
    }
    
    [SerializeField] private DDEncounterEnemyTimeline timeline;
    
    [Header("Testing")] [SerializeField] private TMPro.TextMeshProUGUI phaseDebug;

    private DDDungeonCardEncounter currentEncounter;

    private bool playersTurnEnding;

    public UnityEngine.Events.UnityEvent<DDAffixManager, EAffixType, int, int?> AffixModified;

    public void SetUpEncounter(DDDungeonCardEncounter encounter)
    {
        changingPhase = false;
        queuedChangingPhase = null;
        
        gameObject.SetActive(true);

        currentEncounter = encounter;
        currentEncounter.StartEncounter();

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
            destroyedEnemies.Add(enemy);
            
            if (enemies.Count == 0)
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
                    for (int i = enemies.Count - 1; i >= 0; i--)
                    {
                        destroyedEnemies.Add(enemy);
                    }

                    ChangeCurrentPhase(EEncounterPhase.EncounterEnd);
                }
            }
        }
    }

    public IEnumerator CheckDestroyedEnemies()
    {
        for (int i = 0; i < destroyedEnemies.Count; i++)
        {
            // Death animations happen here.
            // On death effects happen.
            yield return destroyedEnemies[i].DoDeath();
            // Remove from timeline
            timeline.RemoveEnemyFromTimeline(destroyedEnemies[i]);
            // Destroy object.
            Destroy(destroyedEnemies[i].gameObject);
            // Remove enemies.
            destroyedEnemies.RemoveAt(i--);
        }
    }

    private void Update()
    {
        if (changingPhase)
        {
            return;
        }
        else if(queuedChangingPhase != null)
        {
            ChangeCurrentPhase(queuedChangingPhase.Value);
            queuedChangingPhase = null;
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
    }

    private void DoEncounterStart()
    {
        // Do checks
        // Wait for things to come online
        ChangeCurrentPhase(EEncounterPhase.MonsterForecast);
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
                EnemyDefeated(enemy);
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
        ChangeCurrentPhase(EEncounterPhase.MonstersAct);
    }

    private IEnumerator DoMonstersAct()
    {
        DDGamePlaySingletonHolder.Instance.Board.DoAllEffects();

        yield return DoMonsterAffixes();

        // Something destroyed this enemy mid action (probably bombs exploding for now).
        // So we are going backwards for now for safety
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            yield return enemies[i].DoActions();
            
            timeline.RemoveEnemyFromTimeline(enemies[i]);
            
            // Sometimes enemies destroy themselves.
            yield return CheckDestroyedEnemies();
        }

        if (currentEncounterPhase != EEncounterPhase.EncounterEnd)
        {
            ChangeCurrentPhase(EEncounterPhase.MonsterForecast);
        }
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
        yield return CheckDestroyedEnemies();

        DDGamePlaySingletonHolder.Instance.Board.ClearAllEffects();
        
        StopCoroutine(changingPhaseCoroutine);
        
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
        Debug.Log("Attempt to change phase: " + toPhase);
        if (changingPhase)
        {
            if (queuedChangingPhase != null)
            {
                Debug.LogError("We are changing phase and also have a queued phase changed. " + currentEncounterPhase + " | " + queuedChangingPhase.Value + " | " + toPhase);
            }
            else
            {
                queuedChangingPhase = toPhase;                
            }
        }
        else
        {
            changingPhaseCoroutine = StartCoroutine(ChangeCurrentPhaseOverTIme(toPhase));
        }
    }

    // This being an enum is awkward becuase phases change phases during their change 
    // Think we have to queue up changes so that way the first one finishes and then if one is queued up to start that after the first coroutine is done
    
    private IEnumerator ChangeCurrentPhaseOverTIme(EEncounterPhase toPhase)
    {
        changingPhase = true;
        
        currentEncounterPhase = toPhase;

        switch (currentEncounterPhase)
        {
            case EEncounterPhase.EncounterStart:
                player.SetHandSizeToDefault();
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