using UnityEngine;

namespace Combat.Skills
{
    public class EnemyHealth : HealthSystemBase
    {
        protected override void Awake() 
        { 
            base.Awake();
        }
        public override void ApplyDamage(DamagePayload dmg)
        {
            base .ApplyDamage(dmg);
        }
    }
}