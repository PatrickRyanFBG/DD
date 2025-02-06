using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDAffix
{
    [SerializeField]
    private EAffixType affixType;
    public EAffixType AffixType => affixType;

    [SerializeField]
    private Texture image;
    public Texture Image => image;

    [SerializeField]
    private bool existsAtOrBelowZero;
    public bool ExistsAtOrBelowZero => existsAtOrBelowZero;
}
