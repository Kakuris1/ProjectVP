using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Targeting/Nearest")]
    public class NearestTargetTargetingAsset : TargetingAsset
    {
        public float range = 25f;
        public LayerMask mask;
        private static readonly Collider[] _buf = new Collider[64]; // 읽기 전용 버퍼(정적 공유)

        public override int AcquireTargets(in SkillContext ctx, List<Transform> results)
        {
            int n = Physics.OverlapSphereNonAlloc(ctx.Origin, range, _buf, mask);

            Transform nearest = null; float best = float.MaxValue;
            for (int i = 0; i < n; i++)
            {
                var t = _buf[i].transform;
                float d = (t.position - ctx.Origin).sqrMagnitude;
                if (d < best) { best = d; nearest = t; }
            }
            if (nearest != null) results.Add(nearest);
            return results.Count;
        }
    }
}
