using UnityEngine;
using Combat.Skills;
using System.Collections.Generic;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DrillProjectile : MonoBehaviour, IProjectileLogic
{
    [Header("투사체 설정")]
    public float moveSpeed = 10f;
    [Tooltip("지속 피해 주기 (초)")]
    public float tickRate = 0.2f;
    [Tooltip("이 투사체가 영향을 줄 대상의 레이어")]
    public LayerMask targetLayer;
    [Tooltip("투사체를 '막을' 대상의 레이어 (예: Environment")]
    public LayerMask obstacleLayer;

    // 내부 시스템
    private SkillContext _ctx;
    private bool _isInitialized = false;
    private float _distanceTraveled = 0f;
    private float _maxTravelDistance;
    [Tooltip("투사체의 실제 사거리 배율. (값 = SkillSpec의 skillRange * 이 계수)")]
    public float projectileRangeMultiplier = 1.5f;

    // 지속 피해를 입히고 있는 타겟과 마지막 틱 시간 저장
    private Dictionary<Collider, float> _targetsInside = new Dictionary<Collider, float>();

    // 1. DeliveryAsset이 호출 (Projectile.cs와 동일)
    public void Initialize(in SkillContext ctx)
    {
        _ctx = ctx;
        _isInitialized = true;

        // 상태 리셋 (풀링 대비)
        _distanceTraveled = 0f;
        _targetsInside.Clear();

        // 최대 사거리 계산 (Projectile.cs와 동일)
        _maxTravelDistance = _ctx.Spec.skillRange * projectileRangeMultiplier;
    }

    // 2. 이동 및 사거리 체크 (Projectile.cs와 동일)
    void Update()
    {
        if (!_isInitialized) return;

        float deltaDistance = moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * deltaDistance);

        _distanceTraveled += deltaDistance;
        if (_distanceTraveled >= _maxTravelDistance)
        {
            _isInitialized = false;
            _ctx.Spawner.Despawn(gameObject);
        }
    }

    // 3. (실시간) 충돌 감지 (최초 진입)
    void OnTriggerEnter(Collider other)
    {
        if (!_isInitialized) return;

        // 장애물 레이어에 부딪혔는지 '먼저' 확인
        if ((obstacleLayer.value & (1<<other.gameObject.layer)) != 0)
        {
            // 장애물에 부딪히면 '아무것도 하지 말고' 즉시 소멸
            _isInitialized = false;
            _ctx.Spawner.Despawn(gameObject);
            return; // 함수 종료
        }

        // 1. 시전자(Caster)는 절대 맞으면 안 됨
        if (other.transform == _ctx.Caster)
            return;

        // 2. 타겟 레이어 확인
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 3. 딕셔너리에 추가하고 '즉시' 1회 적용
        if (!_targetsInside.ContainsKey(other))
        {
            _targetsInside.Add(other, Time.time);
            ApplyImpactTo(other.transform);
        }
    }

    // 4. (실시간) 충돌 유지 (핵심 로직)
    void OnTriggerStay(Collider other)
    {
        if (!_isInitialized) return;

        // 딕셔너리에 있는지 확인
        if (_targetsInside.TryGetValue(other, out float lastTickTime))
        {
            // (마지막 틱 시간 + 0.2초)가 지났다면
            if (Time.time > lastTickTime + tickRate)
            {
                _targetsInside[other] = Time.time; // 마지막 틱 시간 갱신
                ApplyImpactTo(other.transform); // 임팩트 적용
            }
        }
    }

    // 5. (실시간) 충돌 이탈
    void OnTriggerExit(Collider other)
    {
        _targetsInside.Remove(other);
    }

    // 6. 임팩트 적용 (Projectile.cs와 동일)
    void ApplyImpactTo(Transform target)
    {
        if (_ctx.Spec.impacts == null) return;

        for (int i = 0; i < _ctx.Spec.impacts.Length; i++)
        {
            if (_ctx.Spec.impacts[i] != null)
            {
                _ctx.Spec.impacts[i].Apply(_ctx, target);
            }
        }
    }
}