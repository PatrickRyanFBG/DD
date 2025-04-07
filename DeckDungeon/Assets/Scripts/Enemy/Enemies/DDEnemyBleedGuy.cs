using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemyBleedGuy : DDEnemyBase
{
    [Header("Bleed Guy")] [SerializeField] private DDEnemyBase spinningAxe;

    [SerializeField] private int damage = 5;

    [SerializeField] private int expertiseGain = 1;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        // Get All Enemies
        List<DDEnemyOnBoard> allEnemies = new();
        DDGamePlaySingletonHolder.Instance.Board.GetAllEnemies(ref allEnemies);
        int axesCount = 0;
        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i].CurrentEnemy is DDEnemySpinningAxe)
            {
                axesCount++;
            }
        }

        // Check for Axes
        // If 0 axes
        if (axesCount == 0)
        {
            Vector2Int firstLoc = DDGamePlaySingletonHolder.Instance.Board.GetRandomLocation();
            while (DDGamePlaySingletonHolder.Instance.Board.GetEnemyAtLocation(firstLoc))
            {
                firstLoc = DDGamePlaySingletonHolder.Instance.Board.GetRandomLocation();
            }

            Vector2Int secondLoc = DDGamePlaySingletonHolder.Instance.Board.GetRandomLocation();
            while (firstLoc == secondLoc || DDGamePlaySingletonHolder.Instance.Board.GetEnemyAtLocation(secondLoc))
            {
                secondLoc = DDGamePlaySingletonHolder.Instance.Board.GetRandomLocation();
            }

            // Two actions of spawning random axes
            actions.Add(new DDEnemyActionSpawnEnemy(spinningAxe, firstLoc, spinningAxe.Image));
            actions.Add(new DDEnemyActionSpawnEnemy(spinningAxe, secondLoc, spinningAxe.Image));
        }
        // If 1 axes
        else if (axesCount == 1)
        {
            Vector2Int firstLoc = DDGamePlaySingletonHolder.Instance.Board.GetRandomLocation();
            while (DDGamePlaySingletonHolder.Instance.Board.GetEnemyAtLocation(firstLoc))
            {
                firstLoc = DDGamePlaySingletonHolder.Instance.Board.GetRandomLocation();
            }

            // spawn axe, melee move or attack
            actions.Add(new DDEnemyActionSpawnEnemy(spinningAxe, firstLoc, spinningAxe.Image));

            if (RandomHelpers.GetRandomBool(50))
            {
                actions.Add(DDEnemyActionMove.CalculateBestMove(actingEnemy, EMoveDirection.Down, true));
            }
            else
            {
                actions.Add(new DDEnemyActionAttack(damage));
            }
        }
        // If 2 axes
        else
        {
            bool didHeal = false;
            if (actingEnemy.CurrentHealthPercent() < .5f &&
                DDGamePlaySingletonHolder.Instance.Player.GetAffixValue(EAffixType.Bleed) > 0)
            {
                if (RandomHelpers.GetRandomBool(50))
                {
                    didHeal = true;
                    actions.Add(new DDEnemyActionHealPerBleed());
                    actions.Add(DDEnemyActionMove.CalculateBestMove(actingEnemy, EMoveDirection.Down, true));
                }
            }

            if (!didHeal)
            {
                if (!DDGamePlaySingletonHolder.Instance.Player.IsLaneArmored(actingEnemy.CurrentLocaton.Coord.x))
                {
                    // If Armorless Lane
                    // Attack But gains damage per bleed
                    actions.Add(new DDEnemyActionModifyAffix(EAffixType.Expertise, expertiseGain, false));
                    // Self Heal?
                    actions.Add(new DDEnemyActionAttack(damage));
                }
                else
                {
                    // If armorlane
                    // Melee move
                    actions.Add(DDEnemyActionMove.CalculateBestMove(actingEnemy, EMoveDirection.Down, true));
                    // Attack but gains damage per bleed
                    actions.Add(new DDEnemyActionAttack(damage));
                }
            }
        }

        return actions;
    }
}

public class DDEnemyActionHealPerBleed : DDEnemyActionBase
{
    public override void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text)
    {
        text.enabled = false;

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        enemy.DoHeal(DDGamePlaySingletonHolder.Instance.Player.GetAffixValue(EAffixType.Bleed));

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.ActionHealPerBleed;
    }

    public override string GetDescription()
    {
        return "This enemy will heal equal to your bleed";
    }
}