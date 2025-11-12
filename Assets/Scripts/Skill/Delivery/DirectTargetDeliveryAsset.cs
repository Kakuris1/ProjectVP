using UnityEngine;
using System.Collections.Generic;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Instant")] // 즉발형
    public class DirectTargetDeliveryAsset : DeliveryAsset
    {
        [Tooltip("true: SkillVfx를 타겟마다 스폰 (낙뢰 등) / false: 타겟 리스트의 첫번째 타겟 위치에 1개만 스폰 (장판 등)")]
        public bool spawnVfxOnEachTarget = true;
        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            if (targets == null || targets.Count == 0) return;

            // 캐스트 VFX (시전 타이밍)
            if (ctx.Spec.castVfx != null)
                ctx.Spawner?.SpawnOneShot(ctx.Spec.castVfx, ctx.Spec.castVfxSize, ctx.Origin, Quaternion.LookRotation(ctx.Direction));


            // 타겟 수 계산
            int count = (ctx.Spec.maxTargetCount <= 0) ? targets.Count : Mathf.Min(targets.Count, ctx.Spec.maxTargetCount);

            // 타겟 위치에 SkillVfx 스폰 (즉시)
            if (ctx.Spec.skillVfx != null)
            {
                if (spawnVfxOnEachTarget)
                {
                    // 모든 타겟 위치에 VFX 스폰 (예: 연쇄 낙뢰)
                    for (int i = 0; i < count; i++)
                    {
                        var t = targets[i];
                        if (t != null)
                            ctx.Spawner?.SpawnOneShot(ctx.Spec.skillVfx, ctx.Spec.skillVfxSize, t.position, Quaternion.identity);
                    }
                }
                else if (targets[0] != null)
                {
                    // 첫 번째 타겟 위치에만 VFX 1개 스폰 (예: 단일 장판)
                    ctx.Spawner?.SpawnOneShot(ctx.Spec.skillVfx, ctx.Spec.skillVfxSize, targets[0].position, Quaternion.identity);
                }
            }

            // 스킬 딜레이 체크
            if (ctx.Spec.skillDelay > 0f)
            {
                // 딜레이가 있다면, DelayedImpactManager를 동적 스폰
                GameObject helper = new GameObject($"DelayedImpact_Helper ({ctx.Spec.skillName})");
                var manager = helper.AddComponent<DelayedImpactManager>();

                // 헬퍼에게 SkillContext와 타겟 리스트(사본 생성됨)를 전달
                manager.Initialize(ctx, targets);
            }
            else
            {
                // 딜레이가 0이면, 즉시 임팩트 적용
                ApplyImpactsImmediately(in ctx, targets, count);
            }
        }

        private void ApplyImpactsImmediately(in SkillContext ctx, List<Transform> targets, int count)
        {
            if (ctx.Spec.impacts == null || ctx.Spec.impacts.Length == 0) return;

            for (int i = 0; i < count; i++)
            {
                var t = targets[i];
                if (t == null) continue;
                for (int j = 0; j < ctx.Spec.impacts.Length; j++)
                {
                    if (ctx.Spec.impacts[j] != null)
                    {
                        ctx.Spec.impacts[j].Apply(ctx, t);
                    }
                    else
                    {
                        Debug.LogError($"SkillSpec 의 'Impacts' 배열 {j}번째 요소가 비어있습니다!", ctx.Caster);
                    }
                }
            }
        }
    }
}
