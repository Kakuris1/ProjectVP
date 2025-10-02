using Combat.Skills;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Targeting/NonTarget")]
public class NonTargetingAsset : TargetingAsset
{
    public float range = 2f;
    public float angleDeg = 60f;
    public LayerMask mask;

    static Collider[] _buf = new Collider[64];

    public override int AcquireTargets(in SkillContext ctx, List<Transform> results)
    {
        results.Clear();
        int n = Physics.OverlapSphereNonAlloc(ctx.Origin, range, _buf, mask);
        // ����/���� ���͸� �߰�...
        // results.Add(...)
        return results.Count;
    }
}
