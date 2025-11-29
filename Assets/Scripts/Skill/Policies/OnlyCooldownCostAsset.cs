using UnityEngine;
namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Cost/OnlyCooldown")]
    public class OnlyCooldownCostAsset : CostAsset
    {
        public override bool CheckAndConsume(in SkillContext ctx, float now, ref float nextReadyTime)
        {
            if (now < nextReadyTime) return false; // ÄðÅ¸ÀÓ Ã¼Å©

            nextReadyTime = now + ctx.Spec.cooldown; // ÄðÅ¸ÀÓ °»½Å
            return true;
        }
    }
}
