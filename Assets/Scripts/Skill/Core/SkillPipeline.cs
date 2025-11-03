using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    public class SkillPipeline : MonoBehaviour
    {
        // 인스턴스 필드로 1개만 캐시 → 이 파이프라인을 쓰는 '한 캐릭터' 전용이라 충돌 없음
        private readonly List<Transform> _targets = new(32);

        public bool Execute(in SkillContext ctx)
        {
            _targets.Clear();
            var n = ctx.Spec.targeting.AcquireTargets(in ctx, _targets);

            //Debug.Log(_targets.Count);

            // 이후 Delivery/Impact에 _targets 전달
            ctx.Spec.delivery.Deliver(in ctx, _targets);
            return n > 0;
        }
    }

}
