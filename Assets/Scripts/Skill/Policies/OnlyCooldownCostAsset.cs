using UnityEngine;
namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Cost/OnlyCooldown")]
    public class OnlyCooldownCostAsset : CostAsset
    {
        public override bool CheckAndConsume(in SkillContext ctx, float now, out float nextReadyTime)
        {
            nextReadyTime = now + ctx.Spec.cooldown;
            return true;
        }
    }
}
