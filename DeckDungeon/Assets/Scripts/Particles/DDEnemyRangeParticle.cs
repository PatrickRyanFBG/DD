using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class DDEnemyRangeParticle : DDParticleBase
{
    [HideInInspector] public Vector3 GoalPosition;
    
    public override IEnumerator Play()
    {
        MotionHandle handle = LMotion.Create(transform.position, GoalPosition, 1f).BindToPosition(transform);
        yield return handle.ToYieldInstruction();
        Destroy(gameObject);
    }
}
