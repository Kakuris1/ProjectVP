using UnityEngine;
using System.Collections.Generic;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Instant")] // �����
    public class InstantDeliveryAsset : DeliveryAsset
    {
        [Tooltip("true�� ��� Ÿ�ٿ� ����, false�� ù Ÿ�ٿ��� ����")]
        public bool applyToAllTargets = true;

        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            // ĳ��Ʈ VFX (���� Ÿ�̹�)
            if (ctx.Spec.castVfx != null)
                ctx.Spawner?.SpawnOneShot(ctx.Spec.castVfx, ctx.Spec.castVfxSize, ctx.Origin, Quaternion.LookRotation(ctx.Direction));

            if (targets == null || targets.Count == 0) return;
            if (ctx.Spec.impacts == null || ctx.Spec.impacts.Length == 0) return;

            int count = applyToAllTargets ? targets.Count : 1;

            // ��� ����Ʈ�� �� Ÿ�ٿ� ����
            for (int i = 0; i < count; i++)
            {
                var t = targets[i];
                for (int j = 0; j < ctx.Spec.impacts.Length; j++)
                    ctx.Spec.impacts[j].Apply(ctx, t);
            }
        }
    }
}
