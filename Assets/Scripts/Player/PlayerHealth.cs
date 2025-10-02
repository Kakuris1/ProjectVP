using Combat.Skills;
using UnityEngine;

public class PlayerHealth : HealthSystemBase
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override void ApplyDamage(DamagePayload dmg)
    {
        base.ApplyDamage(dmg);
    }
}
