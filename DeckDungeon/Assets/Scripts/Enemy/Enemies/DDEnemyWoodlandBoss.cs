using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDEnemyWoodlandBoss : DDEnemyBase
{
    [Header("Woodland Boss")] 
    [SerializeField] private float emptyLocSpawnOverPercent = .5f;

    [SerializeField] private int attackDamage = 5;
    [SerializeField] private int sneakAttackDamage = 10;
    
    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new();
        
        if (actingEnemy.gameObject.activeInHierarchy)
        {
            /*
            Vector2Int enemyLoc = actingEnemy.CurrentLocaton.Coord;
            if (enemyLoc.x == 0 || enemyLoc.x == DDGamePlaySingletonHolder.Instance.Board.RowCountIndex)
            {
                // We are on the edge somewhere, lets try to move
                
            }
            else if (enemyLoc.y == 0 || enemyLoc.y == DDGamePlaySingletonHolder.Instance.Board.ColumnCountIndex)
            {
                
            }
            else
            */
            {
                List<DDLocation> surroundingLocations =
                    DDGamePlaySingletonHolder.Instance.Board.GetSurroundingLocations(actingEnemy.CurrentLocaton.Coord);
            
                List<Vector2Int> emptyLocations = new List<Vector2Int>();

                foreach (DDLocation location in surroundingLocations)
                {
                    if (!location.HasEnemy())
                    {
                        emptyLocations.Add(location.Coord);
                    }
                }
                
                if (emptyLocations.Count / (float)surroundingLocations.Count > emptyLocSpawnOverPercent)
                {
                    actions.Add(new DDEnemyActionPlantBushes(actingEnemy.CurrentLocaton.Coord));
                }
                else
                {
                    actions.Add(new DDEnemyActionAttack(attackDamage));
                }
            
                actions.Add(new DDEnemyActionHideInEnemy());
            }
        }
        else
        {
            actions.Add(new DDEnemyActionRevealAndSneakAttack(attackDamage, sneakAttackDamage));
        }
        return actions;
    }
}

public class DDEnemyActionHideInEnemy : DDEnemyActionBase
{
    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.ActionHide;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        List<DDLocation> surroundingLocations =
            DDGamePlaySingletonHolder.Instance.Board.GetSurroundingLocations(enemy.CurrentLocaton.Coord);
        
        List<DDEnemyOnBoard> bushes = new();

        foreach (DDLocation location in surroundingLocations)
        {
            if (location.HasEnemy())
            {
                var surroundingEnemy = location.GetEnemy();
                if (surroundingEnemy.CurrentEnemy is DDEnemyBush)
                {
                    bushes.Add(surroundingEnemy);
                }
            }
        }

        if (bushes.Count > 0)
        {
            DDEnemyOnBoard eob = bushes.GetRandomElement();
            DDEnemyBush bush = eob.CurrentEnemy as DDEnemyBush;
            // If it is a bush
            if (bush)
            {
                enemy.CurrentLocaton.SnapEnemyToHere(null);
                // We hide
                enemy.gameObject.SetActive(false);
                enemy.SnapLocation(eob.CurrentLocaton);
                // We tell the bush to reveal us on death
                eob.DeathActions.Add(BushDestroyed(enemy, eob.CurrentLocaton.Coord));
            }
        }
        
        yield return null;
    }

    private IEnumerator BushDestroyed(DDEnemyOnBoard enemy, Vector2Int location)
    {
        var loc = DDGamePlaySingletonHolder.Instance.Board.GetLocation(location);
        loc.SnapEnemyToHere(enemy);
        enemy.gameObject.SetActive(true);
        
        yield return null;
    }

    public override string GetDescription()
    {
        return "Attempts to hide in a nearby bush.";
    }
}

public class DDEnemyActionRevealAndSneakAttack : DDEnemyActionAttack
{
    private int sneakAttackDamage;

    public DDEnemyActionRevealAndSneakAttack(int regDamage, int sneakDamage) : base(regDamage)
    {
        sneakAttackDamage = sneakDamage;
    }
    
    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.ActionReveal;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        DDEnemyOnBoard bush = enemy.CurrentLocaton.GetEnemy();
        // bush = us if the bush was already destroyed
        if (bush && bush != enemy)
        {
            // Bush will reveal us after being destroyed
            bush.TakeDamage(99, ERangeType.None, true);
            yield return DDGamePlaySingletonHolder.Instance.Encounter.CheckDestroyedEnemies();

            // if we were hidden we do sneak attack damage
            damage = sneakAttackDamage;
        }
        else
        {
            // we were already revealed, do some anim here?
        }

        yield return base.ExecuteAction(enemy);
    }

    public override string GetDescription()
    {
        return "If you are seeing this, the enemy is already revealed.";
    }
}

public class DDEnemyActionPlantBushes : DDEnemyActionBase
{
    private List<DDLocation> surroundingLocations;
    
    public DDEnemyActionPlantBushes(Vector2Int loc)
    {
        surroundingLocations = DDGamePlaySingletonHolder.Instance.Board.GetSurroundingLocations(loc);
    }
    
    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.EnemyDictionary.Bush.Image;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        surroundingLocations.Shuffle();

        foreach (var loc in surroundingLocations)
        {
            DDGamePlaySingletonHolder.Instance.Board.SpawnEnemy(loc.Coord, DDGamePlaySingletonHolder.Instance.EnemyLibrary.EnemyDictionary.Bush);
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override string GetDescription()
    {
        return "Plants bushed in surrounding locations.";
    }
}