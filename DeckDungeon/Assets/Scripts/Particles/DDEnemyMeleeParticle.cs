using System;
using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class DDEnemyMeleeParticle : DDParticleBase
{
    [SerializeField] private RawImage slashOne;
    [SerializeField] private RawImage slashTwo;
    
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    public override IEnumerator Play()
    {
        LMotion.Create(startColor, endColor, .33f).WithEase(Ease.InSine).BindToColor(slashOne);
        yield return new WaitForSeconds(.25f);
        LMotion.Create(startColor, endColor, .33f).WithEase(Ease.InSine).BindToColor(slashTwo);
        yield return new WaitForSeconds(.25f);
        LMotion.Create(endColor, startColor, .33f).WithEase(Ease.OutSine).BindToColor(slashOne);
        yield return new WaitForSeconds(.25f);
        MotionHandle handle = LMotion.Create(endColor, startColor, .33f).WithEase(Ease.OutSine).BindToColor(slashTwo);
        yield return handle.ToYieldInstruction();
        Destroy(gameObject);
    }
}
