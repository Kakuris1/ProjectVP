using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    public class SkillPipeline : MonoBehaviour
    {
        private readonly List<Transform> _targets = new(32);
        public bool Execute(in SkillContext ctx)
        {
            _targets.Clear();
            var n = ctx.Spec.targeting.AcquireTargets(in ctx, _targets);

            // 스킬 사용 로그 출력용
            Debug.Log($"{ctx.Caster.name}가 스킬을 사용했습니다. ({ctx.Spec.skillName})");

            // 이후 Delivery/Impact에 _targets 전달
            ctx.Spec.delivery.Deliver(in ctx, _targets);
            return n > 0;
        }
    }

}
