using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Impact/Damage")]
    public class DamageImpactAsset : ImpactAsset
    {
        public override void Apply(in SkillContext ctx, Transform target)
        {
            if (target.TryGetComponent<IDamageable>(out var hp))
            {
                hp.ApplyDamage(new DamagePayload
                {
                    amount = ctx.Spec.damage,
                    hitPoint = target.position,
                    source = ctx.Caster
                });
                if (ctx.Spec.hitVfx) ctx.Spawner?.SpawnOneShot(ctx.Spec.hitVfx, ctx.Spec.hitVfxSize, target.position, Quaternion.identity);
            }
        }
    }
}
