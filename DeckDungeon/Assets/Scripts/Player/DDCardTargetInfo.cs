using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCardTargetInfo
{
   [SerializeField] protected ETargetType targetType;
   public ETargetType TargetType => targetType;

   [SerializeField] protected bool differentTarget;
   public bool DifferentTarget => differentTarget;

   [SerializeField] protected bool useLastTarget;
   public bool UseLastTarget => useLastTarget;

   public DDCardTargetInfo()
   {
      
   }

   public DDCardTargetInfo(ETargetType targetType)
   {
      this.targetType = targetType;
   }
   
   public DDCardTargetInfo(ETargetType targetType, bool differentTarget)
   {
      this.targetType = targetType;
      this.differentTarget = differentTarget;
   }
   
   public DDCardTargetInfo(ETargetType targetType, bool differentTarget, bool useLastTarget)
   {
      this.targetType = targetType;
      this.differentTarget = differentTarget;
      this.useLastTarget = useLastTarget;
   }
}
