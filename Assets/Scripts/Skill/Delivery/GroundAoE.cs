using UnityEngine;
using Combat.Skills;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GroundAoE : MonoBehaviour
{
    [Header("장판 설정")]
    public float duration = 5.0f;
    public float tickRate = 0.5f;

    [Header("대상 필터링")]
    public LayerMask targetLayer;

    // 내부 시스템
    private SkillContext _ctx;
    private bool _isInitialized = false;
    private bool _isActive = false;

    // [수정] 틱 관리 대상 목록 (OnTriggerEnter/Exit으로만 관리됨)
    private HashSet<Collider> _targetsInside = new HashSet<Collider>();
    private Coroutine _tickCoroutine;

    private Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }


    // 1. DeliveryAsset이 호출
    public void Initialize(in SkillContext ctx)
    {
        _ctx = ctx;
        _isInitialized = true;
        _isActive = false; // [추가] 재사용 시 리셋
        _targetsInside.Clear(); // [추가] 재사용 시 리셋
    }

    // 2. 스폰 직후 실행
    void Start()
    {
        if (!_isInitialized)
        {
            Debug.LogError("GroundAoE가 Initialize되지 않았습니다!", this);
            SelfDespawn(); // Despawn 호출
            return;
        }

        Invoke(nameof(SelfDespawn), duration + _ctx.Spec.skillDelay);
        Invoke(nameof(ActivateAoE), _ctx.Spec.skillDelay);
    }

    // 5. 활성화 시 대미지 틱 코루틴 시작
    void ActivateAoE()
    {
        _isActive = true;
        ApplyTickToAllTargets(); // 활성화 즉시 1회 적용
        _tickCoroutine = StartCoroutine(DamageTickCoroutine());
    }

    // 6. 대미지 틱 코루틴
    private IEnumerator DamageTickCoroutine()
    {
        // tickRate(예: 0.5초)마다 반복
        while (true)
        {
            yield return new WaitForSeconds(tickRate);
            ApplyTickToAllTargets();
        }
    }

    // ▼▼▼ [ 7. 수정된 함수 ] ▼▼▼
    private void ApplyTickToAllTargets()
    {
        if (_targetsInside.Count == 0) return;
        if (_ctx.Spec.impacts == null) return;

        // [수정] 복사본 생성
        List<Collider> targetsToTick = _targetsInside.ToList();

        foreach (var targetCollider in targetsToTick)
        {
            // [✨ 핵심 수정]
            // "가짜 널" 상태(오브젝트가 Destroy된)인지 먼저 확인합니다.
            // (Unity 오브젝트는 '==' 연산자가 오버로드되어 있어 안전합니다)
            if (targetCollider == null)
            {
                continue;
            }

            // [안전 장치 2] 비활성화(Despawn된) 상태인지 확인
            if (!targetCollider.gameObject.activeInHierarchy)
            {
                continue;
            }

            // 이제 targetCollider.transform이 안전함을 보장
            Transform targetTransform = targetCollider.transform;

            for (int i = 0; i < _ctx.Spec.impacts.Length; i++)
            {
                if (_ctx.Spec.impacts[i] != null)
                {
                    _ctx.Spec.impacts[i].Apply(_ctx, targetTransform);
                }
            }
        }

        // [제거] _targetsToRemove 및 _targetsInside.Remove 로직 제거
        // 목록 정리는 OnTriggerExit이 전담
    }

    // 8. (실시간) 장판 범위에 '처음' 들어왔을 때 (목록에만 추가)
    void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        if (other.transform == _ctx.Caster) return;
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        _targetsInside.Add(other);
    }

    // 9. (실시간) 장판 범위에서 '나갔을' 때 (목록에서 제거)
    void OnTriggerExit(Collider other)
    {
        _targetsInside.Remove(other);
    }

    // 10. (Invoke) 스스로 파괴(반납)
    private void SelfDespawn()
    {
        if (_tickCoroutine != null)
        {
            StopCoroutine(_tickCoroutine);
            _tickCoroutine = null; // [추가] 코루틴 참조 비우기
        }

        if (_isInitialized && _ctx.Spawner != null)
        {
            _ctx.Spawner.Despawn(gameObject);
        }
        else if (SkillManager.Instance?.Spawner != null)
        {
            SkillManager.Instance.Spawner.Despawn(gameObject);
        }
        else
        {
            Destroy(gameObject); // 최후의 수단
        }
    }
}