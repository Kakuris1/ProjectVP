using UnityEngine;
using Combat.Skills;
using System.Collections.Generic;

// 이 스크립트는 장판 프리팹에 붙어야 합니다.
// 또한, 프리팹에는 SphereCollider(IsTrigger=true)와 Rigidbody(IsKinematic=true)가 필요합니다.
[RequireComponent(typeof(Collider))]
public class GroundAoE : MonoBehaviour
{
    [Header("장판 설정")]
    [Tooltip("장판이 활성화된 후 총 지속되는 시간")]
    public float duration = 5.0f;
    [Tooltip("대미지/힐이 적용되는 주기 (초)")]
    public float tickRate = 1f;

    [Header("대상 필터링")]
    [Tooltip("이 장판이 영향을 줄 대상의 레이어")]
    public LayerMask targetLayer;

    // 내부 시스템
    private SkillContext _ctx;
    private bool _isInitialized = false;
    private bool _isActive = false;

    // 틱 주기 관리를 위해 내부에 있는 타겟과 마지막 틱 시간을 저장
    private Dictionary<Collider, float> _targetsInside = new Dictionary<Collider, float>();

    // 1. DeliveryAsset이 호출
    public void Initialize(in SkillContext ctx)
    {
        _ctx = ctx;
        _isInitialized = true;
    }

    // 2. 스폰 직후 실행
    void Start()
    {
        if (!_isInitialized)
        {
            Debug.LogError("GroundAoE가 Initialize되지 않았습니다!", this);
            Destroy(gameObject);
            return;
        }

        // 장판의 총 생명주기 설정
        Destroy(gameObject, duration + _ctx.Spec.skillDelay);

        // SkillSpec의 'skillDelay'를 "장판 활성화 대기 시간"으로 사용
        Invoke(nameof(ActivateAoE), _ctx.Spec.skillDelay);
    }

    // 3. skillDelay 이후 장판 활성화
    void ActivateAoE()
    {
        _isActive = true;
        // (필요시) 여기서 장판 활성화 이펙트를 따로 스폰할 수 있음
    }

    // 4. (실시간) 장판 범위에 '처음' 들어왔을 때
    void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return; // 아직 활성화 안됐으면 무시

        // 설정한 targetLayer와 일치하는지 확인
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 딕셔너리에 추가하고 '즉시' 1회 적용
        if (!_targetsInside.ContainsKey(other))
        {
            _targetsInside.Add(other, Time.time);
            ApplyImpactTo(other.transform);
        }
    }

    // 5. (실시간) 장판 범위에 '머무르는' 동안
    void OnTriggerStay(Collider other)
    {
        if (!_isActive) return;

        // 딕셔너리에 있는지 (Enter가 호출됐었는지) 확인
        if (_targetsInside.TryGetValue(other, out float lastTickTime))
        {
            // 현재 시간이 (마지막 틱 시간 + 틱 주기)를 넘겼다면
            if (Time.time > lastTickTime + tickRate)
            {
                _targetsInside[other] = Time.time; // 마지막 틱 시간 갱신
                ApplyImpactTo(other.transform); // 임팩트 적용
            }
        }
    }

    // 6. (실시간) 장판 범위에서 '나갔을' 때
    void OnTriggerExit(Collider other)
    {
        // 딕셔너리에서 제거
        _targetsInside.Remove(other);
    }

    // 7. 실제 임팩트(대미지/힐) 적용
    void ApplyImpactTo(Transform target)
    {
        if (_ctx.Spec.impacts == null) return;

        // SkillSpec에 등록된 모든 Impact를 순차적으로 적용
        for (int i = 0; i < _ctx.Spec.impacts.Length; i++)
        {
            if (_ctx.Spec.impacts[i] != null)
            {
                _ctx.Spec.impacts[i].Apply(_ctx, target);
            }
        }
    }
}