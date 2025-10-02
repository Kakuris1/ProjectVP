using Combat.Skills;
using UnityEngine;

public abstract class HealthSystemBase : MonoBehaviour, IDamageable
{
    protected IUnitDataHub DataHub { get; private set; }

    protected virtual void Awake()
    {
        DataHub = GetComponent<IUnitDataHub>();
        if (DataHub == null)
        {
            Debug.LogError($"이 오브젝트({name})에 IHealthDataHub가 없습니다!");
        }
    }

    // ImpactAsset이 이 메서드를 호출
    public virtual void ApplyDamage(DamagePayload payload)
    { 
        if (DataHub == null || DataHub.IsDead) return;

        // 후에 추가 될 공용 로직 작성 (계수 등)
        float finalDamage = payload.amount;

        DataHub.Internal_TakeDamage(finalDamage);
    }
}

