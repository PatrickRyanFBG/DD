using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerSelector : MonoBehaviour
{
    [SerializeField]
    private Camera boardCam;
    [SerializeField]
    private Camera uiCam;

    [SerializeField]
    private LayerMask currentMask;

    private Ray boardRay;
    private Ray uiRay;
    private RaycastHit hit;

    private DDSelection currentlyHovered;

    public UnityEngine.Events.UnityEvent<DDSelection> SomethingSelected;

    private bool blocked;

    public void BlockClicks(bool shouldBlock)
    {
        blocked = shouldBlock;

        if(blocked)
        {
            if (currentlyHovered)
            {
                currentlyHovered.Unhovered();
                currentlyHovered = null;
            }
        }
    }

    private void OnEnable()
    {
        if (DDGamePlaySingletonHolder.Instance != null)
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.PhaseChanged.AddListener(DungeonPhaseChanged);
            DDGamePlaySingletonHolder.Instance.ShowDeckArea.OnClose.AddListener(() => { BlockClicks(false); });
        }
    }

    private void OnDisable()
    {
        if (DDGamePlaySingletonHolder.Instance != null)
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.PhaseChanged.RemoveListener(DungeonPhaseChanged);
        }
    }

    private void DungeonPhaseChanged(EDungeonPhase toPhase)
    {
        currentlyHovered = null;
    }

    private void Update()
    {
        if(blocked)
        {
            return;
        }

        bool camHitSomething = false;

        boardRay = boardCam.ScreenPointToRay(Input.mousePosition);
        LayerMask currentAndCameraMask = currentMask & boardCam.cullingMask;
        
        Debug.DrawLine(boardRay.origin, boardRay.origin + boardRay.direction * 1000, Color.cyan);

        if (Physics.Raycast(boardRay, out hit, 1000, currentAndCameraMask))
        {
            DDSelection mousedOver = hit.transform.GetComponent<DDSelection>();
            camHitSomething = true;

            if (currentlyHovered != mousedOver)
            {
                if (currentlyHovered)
                {
                    currentlyHovered.Unhovered();
                }


                if(mousedOver)
                {
                    if (mousedOver.Hovered())
                    {
                        currentlyHovered = mousedOver;
                    }
                }
            }
        }
        else
        {
            uiRay = uiCam.ScreenPointToRay(Input.mousePosition);
            currentAndCameraMask = currentMask & uiCam.cullingMask;

            Debug.DrawLine(uiRay.origin, uiRay.origin + uiRay.direction * 1000, Color.magenta);

            if (Physics.Raycast(uiRay, out hit, 1000, currentAndCameraMask))
            {
                DDSelection mousedOver = hit.transform.GetComponent<DDSelection>();
                camHitSomething = true;

                if (currentlyHovered != mousedOver)
                {
                    if (currentlyHovered)
                    {
                        currentlyHovered.Unhovered();
                    }

                    currentlyHovered = mousedOver;
                    
                    if (currentlyHovered)
                    {
                        currentlyHovered.Hovered();
                    }
                }
            }
        }

        if (!camHitSomething)
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

#if UNITY_EDITOR
    private void OnGUI()
    {
        if(hit.collider != null)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height - 50, 150, 50), hit.collider.gameObject.ToString());
        }
    }
#endif

    public Vector3 GetMousePos()
    {
        uiRay = uiCam.ScreenPointToRay(Input.mousePosition);
        return uiRay.origin + uiRay.direction * (uiCam.transform.position.y - 1);
    }

    public void SetSelectionLayer(int layer)
    {
        currentMask = layer;
    }

    public void SetToPlayerCard()
    {
        SetSelectionLayer(ETargetType.PlayerCard.GetLayer());
    }
}
