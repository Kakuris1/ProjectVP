using UnityEngine;

namespace Combat.Skills
{
    public class EnemyHealth : HealthSystemBase
    {
        private EnemyInformation info;
        protected override void Awake() 
        { 
            base.Awake();
            info = DataHub as EnemyInformation;
        }
        public override void ApplyDamage(DamagePayload dmg)
        {
            base .ApplyDamage(dmg);

            if (info != null)
            {
                // 피격 즉시 적대적으로 변경
                info.Hostile = true;

                // 나를 때린 유닛(dmg.source)으로 타겟을 즉시 갱신
                if (dmg.source != null)
                {
                    info.SetTarget(dmg.source);
                }
            }
        }
    }
}