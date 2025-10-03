using UnityEngine;
using System.Collections.Generic;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Instant")] // 즉발형
    public class InstantDeliveryAsset : DeliveryAsset
    {
        [Tooltip("true면 모든 타겟에 적용, false면 첫 타겟에만 적용")]
        public bool applyToAllTargets = true;

        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            // 캐스트 VFX (시전 타이밍)
            if (ctx.Spec.castVfx != null)
                ctx.Spawner?.SpawnOneShot(ctx.Spec.castVfx, ctx.Spec.castVfxSize, ctx.Origin, Quaternion.LookRotation(ctx.Direction));

            if (targets == null || targets.Count == 0) return;
            if (ctx.Spec.impacts == null || ctx.Spec.impacts.Length == 0) return;

            int count = applyToAllTargets ? targets.Count : 1;

            // 모든 임팩트를 각 타겟에 적용
            for (int i = 0; i < count; i++)
            {
                var t = targets[i];
                for (int j = 0; j < ctx.Spec.impacts.Length; j++)
                    ctx.Spec.impacts[j].Apply(ctx, t);
            }
        }
    }
}
