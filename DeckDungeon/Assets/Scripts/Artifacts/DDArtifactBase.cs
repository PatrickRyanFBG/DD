using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDArtifactBase : DDScriptableObject
{
    [SerializeField] private string artifactName;
    public string ArtifactName => artifactName;

    [SerializeField, Multiline] private string description;
    public string Description => description;

    [SerializeField] private Texture icon;
    public Texture Icon => icon;

    public abstract void Equipped();

    public abstract void Unequipped();

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(artifactName))
        {
#if UNITY_EDITOR
            artifactName = name;
            UnityEditor.EditorUtility.SetDirty(this);
#else
            artifactName = "MISSING NAME";
#endif
        }
    }
}