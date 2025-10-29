using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Targeting/Nearest")]
    public class NearestTargetTargetingAsset : TargetingAsset
    {
        public override int AcquireTargets(in SkillContext ctx, List<Transform> results)
        {
            results = ctx.TargetSensor.GetCurrentTargetList();
            Transform nearest = ctx.TargetSensor.GetNearestTarget();
            return results.Count; 
        }
    }
}
