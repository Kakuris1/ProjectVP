using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Cost/Cooldown and Range")]
    public class CooldownAndRangeCostAsset : CostAsset
    {
        public override bool CheckAndConsume(in SkillContext ctx, float now, ref float nextReadyTime)
        {
            if (now < nextReadyTime) return false; // 쿨타임 체크

            // 센서가 제공하는 '가장 가까운' 타겟을 가져옵니다.
            Transform nearestTarget = ctx.TargetSensor.GetNearestTarget();

            // [조건 A] 타겟이 아예 없는가?
            if (nearestTarget == null) return false;

            // [조건 B] 타겟이 스킬의 '공식 사거리' 밖에 있는가?
            if (!ctx.TargetSensor.IsNearestTargetInAttackRange(ctx.Spec.skillRange, ctx.Caster.position))
            {
                return false; // (실패) 사거리 밖임. 쿨타임 돌지 않음.
            }

            // 소모 값이 있을 경우 추가 해야함

            // 쿨타임 갱신
            nextReadyTime = now + ctx.Spec.cooldown;
            return true;
        }
    }
}