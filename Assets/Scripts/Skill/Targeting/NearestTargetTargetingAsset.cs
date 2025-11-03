using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Targeting/Nearest")]
    public class NearestTargetTargetingAsset : TargetingAsset
    {
        public override int AcquireTargets(in SkillContext ctx, List<Transform> targets)
        {
            targets.Add(ctx.TargetSensor.GetNearestTarget());
            return targets.Count; 
        }
    }
}
