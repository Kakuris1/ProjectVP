using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    public class SkillPipeline : MonoBehaviour
    {
        // �ν��Ͻ� �ʵ�� 1���� ĳ�� �� �� ������������ ���� '�� ĳ����' �����̶� �浹 ����
        private readonly List<Transform> _targets = new(32);

        public bool Execute(in SkillContext ctx)
        {
            _targets.Clear();
            var n = ctx.Spec.targeting.AcquireTargets(in ctx, _targets);

            // ���� Delivery/Impact�� _targets ����
            ctx.Spec.delivery.Deliver(in ctx, _targets);  // IReadOnlyList�� �����ص� ����
            return n > 0;
        }
    }

}
