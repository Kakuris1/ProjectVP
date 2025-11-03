using Combat.Skills;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Targeting/NonTarget")]
public class NonTargetingAsset : TargetingAsset
{
    public override int AcquireTargets(in SkillContext ctx, List<Transform> targets)
    {
        targets.AddRange(ctx.TargetSensor.GetCurrentTargetList());
        return targets.Count;
    }
}
