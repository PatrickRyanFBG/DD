using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDArtifactUI : MonoBehaviour
{
    [SerializeField]
    private RawImage icon;

    [SerializeField]
    private TMPro.TextMeshProUGUI description;

    private DDArtifactBase currentArtifact;

    public void SetUpArtifact(DDArtifactBase artifact)
    {
        currentArtifact = artifact;
        icon.texture = currentArtifact.Icon;
        description.text = currentArtifact.ArtifactName + "\r\n" + currentArtifact.Description;
    }

    public void StartHover()
    {
        description.enabled = true;
    }

    public void EndHover()
    {
        description.enabled = false;
    }
}
