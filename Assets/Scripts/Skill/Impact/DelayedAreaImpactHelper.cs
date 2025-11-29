using UnityEngine;
using Combat.Skills;

// 1회성 광역 폭발을 '딜레이' 이후 실행하는 헬퍼
public class DelayedAreaImpactHelper : MonoBehaviour
{
    private SkillContext _ctx;
    private Vector3 _centerPoint;
    private float _skillAreaRange;
    private bool _isInitialized = false;

    // OverlapSphereNonAlloc용 내부 버퍼 (재활용)
    static readonly Collider[] _overlapBuf = new Collider[128];

    // 1. DeliveryAsset이 호출
    public void Initialize(in SkillContext ctx, Vector3 centerPoint, float skillAreaRange)
    {
        _ctx = ctx;
        _centerPoint = centerPoint;
        _skillAreaRange = skillAreaRange;
        _isInitialized = true;
    }

    // 2. 씬에 스폰된 직후 실행
    void Start()
    {
        if (!_isInitialized)
        {
            Destroy(gameObject); // 초기화 안됐으면 자폭
            return;
        }

        // 3. skillDelay만큼 대기 후 ApplyAreaImpact 실행
        Invoke(nameof(ApplyAreaImpact), _ctx.Spec.skillDelay);
    }

    // 4. 딜레이 시간 경과 후 실행
    void ApplyAreaImpact()
    {
        if (_ctx.Caster == null) // 시전자가 사라졌으면 중단
        {
            _ctx.Spawner.Despawn(gameObject);
            return;
        }

        // 5. 폭발 반경 = SkillSpec의 skillRange
        float radius = _skillAreaRange;

        // 6. OverlapSphere로 범위 내 모든 콜라이더 수집
        int hitCount = Physics.OverlapSphereNonAlloc(_centerPoint, radius, _overlapBuf);
        if (hitCount == 0)
        {
            _ctx.Spawner.Despawn(gameObject);
            return;
        }

        // 7. 감지된 모든 대상에게 Impact 적용
        for (int i = 0; i < hitCount; i++)
        {
            var col = _overlapBuf[i];
            if (col == null) continue;

            // 시전자는 맞지 않도록 예외 처리
            if (col.transform == _ctx.Caster) continue;

            if (_ctx.Spec.impacts != null)
            {
                for (int j = 0; j < _ctx.Spec.impacts.Length; j++)
                {
                    if (_ctx.Spec.impacts[j] != null)
                    {
                        _ctx.Spec.impacts[j].Apply(_ctx, col.transform);
                    }
                }
            }
        }

        // 8. 임무 완수 후 스스로 파괴(반납)
        _ctx.Spawner.Despawn(gameObject);
    }
}