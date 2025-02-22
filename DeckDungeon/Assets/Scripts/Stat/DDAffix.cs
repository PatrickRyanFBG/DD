using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DDAffix
{
    [SerializeField] private EAffixType affixType;
    public EAffixType AffixType => affixType;
    
    [SerializeField, Multiline] private string affixDescription;
    public string AffixDescription => affixDescription;

    [SerializeField] private Texture image;
    public Texture Image => image;

    [SerializeField] private bool existsAtZero;

    public bool ExistsAtZero => existsAtZero;

    [SerializeField] bool existsNegative;
    public bool ExistsNegative => existsNegative;

}