// MeleeDeliveryAsset.cs
using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Melee")]
    public class MeleeDeliveryAsset : DeliveryAsset
    {
        [Header("Melee Shape")]
        public float radius = 0.5f;               // 반경(월드)
        public float angleDeg = 60f;              // 전방 원뿔 각도(도). 0이면 원형
        public LayerMask hitMask = ~0;

        [Header("Hit options")]
        public bool hitAll = true;                // true: 모든 적에 적용, false: 첫 대상만
        public bool requireLineOfSight = false;   // true면 레이캐스트로 가시성 체크

        // 내부 버퍼(NonAlloc)
        static readonly Collider[] _overlapBuf = new Collider[64];

        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            // cast VFX
            if (ctx.Spec.castVfx) ctx.Spawner?.SpawnOneShot(ctx.Spec.castVfx, ctx.Spec.castVfxSize, ctx.Origin, Quaternion.LookRotation(-ctx.Direction));

            float skillRange = ctx.Spec.skillRange;

            // OverlapSphereNonAlloc을 써서 충돌 후보 수집
            int n = Physics.OverlapSphereNonAlloc(ctx.Origin, skillRange + radius, _overlapBuf, hitMask);
            if (n <= 0) return;

            int applied = 0;
            Vector3 forward = ctx.Direction.normalized;
            Vector3 origin = ctx.Origin;

            for (int i = 0; i < n; i++)
            {
                var col = _overlapBuf[i];
                if (col == null) continue;
                var t = col.transform;

                // 거리 체크 (원형/반경)
                // 1. 시전자(origin)에서 가장 가까운 타겟 콜라이더의 '가장자리' 지점을 찾음
                Vector3 closestPoint = col.ClosestPoint(origin);

                // 2. '가장자리'까지의 실제 방향과 거리 계산
                Vector3 dirToTarget = closestPoint - origin;
                float distance = dirToTarget.magnitude;

                // 3. 거리 체크: 이 스킬의 '공식 사거리' (skillRange) 이내인지 확인
                if (distance > skillRange) continue;

                // 전방 원뿔 체크 (angle 기준)
                if (angleDeg > 0f)
                {
                    Vector3 toTarget = (t.position - origin).normalized;
                    float cos = Vector3.Dot(forward, toTarget);
                    float cosThreshold = Mathf.Cos(Mathf.Deg2Rad * (angleDeg * 0.5f));
                    if (cos < cosThreshold) continue;
                }

                // optional: 가시성 검사
                if (requireLineOfSight)
                {
                    var dir = (t.position - origin);
                    if (Physics.Raycast(origin, dir.normalized, out var hitInfo, dir.magnitude, ~0, QueryTriggerInteraction.Ignore))
                    {
                        if (hitInfo.collider != col) continue; // 뭔가 가로막았음
                    }
                }

                // 임팩트 적용
                if (ctx.Spec.impacts != null)
                {
                    for (int j = 0; j < ctx.Spec.impacts.Length; j++)
                        ctx.Spec.impacts[j].Apply(ctx, t);
                }

                applied++;
                if (!hitAll) break;
            }
        }
    }
}
