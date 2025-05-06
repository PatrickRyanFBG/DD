using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDDungeonCombatData : DDScriptableObject
{
    [SerializeField] private List<DDCombatTier> tiers;

    [System.NonSerialized] private Dictionary<ECombatTier, DDCombatTier> tiersDictionary;
    
    public DDCombatEnemySetup GetRandomEnemySetup(ECombatTier tier, HashSet<string> usedSetups)
    {
        if (tiersDictionary == null)
        {
            tiersDictionary = new Dictionary<ECombatTier, DDCombatTier>();
            for (int i = 0; i < tiers.Count; i++)
            {
                tiersDictionary[tiers[i].Tier] = tiers[i];
            }
        }

        DDCombatEnemySetup setup = null;

        // Get setup from a tier by atleast the value entered
        while (setup == null)
        {
            if (tiersDictionary.TryGetValue(tier, out var combatTier))
            {
                if (usedSetups.Count == combatTier.EnemySetups.Count)
                {
                    usedSetups.Clear();
                }
                
                setup = combatTier.EnemySetups.GetRandomElement();

                if (usedSetups.Contains(setup.GUID))
                {
                    setup = null;
                }
            }
            else
            {
                tier++;
            }
        }

        return setup;
    }

    private void OnValidate()
    {
        foreach (var tier in tiers)
        {
            tier.OnValidate();
        }
    }
}

[System.Serializable]
public class DDCombatTier
{
    [SerializeField] private ECombatTier tier;
    public ECombatTier Tier => tier;
    
    [SerializeField] private List<DDCombatEnemySetup> enemySetups;

    public List<DDCombatEnemySetup> EnemySetups => enemySetups;

    public void OnValidate()
    {
        foreach (var setup in enemySetups)
        {
            setup.OnValidate();
        }
    }
}

[System.Serializable]
public class DDCombatEnemySetup
{
    [SerializeField] private string guid;
    public string GUID => guid;
    
    [SerializeField] private List<DDCombatEnemyInfo> enemies;
    public List<DDCombatEnemyInfo> Enemies => enemies;

    public void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString();
        }
    }
}

[System.Serializable]
public class DDCombatEnemyInfo
{
    [SerializeField] private int amount;
    public int Amount => amount;

    [SerializeField] private DDEnemyBase enemy;
    public DDEnemyBase Enemy => enemy;
}
