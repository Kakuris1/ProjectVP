using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위해 추가

// AllyInformation에 기록된 상태에 따라 실제 이동을 실행하는 '다리' 역할.
[RequireComponent(typeof(NavMeshAgent), typeof(AllyInformation))]
public class AllyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private AllyInformation allyInfo;
    private AllyController allyCon;
    private Transform playerTransform;

    [Header("Idle 상태 설정")]
    [SerializeField] private float idleWanderRadius = 3.0f; // Idle 시 배회 반경
    [SerializeField] private float idleWanderInterval = 4.0f; // 배회 주기 (초)
    [SerializeField] private float idleWanderTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        allyInfo = GetComponent<AllyInformation>();
        allyCon = GetComponent<AllyController>();
        playerTransform = Player.Instance.transform;
    }

    private void Update()
    {
        // AllyInformation에 기록된 현재 상태를 읽어와서 그에 맞는 이동 로직을 실행
        switch (allyInfo.CurrentState)
        {
            case UnitState.Following:
                HandleFollowingMovement();
                break;

            case UnitState.MovingToCommand:
                HandleCommandMovement();
                break;

            case UnitState.Engaging:
                HandleEngagingMovement();
                break;

            case UnitState.Idle:
                HandleIdleMovement();
                break;
        }
    }

    // 플레이어 추적 (4m 거리 유지)
    private void HandleFollowingMovement()
    {
        agent.stoppingDistance = 4.0f;
        agent.SetDestination(playerTransform.position);

        // Idle 상태로 전환될 때 1초 후 배회할 수 있도록 타이머 초기화
        idleWanderTimer = 1f;
    }

    // Idle 상태일 때, 2m 반경 내에서 무작위로 배회
    private void HandleIdleMovement()
    {
        idleWanderTimer -= Time.deltaTime;
            
        // 배회할 시간이 되었다면
        if (idleWanderTimer <= 0f)
        {
            // 1. 3m 반경 내의 무작위 방향과 지점 설정
            // 1-1 플레이어로 부터 멀어지는 방향벡터
            Vector3 awayFromPlayerDir = (transform.position - playerTransform.position).normalized;
            // 1-2 순수 랜덤 방향벡터
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 randomDir = new Vector3(randomCircle.x, 0 , randomCircle.y);

            float distance = (transform.position - playerTransform.position).magnitude;
            float MaxDistance = allyCon.startFollowingDistance;

            // 1-3 플레이어와의 거리에 따라 방향에 가중치를 준 벡터 설정
            Vector3 potentialDir = (awayFromPlayerDir * (1 - distance / MaxDistance) + randomDir * (distance / MaxDistance)).normalized;

            // 1-4 기존 위치 중심으로 목표 지점 설정
            Vector3 potentialTargetPos = transform.position + potentialDir * idleWanderRadius;

            // 1-5 목표지점이 최대 반경을 벗어나지 않도록 확인
            float distFromPlayer = Vector3.Distance(potentialTargetPos, playerTransform.position);
            if (distFromPlayer > MaxDistance)
            {
                // 목표 지점이 최대 반경을 벗어남
                // 플레이어 -> 목표 지점 방향 벡터
                Vector3 fromPlayerToTargetDir = (potentialTargetPos - playerTransform.position).normalized;

                // 플레이어 위치에서 + 그 방향으로 * 최대 거리(살짝 안쪽) 만큼 떨어진 지점을 새로운 목표 지점으로 강제 설정
                potentialTargetPos = playerTransform.position + fromPlayerToTargetDir * (MaxDistance - 0.5f);
            }

            // 2. 해당 지점이 NavMesh 위에 있는지 확인 (매우 중요!)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialTargetPos, out hit, idleWanderRadius, NavMesh.AllAreas))
            {
                // 3. NavMesh 위의 유효한 지점으로 이동 명령
                agent.stoppingDistance = 0f; // 배회 지점까지는 정확히 이동
                agent.SetDestination(hit.position);
            }

            // 4. 다음 배회 시간 설정 (모든 유닛이 동시에 움직이지 않게 약간의 랜덤성 부여)
            idleWanderTimer = idleWanderInterval + Random.Range(-1f, 1f);
        }
    }

    private void HandleCommandMovement()
    {
        // '모이기' 명령
        agent.stoppingDistance = 1; // 약간의 여유
        agent.SetDestination(allyInfo.CommandTargetPosition);
    }

    private void HandleEngagingMovement()
    {
        if (allyInfo.CurrentTarget == null)
        {
            StopMovement();
            return;
        }

        // 스킬 사거리에 맞춰 정지
        agent.stoppingDistance = allyInfo.attackRange; // (예시: 스킬 사거리 5m)
        agent.SetDestination(allyInfo.CurrentTarget.position);
    }

    private void StopMovement()
    {
        if (agent.isOnNavMesh)
            agent.ResetPath();
    }


    // 이하 미사용

    [Header("추적 설정")]
    [SerializeField] private float formationSpacing = 2.0f; // 포메이션 대형 간격
    /// <summary>
    /// TeamManager가 부여한 자신의 포메이션 슬롯 번호를 이용해
    /// 플레이어 주변의 목표 위치를 계산
    /// </summary>
    private Vector3 CalculateFormationPosition()
    {
        // 자신의 포메이션 슬롯 번호를 가져옵니다.
        int slotIndex = allyInfo.FormationSlot;
        allyInfo.FormationSlot = slotIndex;

        // 플레이어 뒤쪽 반원 형태로 위치를 계산합니다. (V자 대형 등 다양하게 변형 가능)
        // 슬롯 번호가 짝수냐 홀수냐에 따라 왼쪽/오른쪽으로 나눕니다.
        float angleOffset = (slotIndex % 2 == 0) ? -30 : 30;
        int rank = slotIndex / 2; // 몇 번째 줄에 서는지

        float angle = angleOffset * (rank + 1);
        float distance = formationSpacing * (rank + 1);

        // 플레이어의 방향을 기준으로 회전시켜 위치를 계산합니다.
        Vector3 offset = Quaternion.Euler(0, angle, 0) * playerTransform.forward * -1 * distance;

        return playerTransform.position + offset;
    }
}

