using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    public Button Button => button;

    [SerializeField]
    private TMPro.TextMeshProUGUI buttonText;

    [HideInInspector]
    public UnityEngine.Events.UnityEvent<UnityEngine.EventSystems.BaseEventData> OnPointerEnter;

    public void EventTriggerPointerEnter(UnityEngine.EventSystems.BaseEventData data)
    {
        OnPointerEnter?.Invoke(data);
    }
}
