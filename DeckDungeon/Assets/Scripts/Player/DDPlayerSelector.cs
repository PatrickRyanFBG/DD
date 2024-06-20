using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerSelector : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask currentMask;

    private Ray r;
    private RaycastHit hit;

    private DDSelection currentlyHovered;

    public UnityEngine.Events.UnityEvent<DDSelection> SomethingSelected;

    private void OnEnable()
    {
        SingletonHolder.Instance.Dungeon.PhaseChanged.AddListener(DungeonPhaseChanged);
    }

    private void OnDisable()
    {
        SingletonHolder.Instance.Dungeon.PhaseChanged.RemoveListener(DungeonPhaseChanged);
    }

    private void DungeonPhaseChanged(EDungeonPhase toPhase)
    {
        currentlyHovered = null;
    }

    private void Update()
    {
        r = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out hit, 1000, currentMask))
        {
            DDSelection mousedOver = hit.transform.GetComponent<DDSelection>();

            if (currentlyHovered != mousedOver)
            {
                if (currentlyHovered)
                {
                    currentlyHovered.Unhovered();
                }

                currentlyHovered = mousedOver;
                currentlyHovered.Hovered();
            }
        }
        else
        {
            if (currentlyHovered)
            {
                currentlyHovered.Unhovered();
                currentlyHovered = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentlyHovered)
            {
                SomethingSelected?.Invoke(currentlyHovered);
            }
        }
    }

    public Vector3 GetMousePos()
    {
        return r.origin + r.direction * (cam.transform.position.y - 1);
    }

    public void SetSelectionLayer(int layer)
    {
        currentMask = layer;
    }

    public void SetToPlayerCard()
    {
        SetSelectionLayer(1 << (int)Target.ETargetType.PlayerCard);
    }
}
