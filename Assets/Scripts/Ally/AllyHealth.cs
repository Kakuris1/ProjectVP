using Combat.Skills;
using UnityEngine;

[RequireComponent(typeof(AllyInformation))]
public class AllyHealth : HealthSystemBase
{
    private AllyInformation info; // AllyInformation 참조

    protected override void Awake()
    {
        base.Awake();

        // DataHub를 AllyInformation으로 캐스팅
        info = DataHub as AllyInformation;

        DataHub.OnDeath += HandleAllyDeath;
    }

    private void OnDestroy()
    {
        if (DataHub != null)
        {
            DataHub.OnDeath -= HandleAllyDeath;
        }
    }

    public override void ApplyDamage(DamagePayload dmg)
    {
        // 1. 기본 ApplyDamage (HealthSystemBase)를 호출해 체력을 깎습니다.
        base.ApplyDamage(dmg);

        // 2. info가 있고, 아직 죽지 않았다면
        if (info != null && !info.IsDead)
        {
            // 3. 나를 때린 대상(dmg.source)이 존재한다면
            if (dmg.source != null)
            {
                // 4. 나의 타겟을 그 대상으로 즉시 변경합니다.
                // (AllyInformation에 SetTarget이 EnemyInformation처럼 구현되어 있어야 함)
                info.SetTarget(dmg.source);
            }
        }
    }

    // Ally 사망 처리 로직
    private void HandleAllyDeath()
    {
        TeamManager.Instance.RemoveAlly(DataHub as AllyInformation);
    }
}