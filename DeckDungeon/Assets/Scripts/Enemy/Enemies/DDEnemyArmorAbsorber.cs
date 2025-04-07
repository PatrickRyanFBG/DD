using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDEnemyArmorAbsorber : DDEnemyBase
{
    [Header("Armor Absorber")] [SerializeField]
    private int regularDamage = 5;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>();
        Vector2Int pos = actingEnemy.CurrentLocaton.Coord;

        if (actingEnemy.GetAffixValue(EAffixType.Armor) > 0)
        {
            // Regular Range Move
            actions.Add(DDEnemyActionMove.CalculateBestMove(actingEnemy, EMoveDirection.Up, true));
            // Deal Damage equal to Armor
            actions.Add(new DDEnemyActionAttackArmorBased());
        }
        else
        {
            List<int> allLaneArmors = new List<int>();
            for (int i = 0; i < DDGamePlaySingletonHolder.Instance.Board.ColumnsCount; i++)
            {
                allLaneArmors.Add(DDGamePlaySingletonHolder.Instance.Player.GetLaneAffix(EAffixType.Armor, i) ?? 0);
            }

            if (allLaneArmors[pos.x] > 0)
            {
                // Asorb Armor action
                actions.Add(new DDEnemyActionAbsorbArmor());
                // Regular Attack
                actions.Add(new DDEnemyActionAttack(regularDamage));
            }
            else
            {
                int closestLeft = -1;
                int closestRight = -1;

                for (int i = pos.x + 1; i < allLaneArmors.Count; i++)
                {
                    if (allLaneArmors[i] > 0)
                    {
                        closestRight = i;
                        break;
                    }
                }

                for (int i = pos.x - 1; i >= 0; i--)
                {
                    if (allLaneArmors[i] > 0)
                    {
                        closestLeft = i;
                        break;
                    }
                }

                bool armorLeft = closestLeft > -1;
                bool armorRight = closestRight > -1;
                int diffLeft = Mathf.Abs(closestLeft - pos.x);
                int diffRight = Mathf.Abs(closestRight - pos.x);

                if (armorLeft && armorRight)
                {
                    if (diffLeft == diffRight)
                    {
                        if (RandomHelpers.GetRandomBool())
                        {
                            // GO LEFT
                            CalculateMovesWithDirection(actingEnemy, EMoveDirection.Left, diffLeft, ref actions);
                        }
                        else
                        {
                            // GO RIGHT
                            CalculateMovesWithDirection(actingEnemy, EMoveDirection.Right, diffRight, ref actions);
                        }
                    }
                    else if (diffLeft < diffRight)
                    {
                        CalculateMovesWithDirection(actingEnemy, EMoveDirection.Left, diffLeft, ref actions);
                    }
                    else
                    {
                        CalculateMovesWithDirection(actingEnemy, EMoveDirection.Right, diffRight, ref actions);
                    }
                }
                else if (armorLeft)
                {
                    CalculateMovesWithDirection(actingEnemy, EMoveDirection.Left, diffLeft, ref actions);
                }
                else if (armorRight)
                {
                    CalculateMovesWithDirection(actingEnemy, EMoveDirection.Right, diffRight, ref actions);
                }
                else
                {
                    // No Armor Found
                    actingEnemy.GenericRangeAttackActions(ref actions, new DDEnemyActionAttack(regularDamage));
                }
            }
        }

        return actions;
    }

    private void CalculateMovesWithDirection(DDEnemyOnBoard actingEnemy, EMoveDirection dir, int amount, ref List<DDEnemyActionBase> actions)
    {
        if (amount == 1)
        {
            // Attack
            actions.Add(new DDEnemyActionAttack(regularDamage));
            // Move
            actions.Add(DDEnemyActionMove.CalculateBestMove(actingEnemy, dir, false));
        }
        else if (amount == 2)
        {
            // Move
            actions.Add(DDEnemyActionMove.CalculateBestMove(actingEnemy, dir, false));
            // Move
            actions.Add(new DDEnemyActionMove(dir));
        }
        else
        {
            // Move
            actions.Add(DDEnemyActionMove.CalculateBestMove(actingEnemy, dir, false));
            // Attack
            actions.Add(new DDEnemyActionAttack(regularDamage));
        }
    }
}

public class DDEnemyActionAbsorbArmor : DDEnemyActionBase
{
    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.ActionArmorAbsorb;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        int armorAmount =
            DDGamePlaySingletonHolder.Instance.Player.GetLaneAffix(EAffixType.Armor, enemy.CurrentLocaton.Coord.x) ?? 0;

        if (armorAmount > 0)
        {
            DDGamePlaySingletonHolder.Instance.Player.ModifyAffix(EAffixType.Armor, 0, true);
        }
        
        yield return null;

        enemy.ModifyAffix(EAffixType.Armor, armorAmount, true);
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        text.enabled = false;
        image.texture = GetIcon();
        image.enabled = true;
    }

    public override string GetDescription()
    {
        return "This enemy will absorb armor in lane.";
    }
}

public class DDEnemyActionAttackArmorBased : DDEnemyActionAttack
{
    public DDEnemyActionAttackArmorBased() : base(0)
    {
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.AttackArmorBased;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        damage = enemy.GetAffixValue(EAffixType.Armor);

        yield return base.ExecuteAction(enemy);
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        text.enabled = false;
        image.texture = GetIcon();
        image.enabled = true;
    }

    public override string GetDescription()
    {
        return "This enemy will deal its armor in damage to you";
    }
}