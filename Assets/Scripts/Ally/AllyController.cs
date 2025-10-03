using UnityEngine;

// AllyInformation의 데이터를 기반으로 판단을 내리고 상태를 결정하는 '두뇌' 역할.
[RequireComponent(typeof(AllyInformation))]
public class AllyController : MonoBehaviour
{
    private AllyInformation allyInfo;

    // TODO: 이 변수들은 플레이어의 전체 명령 시스템과 연동되어야 합니다.
    private bool hasMoveCommand = false; // '한곳에 모이기' 명령을 받았는가?
    private Vector3 moveCommandPosition; // '한곳에 모이기' 명령의 목표 지점

    // 적 탐지 관련 설정
    [Header("적 탐지")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;

    void Awake()
    {
        allyInfo = GetComponent<AllyInformation>();
    }

    void Update()
    {
        // '한곳에 모이기' 명령이 최우선 순위를 가집니다.
        if (hasMoveCommand)
        {
            allyInfo.ChangeState(UnitState.MovingToCommand);
            allyInfo.SetCommandPosition(moveCommandPosition);
            // TODO: 목표 지점 도착 시 hasMoveCommand를 false로 바꿔주는 로직 필요
            return;
        }

        // 전투 모드일 때만 적을 탐지하고 교전합니다.
        if (TeamManager.Instance.IsCombatMode)
        {
            FindAndEngageEnemy();
        }
        else
        {
            // 비전투 모드일 때는 항상 플레이어를 따라다닙니다.
            allyInfo.ChangeState(UnitState.Following);
        }
    }

    private void FindAndEngageEnemy()
    {
        // 주변의 적을 탐지합니다.
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

        if (enemies.Length > 0)
        {
            // 가장 가까운 적을 목표로 설정합니다.
            currentTarget = enemies[0].transform;
            allyInfo.SetTarget(currentTarget);
            allyInfo.ChangeState(UnitState.Engaging);
        }
        else
        {
            // 주변에 적이 없으면 다시 플레이어를 따라갑니다.
            allyInfo.SetTarget(null);
            allyInfo.ChangeState(UnitState.Following);
        }
    }

    public void OrderMoveTo(Vector3 position)
    {
        hasMoveCommand = true;
        moveCommandPosition = position;
    }
}
