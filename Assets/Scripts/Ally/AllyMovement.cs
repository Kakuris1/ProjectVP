using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위해 추가

// AllyInformation에 기록된 상태에 따라 실제 이동을 실행하는 '다리' 역할.
[RequireComponent(typeof(NavMeshAgent), typeof(AllyInformation))]
public class AllyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private AllyInformation allyInfo;
    private Transform playerTransform;

    [Header("추적 설정")]
    [SerializeField] private float formationSpacing = 2.0f; // 포메이션 대형 간격

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        allyInfo = GetComponent<AllyInformation>();

        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // AllyInformation의 상태에 따라 다른 이동 로직을 수행합니다.
        switch (allyInfo.CurrentState)
        {
            case UnitState.Following:
                agent.stoppingDistance = 3f;
                Vector3 formationPosition = CalculateFormationPosition();
                agent.SetDestination(formationPosition);
                break;

            case UnitState.MovingToCommand:
                agent.stoppingDistance = 0f;
                agent.SetDestination(allyInfo.CommandTargetPosition);
                break;

            case UnitState.Engaging:
                if (allyInfo.CurrentTarget != null)
                {
                    // 교전 시에는 적과의 거리를 스킬 사거리에 맞게 유지합니다.
                    agent.stoppingDistance = allyInfo.attackRange;
                    agent.SetDestination(allyInfo.CurrentTarget.position);
                }
                break;

            case UnitState.Idle:
                agent.ResetPath(); // 이동 중지
                break;
        }
    }

    /// <summary>
    /// TeamManager가 부여한 자신의 포메이션 슬롯 번호를 이용해
    /// 플레이어 주변의 목표 위치를 계산합니다.
    /// </summary>
    private Vector3 CalculateFormationPosition()
    {
        // 자신의 포메이션 슬롯 번호를 가져옵니다.
        int slotIndex = allyInfo.FormationSlot;
        allyInfo.pomationnum = slotIndex;

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

