using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDArtifactBase : DDScriptableObject
{
    [SerializeField]
    private string artifactName;
    public string ArtifactName { get => artifactName; }

    [SerializeField, Multiline]
    private string description;
    public string Description { get => description; }

    [SerializeField]
    private Texture icon;
    public Texture Icon { get => icon; }

    public abstract void Equipped();
}
