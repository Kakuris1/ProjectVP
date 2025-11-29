using UnityEngine;
using Combat.Skills;
using System.Collections.Generic;

// 맵에 미리 배치된 장판 오브젝트에 이 스크립트를 붙입니다.
// IsTrigger=true인 Collider가 필요합니다.
[RequireComponent(typeof(Collider))]
public class StaticZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [Tooltip("힐/대미지가 적용되는 주기 (초)")]
    public float tickRate = 1f;
    [Tooltip("이 장판이 영향을 줄 대상의 레이어 (예: Ally, Player)")]
    public LayerMask targetLayer;

    [Header("Impact Settings")]
    [Tooltip("이 장판이 적용할 효과 (예: HealImpactAsset)")]
    public ImpactAsset[] impacts;

    [Tooltip("틱당 힐량/대미지량 (이 값이 Impact로 전달됨)")]
    public float tickAmount = 10f;

    [Tooltip("틱마다 대상 위치에 스폰할 이펙트")]
    public GameObject tickVfx;
    public float tickVfxSize;

    // 이 장판이 스스로 생성해서 사용할 '가짜' SkillContext
    private SkillContext _zoneContext;

    // 장판 내부에 있는 타겟 목록
    private Dictionary<Collider, float> _targetsInside = new Dictionary<Collider, float>();

    void Awake()
    {
        // --- 이 장판 전용 '가짜' SkillContext 생성 ---

        // 1. SkillManager에서 서비스 가져오기
        ISpawner spawner = null;
        ITimeSource timeSource = null;
        if (SkillManager.Instance != null)
        {
            spawner = SkillManager.Instance.Spawner;
            timeSource = SkillManager.Instance.TimeSource;
        }
        else
        {
            Debug.LogError("StaticZone 스크립트는 씬에 SkillManager가 필요합니다!", this);
            this.enabled = false; // 스크립트 비활성화
            return;
        }

        // 2. '가짜' SkillRuntimeSpec 생성
        var fakeSpec = new SkillRuntimeSpec
        {
            skillName = gameObject.name,
            damage = this.tickAmount, // 인스펙터의 tickAmount를 damage 슬롯에 할당
            impacts = this.impacts,   // 인스펙터의 impacts를 할당
            hitVfx = null,
            hitVfxSize = 0f
            // (나머지 값은 기본값/null이어도 됨)
        };

        // 3. '가짜' SkillContext 최종 조립
        _zoneContext = new SkillContext
        {
            Caster = this.transform, // 힐 장판(이 오브젝트)이 Caster
            Origin = this.transform.position,
            Direction = this.transform.forward,
            Spec = fakeSpec,
            Time = timeSource,
            Spawner = spawner
        };
    }

    // --- GroundAoE.cs와 거의 동일한 로직 ---

    void OnTriggerEnter(Collider other)
    {
        // 타겟 레이어가 아니면 무시
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 목록에 없으면 추가하고 즉시 1회 적용
        if (!_targetsInside.ContainsKey(other))
        {
            _targetsInside.Add(other, Time.time);
            ApplyImpactTo(other.transform);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 틱 주기마다 힐 적용
        if (_targetsInside.TryGetValue(other, out float lastTickTime))
        {
            if (Time.time > lastTickTime + tickRate)
            {
                _targetsInside[other] = Time.time;
                ApplyImpactTo(other.transform);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 목록에서 제거
        _targetsInside.Remove(other);
    }

    // ImpactAsset 시스템을 사용하여 힐/대미지 적용
    void ApplyImpactTo(Transform target)
    {
        if (_zoneContext.Spec.impacts == null) return;

        // Awake()에서 만든 '가짜' 컨텍스트를 사용
        for (int i = 0; i < _zoneContext.Spec.impacts.Length; i++)
        {
            if (_zoneContext.Spec.impacts[i] != null)
            {
                _zoneContext.Spec.impacts[i].Apply(_zoneContext, target);
            }
        }

        //  StaticZone이 '직접' 이펙트를 스폰
        if (this.tickVfx != null && _zoneContext.Spawner != null)
        {
            //  'tickRate' (1.0초)를 파괴 시간으로 전달하며 새 메서드 호출
            _zoneContext.Spawner.SpawnOneShot(
                this.tickVfx,
                this.tickVfxSize,
                target.position,
                Quaternion.identity,
                this.tickRate // <-- '1.0' (tickRate) 값이 여기 들어갑니다.
            );
        }
    }
}