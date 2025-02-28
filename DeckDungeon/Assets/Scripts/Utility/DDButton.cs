using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDButton : MonoBehaviour
{
    [SerializeField] private Button button;
    public Button Button => button;

    [SerializeField] private TMPro.TextMeshProUGUI buttonText;
    public TextMeshProUGUI ButtonText => buttonText;

    [HideInInspector] public UnityEngine.Events.UnityEvent<UnityEngine.EventSystems.BaseEventData> OnPointerEnter;

    public void EventTriggerPointerEnter(UnityEngine.EventSystems.BaseEventData data)
    {
        OnPointerEnter?.Invoke(data);
    }
}