using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDArtifactUI : MonoBehaviour
{
    [SerializeField] private RawImage icon;

    private DDArtifactBase currentArtifact;

    public void SetUpArtifact(DDArtifactBase artifact)
    {
        currentArtifact = artifact;
        icon.texture = currentArtifact.Icon;
    }

    public void StartHover()
    {
        DDGlobalManager.Instance.ToolTip.SetText(currentArtifact.ArtifactName + "\r\n" + currentArtifact.Description);
    }

    public void EndHover()
    {
        DDGlobalManager.Instance.ToolTip.SetText("");
    }
}