using System;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Mouse Lunge")]
    public class MouseLungeDeliveryAsset : DeliveryAsset
    {
        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            // 타겟이 없으면(TargetingAsset이 실패했으면) 아무것도 안 함
            if (targets == null || targets.Count == 0 || targets[0] == null) return;

            Transform target = targets[0]; // 가장 가까운 타겟 1명

            // 스킬 시전자(Caster)의 'EnemyMovement' 컴포넌트
            EnemyMovement movement = ctx.Caster.GetComponent<EnemyMovement>();
            if (movement == null)
            {
                Debug.LogError($"'{ctx.Caster.name}'에 EnemyMovement가 없습니다!", ctx.Caster);
                return;
            }
            EnemyController controller = ctx.Caster.GetComponent<EnemyController>();

            // 'in' 매개변수인 'ctx'를 캡처할 수 없으므로,
            // 로컬 사본(struct 복사)을 만듬
            SkillContext ctx_localCopy = ctx;

            // "박치기가 적중하는 순간, 이 로직을 실행하세요" 라는 '콜백'을 만듬
            Action onHitAction = () =>
            {
                if (ctx_localCopy.Spec.impacts != null)
                {
                    for (int j = 0; j < ctx_localCopy.Spec.impacts.Length; j++)
                    {
                        ctx_localCopy.Spec.impacts[j].Apply(ctx_localCopy, target);
                    }
                }
                // 공격 횟수 카운트 +1
                controller?.IncrementLungeCounter();
            };

            // EnemyMovement에게 "이 타겟을 향해, 이 콜백을 가지고 박치기해!"라고 명령
            movement.PerformLungeAttack(target, onHitAction);
        }
    }
}