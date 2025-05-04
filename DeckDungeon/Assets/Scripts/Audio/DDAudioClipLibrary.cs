using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Not the biggest fan of this way of collecting them
public class DDAudioClipLibrary : DDScriptableObject
{
    [FormerlySerializedAs("melee")] public DDAudioClip Melee;
    [FormerlySerializedAs("ranged")] public DDAudioClip Ranged;
    [FormerlySerializedAs("bonk")] public DDAudioClip Bonk;

    [FormerlySerializedAs("drawCard")] public DDAudioClip DrawCard;
    [FormerlySerializedAs("hoverCard")] public DDAudioClip HoverCard;
    [FormerlySerializedAs("selectCard")] public DDAudioClip SelectCard;
    [FormerlySerializedAs("discardCard")] public DDAudioClip DiscardCard;

    [FormerlySerializedAs("hoverTarget")] public DDAudioClip HoverTarget;
    [FormerlySerializedAs("selectTarget")] public DDAudioClip SelectTarget;
}