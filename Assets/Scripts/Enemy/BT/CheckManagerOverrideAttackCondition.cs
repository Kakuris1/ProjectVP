using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckManagerOverrideAttackCondition", story: "CheckManagerOverrideAttackCondition", category: "Conditions", id: "05c505697c8acde484b85becddb34272")]
public partial class CheckManagerOverrideAttackCondition : Condition
{

    public override bool IsTrue()
    {
        return !MouseManager.Instance.overrideKitingAndAttack;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
