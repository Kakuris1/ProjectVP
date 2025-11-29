using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Cost/BT Flag Cooldown and Range")]
    public class BtFlagCostAsset : CostAsset
    {
        public override bool CheckAndConsume(in SkillContext ctx, float now, ref float nextReadyTime)
        {
            // 1. 쿨타임 체크 
            if (now < nextReadyTime) return false;

            // 2. 사거리 체크 
            Transform nearestTarget = ctx.TargetSensor.GetNearestTarget();
            if (nearestTarget == null) return false;
            if (!ctx.TargetSensor.IsNearestTargetInAttackRange(ctx.Spec.skillRange, ctx.Caster.position))
            {
                return false;
            }

            // 3. BT 플래그 체크
            if (ctx.Caster.TryGetComponent<EnemyInformation>(out var info))
            {
                // BT가 'CanAttack' 플래그를 false로 설정했다면
                if (!info.CanAttack)
                {
                    return false; // 승인 거부! (공격 실패)
                }
            }

            // 4. 모든 조건을 통과: 쿨타임 갱신 및 시전 승인
            nextReadyTime = now + ctx.Spec.cooldown;
            return true;
        }
    }
}