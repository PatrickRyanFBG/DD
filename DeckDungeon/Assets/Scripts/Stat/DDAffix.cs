using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DDAffix
{
    [SerializeField] private EAffixType affixType;
    public EAffixType AffixType => affixType;

    [SerializeField] private Texture image;
    public Texture Image => image;

    [FormerlySerializedAs("existsAtOrBelowZero")] [SerializeField]
    private bool existsAtZero;

    public bool ExistsAtZero => existsAtZero;

    private bool existsNegative;
    public bool ExistsNegative => existsNegative;
}