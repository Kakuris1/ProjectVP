using UnityEngine;
using System.Collections.Generic;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Projectile")] // 투사체형
    public class ProjectileDeliveryAsset : DeliveryAsset
    {
        [Header("Projectile")]
        public GameObject projectilePrefab;
        public float speed = 18f;
        public float lifetime = 5f;

        [Header("Firing")]
        [Tooltip("true면 각 타겟을 조준해 1발씩 쏨, false면 전방으로만 N발(=1 추천)")]
        public bool aimAtEachTarget = true;

        [Tooltip("최대 발사 수(aimAtEachTarget=true면 타겟 수와 min 처리)")]
        public int maxProjectiles = 1;

        [Header("Hit")]
        public LayerMask hitMask = ~0;
        public bool destroyOnHit = true;

        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            if (!projectilePrefab) return;

            // 캐스트 VFX
            if (ctx.Spec.castVfx != null)
                ctx.Spawner?.SpawnOneShot(ctx.Spec.castVfx, ctx.Origin, Quaternion.LookRotation(ctx.Direction));

            if (aimAtEachTarget && targets != null && targets.Count > 0)
            {
                int count = Mathf.Min(maxProjectiles <= 0 ? targets.Count : maxProjectiles, targets.Count);
                for (int i = 0; i < count; i++)
                {
                    var t = targets[i];
                    var dir = (t.position - ctx.Origin);
                    if (dir.sqrMagnitude < 0.0001f) dir = ctx.Direction;
                    SpawnOne(ctx, dir.normalized);
                }
            }
            else
            {
                // 전방으로만 발사 (maxProjectiles만큼)
                int count = Mathf.Max(1, maxProjectiles);
                for (int i = 0; i < count; i++)
                    SpawnOne(ctx, ctx.Direction);
            }
        }

        void SpawnOne(in SkillContext ctx, Vector3 dir)
        {
            var rot = Quaternion.LookRotation(dir);
            var go = ctx.Spawner?.Spawn(projectilePrefab, ctx.Origin, rot);
            if (!go) return;

            if (!go.TryGetComponent<Projectile>(out var proj))
                proj = go.AddComponent<Projectile>(); // 안전장치

            proj.Init(ctx, dir, speed, lifetime, hitMask, destroyOnHit);
        }
    }
}
