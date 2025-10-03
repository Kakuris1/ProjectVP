using Combat.Skills;
using UnityEngine;
// 동료 유닛 데이터 중심 클래스
public class AllyInformation : MonoBehaviour
{
    [Header("최초 구역")]
    public int targetAreaNumber;
    [Header("상태 (State)")]
    // 외부에서는 읽기만 가능하도록 private set을 사용합니다.
    public UnitState CurrentState; //프로퍼티로 바꿔야함!!!
    public Vector3 CommandTargetPosition { get; private set; }

    [Header("능력치 (Stats)")]
    public float moveSpeed;
    public float attackRange;
    [Header("스킬 (Skill")]
    public SkillSpecAsset Skill;
    public float NextSkillReadyTime { get; set; }
    public Transform CurrentTarget { get; set; }
    // ... 기타 필요한 능력치 (최대 체력, 공격력 등)

    [Header("포메이션 정보")]
    public int pomationnum;
    public int FormationSlot { get; set; }

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

    void Start()
    {
        // 게임 시작 시 기본 상태는 플레이어를 따라가는 것.
        ChangeState(UnitState.Idle);
    }
}

// 유닛이 가질 수 있는 상태들을 Enum(열거형)으로 정의합니다.
public enum UnitState
{
    Idle,           // 대기
    Following,      // 플레이어 추적
    Engaging,       // 적과 교전
    MovingToCommand // 명령 지점으로 이동
}



/*
 * 1. 아군 영입 이벤트

2. 아군 움직임 구현

3. 아군 상태 변경 (입력이나 주변 상황에 따른 상태 변화)

4. 아군 전체 명령 반응 코드 (명령 내릴 시 각 개체의 반응 구현)

5. 상호작용 관리

6. 아군/적군/중립 피아 변환

7. 스킬 사용 컴포넌트(이미 있는 플레이어 스킬 그대로 넣기)

8. 체력/사망 관리

9. AllyInformation (아군 유닛 정보 데이터를 모두 갖는 데이터 중심 클래스)*/