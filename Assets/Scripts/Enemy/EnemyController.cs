using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyInformation), typeof(EnemySensorSight))]
public class EnemyController : MonoBehaviour
{
    private EnemyInformation EnemyInfo;
    private EnemySensorSight sensor;

    // TODO: 여기에 나중에 만들 BT 컴포넌트 변수 선언
    // private BehaviorTreeRunner behaviorTree; 

    void Awake()
    {
        EnemyInfo = GetComponent<EnemyInformation>();
        sensor = GetComponent<EnemySensorSight>();
        // behaviorTree = GetComponent<BehaviorTreeRunner>();
    }

    private void OnEnable()
    {
        sensor.OnTargetChanged += HandleTargetChanged;
        EnemyInfo.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        // 오브젝트 비활성화 시 구독 해제 (메모리 누수 방지)
        if (sensor != null)
        {
            sensor.OnTargetChanged -= HandleTargetChanged;
        }
        if (EnemyInfo != null)
        {
            EnemyInfo.OnDeath -= HandleDeath;
        }
    }

    void Update()
    {
        if (EnemyInfo.IsDead)
        {
            // 죽었을 경우, 부활 등 타 코드 확장성 고려해서 Dead로 체크
            return;
        }

        // 이동 명령이 떨어지면 최우선
        if (EnemyInfo.hasMoveCommand)
        {
            EnemyInfo.ChangeState(EnemyUnitState.MovingToCommand);
            return;
        }

        // 타겟이 없으면 순찰
        if(EnemyInfo.CurrentTarget == null)
        {
            EnemyInfo.ChangeState(EnemyUnitState.Patrol);
            return;
        }
        else if (EnemyInfo.Hostile)
        {// 타겟이 있고 적대적임
            EnemyInfo.ChangeState(EnemyUnitState.Engaging);
            return;
        }
        else
        {// 타겟이 있으나 비적대적
            EnemyInfo.ChangeState(EnemyUnitState.StopAndWatching);
        }
    }

    public void OrderMoveTo(Vector3 position)
    {
        EnemyInfo.hasMoveCommand = true;
        EnemyInfo.SetCommandPosition(position);
    }

    private void HandleDeath()
    {
        // 이미 죽음 상태가 아니라면 상태 변경 (한 번만 실행)
        if (EnemyInfo.CurrentState != EnemyUnitState.Dead)
        {
            EnemyInfo.ChangeState(EnemyUnitState.Dead);
            // (선택) 여기서 NavMeshAgent 비활성화 등 추가 정리 작업 가능
            this.enabled = false; // 컨트롤러 자체를 꺼버림
        }
        return; // 죽었으면 FSM 정지
    }


    // --- 이벤트 핸들러 ---

    /// <summary>
    /// '눈'(Sensor)이 타겟 변경을 '두뇌'(Controller)에 보고할 때 호출되는 함수
    /// </summary>
    private void HandleTargetChanged(Transform newTarget)
    {
        // 교전 중일 때는 BT가 타겟 변경(어그로)을 관리해야 하므로,
        // FSM이 멋대로 타겟을 바꾸지 않도록
        if (EnemyInfo.CurrentState == EnemyUnitState.Engaging) return;

        // 현재 추적중인 적이 있으면 대상 유지
        if (EnemyInfo.CurrentTarget != null) { return; }
        // 비전투(Stop, Patrol) 중에 타겟을 발견한 경우
        EnemyInfo.SetTarget(newTarget);
    }
}