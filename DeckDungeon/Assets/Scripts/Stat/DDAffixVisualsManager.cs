using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAffixVisualsManager : MonoBehaviour
{
    [SerializeField] private DDAffixVisuals visualsPrefab;
    [SerializeField] private RectTransform visualsParent;
    [SerializeField] private EMoveDirection rowDirection = EMoveDirection.Right;
    [SerializeField] private int rowMax = 4;
    [SerializeField] private EMoveDirection columnDirection = EMoveDirection.Up;
    
    private Dictionary<EAffixType, DDAffixVisuals> visualsList = new Dictionary<EAffixType, DDAffixVisuals>();

    public void AddVisual(EAffixType type, int value)
    {
        DDAffix aff = DDGlobalManager.Instance.AffixLibrary.GetAffixByType(type);
        DDAffixVisuals visuals = Instantiate(visualsPrefab, visualsParent);
        visuals.gameObject.SetActive(true);
        visuals.SetInfo(aff, value);
        visualsList.Add(type, visuals);
    }

    public void ModifyVisual(EAffixType type, int value)
    {
        if (visualsList.TryGetValue(type, out DDAffixVisuals visuals))
        {
            if (visuals.UpdateInfo(value))
            {
                Destroy(visuals.gameObject);
                visualsList.Remove(type);
            }
        }
    }

    public void ClearVisuals()
    {
        foreach (var kvp in visualsList)
        {
            Destroy(kvp.Value.gameObject);
        }
        
        visualsList.Clear();
    }

    private void OrganizedVisuals()
    {
        // We want to stack the visuals horizontally then veritcally by using LayoutGroups
    }
}
