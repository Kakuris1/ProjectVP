using Combat.Skills;
using System;
using UnityEngine;
// 동료 유닛 데이터 중심 클래스
public class EnemyInformation : MonoBehaviour, IUnitDataHub
{
    [Header("최초 구역")]
    public int targetAreaNumber;
    [Header("적 유닛 ID")]
    public int EnemyID;
    [Header("상태 (State)")]
    // 외부에서는 읽기만 가능하도록 private set을 사용합니다.
    public EnemyUnitState CurrentState; //프로퍼티로 바꿔야함!!!
    // 플레이어와의 조우시 적대적 = 즉시 교전, 비적대적 = 도망
    public bool Hostile; // 프로퍼티로 바꿔야함
    [Header("비적대적일 때 행동 SO")]
    public NonHostileBehaviorAsset nonHostileBehavior;
    [Header("BT 제어 플래그")]
    [Tooltip("BT가 공격을 허가할 때만 true가 됨")]
    public bool CanAttack = false; // 기본값은 false
    // 적 최초 조우 여부
    protected bool firstTimeToMeet = true;
    // 유닛 이동 명령 여부
    public bool hasMoveCommand = false;
    public Vector3 CommandTargetPosition { get; private set; }

    [Header("능력치 (Stats)")]
    public float patrolSpeed = 5f;
    public float engagingSpeed = 8f;
    public float runSpeed = 10f;
    public float skillRange = 5f;
    [SerializeField] private float maxHealth = 100f;
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; } = false;
    [Header("시야 설정")]
    [Tooltip("적을 감지할 최대 반경 (XZ 평면 기준)")]
    public float detectionRadius = 12f;
    [Header("스킬 (Skill")]
    public SkillSpecAsset Skill;
    [Tooltip("현재 목표물")]
    public Transform CurrentTarget;

    // 이벤트
    public event Action<float, float> OnHPChanged; // 현재, 최대 체력
    public event Action OnDeath;

    protected virtual void Awake()
    {
        CurrentHP = maxHealth;
        MaxHP = maxHealth;
        SkillController _SkillController = GetComponent<SkillController>();
        _SkillController.Equip(Skill);
        skillRange = Skill.skillRange;
    }
    // 상태 변경
    public void ChangeState(EnemyUnitState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
    }

    //현재 공격 대상을 설정
    public virtual void SetTarget(Transform newTarget)
    {
        CurrentTarget = newTarget;
    }

    //명령받은 목표 지점을 설정
    public void SetCommandPosition(Vector3 position)
    {
        CommandTargetPosition = position;
    }
    
    public void SetCurrentHP(float value)
    {
        CurrentHP = value;
    }

    public virtual void Internal_TakeDamage(float amount)
    {
        if (IsDead) return;
        CurrentHP -= amount;
        // HP 변경 알림
        OnHPChanged?.Invoke(CurrentHP, MaxHP); 

        Debug.Log($"Hit, {gameObject.name} HP : {CurrentHP}");

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Internal_Die(); // 사망 처리
        }
    }

    public virtual void Internal_Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeath?.Invoke(); // 사망 알림 내부
        EventManager.Instance.EnemyDefeated(EnemyID); // 사망 알림 전역
        Debug.Log($"몬스터 ID {EnemyID} 처치!");

        Destroy(gameObject);
    }
}

// 적 유닛이 가질 수 있는 상태들
public enum EnemyUnitState
{
    StopAndWatching,// 경계
    Patrol,         // 순찰
    Engaging,       // 적과 교전
    MovingToCommand,// 명령 지점으로 이동
    Dead            // 죽음
}