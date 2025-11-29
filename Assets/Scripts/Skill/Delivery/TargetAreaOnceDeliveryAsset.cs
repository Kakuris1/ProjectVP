using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Target Area Once")]
    public class TargetAreaOnceDeliveryAsset : DeliveryAsset
    {
        [Tooltip("폭발 피해를 입힐 대상 레이어 (ImpactAsset이 아닌 DeliveryAsset에서 필터링)")]
        public LayerMask hitMask = ~0;
        [Header("스킬 범위")]
        public float skillAreaRange = 3f;

        // OverlapSphereNonAlloc용 내부 버퍼 (재활용) [cite: MeleeDeliveryAsset.cs]
        static readonly Collider[] _overlapBuf = new Collider[128];

        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            // 1. 타겟이 없으면 중단
            if (targets == null || targets.Count == 0 || targets[0] == null) return;

            // 2. 시전자 위치에 CastVfx 스폰
            Vector3 spawnPos = ctx.Origin + ctx.Direction * 1.5f;
            if (ctx.Spec.castVfx != null)
                ctx.Spawner?.SpawnOneShot(ctx.Spec.castVfx, ctx.Spec.castVfxSize, spawnPos, Quaternion.LookRotation(ctx.Direction), 1f);

            // 3. 폭발 중심점 = 첫 번째 타겟의 위치
            Vector3 centerPoint = targets[0].position;

            // 4. 폭발 중심점에 SkillVfx (폭발 이펙트) 스폰
            if (ctx.Spec.skillVfx)
                ctx.Spawner?.SpawnOneShot(ctx.Spec.skillVfx, ctx.Spec.skillVfxSize, centerPoint, Quaternion.identity, 5f); // 5초 뒤 자동 파괴 (이펙트 길이에 맞게 조절)

            // 5. 딜레이 체크
            if (ctx.Spec.skillDelay > 0f)
            {
                // 딜레이가 있다면, DelayedAreaImpactHelper를 동적 스폰
                GameObject helper = new GameObject($"DelayedAreaImpact_Helper ({ctx.Spec.skillName})");
                var manager = helper.AddComponent<DelayedAreaImpactHelper>();

                // 헬퍼에게 SkillContext와 '폭발 중심점'을 전달
                manager.Initialize(ctx, centerPoint, skillAreaRange);
            }
            else
            {
                // 딜레이가 0이면, 즉시 폭발 적용
                ApplyAreaImpactImmediately(in ctx, centerPoint);
            }
        }

        // 딜레이 0일 때 사용할 즉시 적용 로직
        private void ApplyAreaImpactImmediately(in SkillContext ctx, Vector3 centerPoint)
        {
            float radius = skillAreaRange;
            int hitCount = Physics.OverlapSphereNonAlloc(centerPoint, radius, _overlapBuf, (int)hitMask);
            if (hitCount == 0) return;

            for (int i = 0; i < hitCount; i++)
            {
                var col = _overlapBuf[i];
                if (col == null || col.transform == ctx.Caster) continue;

                if (ctx.Spec.impacts != null)
                {
                    for (int j = 0; j < ctx.Spec.impacts.Length; j++)
                    {
                        if (ctx.Spec.impacts[j] != null)
                        {
                            ctx.Spec.impacts[j].Apply(ctx, col.transform);
                        }
                    }
                }
            }
        }
    }
}