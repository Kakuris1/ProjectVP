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

            //Debug.Log(_targets.Count); // Ÿ���� ������ �þ߰� ������� �ʴ� ����

            // ���� Delivery/Impact�� _targets ����
            ctx.Spec.delivery.Deliver(in ctx, _targets);
            return n > 0;
        }
    }

}
