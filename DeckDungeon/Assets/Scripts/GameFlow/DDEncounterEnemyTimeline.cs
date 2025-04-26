using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEncounterEnemyTimeline : MonoBehaviour
{
    [SerializeField] private DDEncounterEnemyIcon iconPrefab;

    [SerializeField] private RectTransform lineParent;

    private List<DDEncounterEnemyIcon> currentIcons = new();
    private Queue<DDEncounterEnemyIcon> unusedIcons = new();
    
    public void SetTimeline(List<DDEnemyOnBoard> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].ActionCount == 0)
            {
                continue;
            }
            
            DDEncounterEnemyIcon icon = GetIcon();
            icon.transform.SetAsFirstSibling();
            icon.Setup(enemies[i]);
            
            currentIcons.Add(icon);
            
            icon.gameObject.SetActive(true);
        }
    }

    public void RemoveEnemyFromTimeline(DDEnemyOnBoard enemy)
    {
        for (int i = 0; i < currentIcons.Count; i++)
        {
            if (currentIcons[i].CurrentEnemy == enemy)
            {
                currentIcons[i].gameObject.SetActive(false);
                unusedIcons.Enqueue(currentIcons[i]);
                currentIcons.RemoveAt(i);
                return;
            }
        }
    }

    public void ClearTimeline()
    {
        foreach (var t in currentIcons)
        {
            t.gameObject.SetActive(false);
            unusedIcons.Enqueue(t);
        }

        currentIcons.Clear();
    }

    private DDEncounterEnemyIcon GetIcon()
    {
        if (unusedIcons.Count > 0)
        {
            return unusedIcons.Dequeue();
        }
        
        return Instantiate(iconPrefab, lineParent);
    }
}
