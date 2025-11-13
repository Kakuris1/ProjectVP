using UnityEngine;
using Combat.Skills;
using System.Collections.Generic;

// 이 스크립트는 투사체 프리팹에 붙어야 합니다.
// 또한, 프리팹에는 Rigidbody(IsKinematic=true)와 Collider(IsTrigger=true)가 필요합니다.
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("투사체 설정")]
    public float moveSpeed = 20f;
    [Tooltip("이 투사체가 영향을 줄 대상의 레이어")]
    public LayerMask targetLayer;
    [Tooltip("투사체의 실제 사거리 배율. (값 = SkillSpec의 skillRange * 이 계수)")]
    public float projectileRangeMultiplier = 1.5f; 

    // 내부 시스템
    private SkillContext _ctx;
    private bool _isInitialized = false;
    private float _distanceTraveled = 0f;
    private int _pierceCountLeft;
    private float _maxTravelDistance; // 최대 사거리를 저장할 변수

    // 이미 맞춘 대상을 저장 (중복 타격 방지)
    private HashSet<Transform> _targetsHit = new HashSet<Transform>();

    // 1. DeliveryAsset이 호출
    public void Initialize(in SkillContext ctx)
    {
        _ctx = ctx;
        _isInitialized = true;

        // 상태 리셋: 이동 거리 초기화
        _distanceTraveled = 0f;

        // SkillSpecAsset에서 관통 횟수를 가져옴
        _pierceCountLeft = _ctx.Spec.pierceCount;

        // 시전자는 절대 맞으면 안 됨
        _targetsHit.Clear();
        _targetsHit.Add(_ctx.Caster);

        _maxTravelDistance = _ctx.Spec.skillRange * this.projectileRangeMultiplier;

        // (안전 장치) 계수가 0 이하일 경우 skillRange로 고정
        if (_maxTravelDistance <= 0f)
        {
            _maxTravelDistance = _ctx.Spec.skillRange;
        }
    }

    // 2. 매 프레임 이동 및 사거리 체크
    void Update()
    {
        if (!_isInitialized) return;

        // 이동
        float deltaDistance = moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * deltaDistance);

        // 사거리 체크
        _distanceTraveled += deltaDistance;
        if (_distanceTraveled >= _maxTravelDistance)
        {
            _ctx.Spawner.Despawn(gameObject); // 사거리 도달 시 디스폰
        }
    }

    // 3. (실시간) 충돌 감지
    void OnTriggerEnter(Collider other)
    {
        if (!_isInitialized) return;

        // 1. 타겟 레이어가 맞는지 확인
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 2. 이미 맞춘 타겟인지 확인 (HashSet.Add는 중복이면 false 반환)
        if (!_targetsHit.Add(other.transform))
            return;

        // 3. Impact 적용
        ApplyImpactTo(other.transform);

        // 4. 관통 횟수(Pierce Count) 처리

        // pierceCount가 0이면 '무한 관통'이므로 파괴 로직을 건너뜀
        if (_pierceCountLeft == 0)
            return;

        // 관통 횟수 1 감소
        _pierceCountLeft--;

        // 남은 관통 횟수가 없으면
        if (_pierceCountLeft <= 0)
        {
            _ctx.Spawner.Despawn(gameObject); // 디스폰
        }
    }

    // 4. 실제 임팩트(대미지/힐) 적용
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