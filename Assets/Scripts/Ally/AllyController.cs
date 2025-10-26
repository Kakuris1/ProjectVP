using UnityEngine;
using UnityEngine.InputSystem;

// AllyInformation의 데이터를 기반으로 판단을 내리고 상태를 결정하는 '두뇌' 역할.
[RequireComponent(typeof(AllyInformation))]
public class AllyController : MonoBehaviour
{
    private AllyInformation allyInfo;
    private AllySensorSight allySensor;

    // TODO: 이 변수들은 플레이어의 전체 명령 시스템과 연동되어야 합니다.
    private bool hasMoveCommand = false; // '한곳에 모이기' 명령을 받았는가?
    private Vector3 moveCommandPosition; // '한곳에 모이기' 명령의 목표 지점

    // 적 탐지 관련 설정
    [Header("적 탐지")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("상태 전환 거리")]
    public float stopFollowingDistance = 4.0f; // 이 거리 안으로 들어오면 Idle
    public float startFollowingDistance = 10.0f; // 이 거리보다 멀어지면 Following

    void Awake()
    {
        allyInfo = GetComponent<AllyInformation>();
        allySensor = GetComponent<AllySensorSight>();
    }

    private void OnEnable()
    {
        // 센서의 "타겟 변경" 이벤트에 내 함수를 등록(구독)
        allySensor.OnTargetChanged += HandleTargetChanged;
    }

    private void OnDisable()
    {
        // 오브젝트가 파괴될 때 구독 해제 (메모리 누수 방지)
        allySensor.OnTargetChanged -= HandleTargetChanged;
    }

    void Update()
    {
        // '한곳에 모이기' 명령이 최우선 순위
        if (hasMoveCommand)
        {
            allyInfo.ChangeState(UnitState.MovingToCommand);
            allyInfo.SetCommandPosition(moveCommandPosition);
            // TODO: 목표 지점 도착 시 hasMoveCommand를 false로 바꿔주는 로직 필요
            return;
        }

        // 전투 모드일 때, 적이 있으면 교전
        if (TeamManager.Instance.IsCombatMode && allyInfo.CurrentTarget != null)
        {
            // 전투 상태
            allyInfo.ChangeState(UnitState.Engaging);
            return;
        }

        // 기본 행동 (플레이어 추적 또는 대기)
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

        if (distanceToPlayer > startFollowingDistance)
        {
            // 플레이어가 너무 멀어지면, 추적 상태로 변경
            allyInfo.ChangeState(UnitState.Following);
        }
        else if (distanceToPlayer <= stopFollowingDistance)
        {
            // 플레이어와 충분히 가까우면, 대기 상태로 변경
            allyInfo.ChangeState(UnitState.Idle);
        }
    }

    public void OrderMoveTo(Vector3 position)
    {
        hasMoveCommand = true;
        moveCommandPosition = position;
    }

    // 센서로부터 "타겟이 변경되었다"는 알림을 받는 함수
    private void HandleTargetChanged(Transform newTarget)
    {
        // 현재 전투중인 적이 있으면 대상 유지
        if (allyInfo.CurrentTarget != null) { return; }

        // 센서가 찾아준 타겟을 내 정보에 갱신
        allyInfo.CurrentTarget = newTarget;
    }
}
