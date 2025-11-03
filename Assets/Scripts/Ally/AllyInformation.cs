using Combat.Skills;
using System;
using UnityEngine;
// 동료 유닛 데이터 중심 클래스
public class AllyInformation : MonoBehaviour, IUnitDataHub
{
    [Header("최초 구역")]
    public int targetAreaNumber;
    [Header("포메이션 정보")]
    public int FormationSlot;
    [Header("상태 (State)")]
    // 외부에서는 읽기만 가능하도록 private set을 사용합니다.
    public UnitState CurrentState; //프로퍼티로 바꿔야함!!!
    public Vector3 CommandTargetPosition { get; private set; }

    [Header("능력치 (Stats)")]
    public float moveSpeed;
    public float skillRange;
    [SerializeField] private float maxHealth = 100f;
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; } = false;
    [Header("시야 설정")]
    [Tooltip("적을 감지할 최대 반경 (XZ 평면 기준)")]
    public float detectionRadius = 5f;
    [Header("스킬 (Skill")]
    public SkillSpecAsset Skill;
    [Tooltip("현재 목표물")]
    public Transform CurrentTarget;

    // 이벤트
    public event Action<float, float> OnHPChanged;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHP = maxHealth;
        MaxHP = maxHealth;
        SkillController _SkillController = GetComponent<SkillController>();
        _SkillController.Equip(Skill);
        skillRange = Skill.skillRange;
    }
    // 상태 변경
    public void ChangeState(UnitState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
    }

    //현재 공격 대상을 설정
    public void SetTarget(Transform newTarget)
    {
        CurrentTarget = newTarget;
    }

    //명령받은 목표 지점을 설정
    public void SetCommandPosition(Vector3 position)
    {
        CommandTargetPosition = position;
    }
    public void Internal_TakeDamage(float amount)
    {
        if (IsDead) return;
        CurrentHP -= amount;
        OnHPChanged?.Invoke(CurrentHP, MaxHP); // HP 변경 알림
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Internal_Die(); // 사망 처리
        }
    }

    public void Internal_Die()
    {
        if (IsDead) return;
        IsDead = true;
        ChangeState(UnitState.Dead);
        OnDeath?.Invoke(); // 사망 알림
    }
}


// 유닛이 가질 수 있는 상태들
public enum UnitState
{
    Idle,           // 대기
    Following,      // 플레이어 추적
    Engaging,       // 적과 교전
    MovingToCommand,// 명령 지점으로 이동
    Dead            // 죽음
}