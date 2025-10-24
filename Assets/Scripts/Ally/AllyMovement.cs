using UnityEngine;
using UnityEngine.AI; // NavMeshAgent�� ����ϱ� ���� �߰�

// AllyInformation�� ��ϵ� ���¿� ���� ���� �̵��� �����ϴ� '�ٸ�' ����.
[RequireComponent(typeof(NavMeshAgent), typeof(AllyInformation))]
public class AllyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private AllyInformation allyInfo;
    private Transform playerTransform;

    [Header("���� ����")]
    [SerializeField] private float formationSpacing = 2.0f; // �����̼� ���� ����

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

        // AllyInformation�� ���¿� ���� �ٸ� �̵� ������ �����մϴ�.
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
                    // ���� �ÿ��� ������ �Ÿ��� ��ų ��Ÿ��� �°� �����մϴ�.
                    agent.stoppingDistance = allyInfo.attackRange;
                    agent.SetDestination(allyInfo.CurrentTarget.position);
                }
                break;

            case UnitState.Idle:
                agent.ResetPath(); // �̵� ����
                break;
        }
    }

    /// <summary>
    /// TeamManager�� �ο��� �ڽ��� �����̼� ���� ��ȣ�� �̿���
    /// �÷��̾� �ֺ��� ��ǥ ��ġ�� ����մϴ�.
    /// </summary>
    private Vector3 CalculateFormationPosition()
    {
        // �ڽ��� �����̼� ���� ��ȣ�� �����ɴϴ�.
        int slotIndex = allyInfo.FormationSlot;
        allyInfo.pomationnum = slotIndex;

        // �÷��̾� ���� �ݿ� ���·� ��ġ�� ����մϴ�. (V�� ���� �� �پ��ϰ� ���� ����)
        // ���� ��ȣ�� ¦���� Ȧ���Ŀ� ���� ����/���������� �����ϴ�.
        float angleOffset = (slotIndex % 2 == 0) ? -30 : 30;
        int rank = slotIndex / 2; // �� ��° �ٿ� ������

        float angle = angleOffset * (rank + 1);
        float distance = formationSpacing * (rank + 1);

        // �÷��̾��� ������ �������� ȸ������ ��ġ�� ����մϴ�.
        Vector3 offset = Quaternion.Euler(0, angle, 0) * playerTransform.forward * -1 * distance;

        return playerTransform.position + offset;
    }
}

