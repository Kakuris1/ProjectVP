using UnityEngine;

[RequireComponent(typeof(AllyInformation))]
public class AllyHealth : HealthSystemBase
{
    protected override void Awake()
    {
        base.Awake();

        DataHub.OnDeath += HandleAllyDeath;
    }

    private void OnDestroy()
    {
        if (DataHub != null)
        {
            DataHub.OnDeath -= HandleAllyDeath;
        }
    }

    // Ally 사망 처리 로직
    private void HandleAllyDeath()
    {
        TeamManager.Instance.RemoveAlly(DataHub as AllyInformation);
    }
}

