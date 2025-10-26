using UnityEngine;
using UnityEngine.AI; // NavMeshAgent�� ����ϱ� ���� �߰�

// AllyInformation�� ��ϵ� ���¿� ���� ���� �̵��� �����ϴ� '�ٸ�' ����.
[RequireComponent(typeof(NavMeshAgent), typeof(AllyInformation))]
public class AllyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private AllyInformation allyInfo;
    private AllyController allyCon;
    private Transform playerTransform;

    [Header("Idle ���� ����")]
    [SerializeField] private float idleWanderRadius = 3.0f; // Idle �� ��ȸ �ݰ�
    [SerializeField] private float idleWanderInterval = 4.0f; // ��ȸ �ֱ� (��)
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
        // AllyInformation�� ��ϵ� ���� ���¸� �о�ͼ� �׿� �´� �̵� ������ ����
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

    // �÷��̾� ���� (4m �Ÿ� ����)
    private void HandleFollowingMovement()
    {
        agent.stoppingDistance = 4.0f;
        agent.SetDestination(playerTransform.position);

        // Idle ���·� ��ȯ�� �� 1�� �� ��ȸ�� �� �ֵ��� Ÿ�̸� �ʱ�ȭ
        idleWanderTimer = 1f;
    }

    // Idle ������ ��, 2m �ݰ� ������ �������� ��ȸ
    private void HandleIdleMovement()
    {
        idleWanderTimer -= Time.deltaTime;
            
        // ��ȸ�� �ð��� �Ǿ��ٸ�
        if (idleWanderTimer <= 0f)
        {
            // 1. 3m �ݰ� ���� ������ ����� ���� ����
            // 1-1 �÷��̾�� ���� �־����� ���⺤��
            Vector3 awayFromPlayerDir = (transform.position - playerTransform.position).normalized;
            // 1-2 ���� ���� ���⺤��
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 randomDir = new Vector3(randomCircle.x, 0 , randomCircle.y);

            float distance = (transform.position - playerTransform.position).magnitude;
            float MaxDistance = allyCon.startFollowingDistance;

            // 1-3 �÷��̾���� �Ÿ��� ���� ���⿡ ����ġ�� �� ���� ����
            Vector3 potentialDir = (awayFromPlayerDir * (1 - distance / MaxDistance) + randomDir * (distance / MaxDistance)).normalized;

            // 1-4 ���� ��ġ �߽����� ��ǥ ���� ����
            Vector3 potentialTargetPos = transform.position + potentialDir * idleWanderRadius;

            // 1-5 ��ǥ������ �ִ� �ݰ��� ����� �ʵ��� Ȯ��
            float distFromPlayer = Vector3.Distance(potentialTargetPos, playerTransform.position);
            if (distFromPlayer > MaxDistance)
            {
                // ��ǥ ������ �ִ� �ݰ��� ���
                // �÷��̾� -> ��ǥ ���� ���� ����
                Vector3 fromPlayerToTargetDir = (potentialTargetPos - playerTransform.position).normalized;

                // �÷��̾� ��ġ���� + �� �������� * �ִ� �Ÿ�(��¦ ����) ��ŭ ������ ������ ���ο� ��ǥ �������� ���� ����
                potentialTargetPos = playerTransform.position + fromPlayerToTargetDir * (MaxDistance - 0.5f);
            }

            // 2. �ش� ������ NavMesh ���� �ִ��� Ȯ�� (�ſ� �߿�!)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialTargetPos, out hit, idleWanderRadius, NavMesh.AllAreas))
            {
                // 3. NavMesh ���� ��ȿ�� �������� �̵� ���
                agent.stoppingDistance = 0f; // ��ȸ ���������� ��Ȯ�� �̵�
                agent.SetDestination(hit.position);
            }

            // 4. ���� ��ȸ �ð� ���� (��� ������ ���ÿ� �������� �ʰ� �ణ�� ������ �ο�)
            idleWanderTimer = idleWanderInterval + Random.Range(-1f, 1f);
        }
    }

    private void HandleCommandMovement()
    {
        // '���̱�' ���
        agent.stoppingDistance = 2f; // �ణ�� ����
        agent.SetDestination(allyInfo.CommandTargetPosition);
    }

    private void HandleEngagingMovement()
    {
        if (allyInfo.CurrentTarget == null)
        {
            StopMovement();
            return;
        }

        // ��ų ��Ÿ��� ���� ����
        agent.stoppingDistance = 5.0f; // (����: ��ų ��Ÿ� 5m)
        agent.SetDestination(allyInfo.CurrentTarget.position);
    }

    private void StopMovement()
    {
        if (agent.isOnNavMesh)
            agent.ResetPath();
    }


    // ���� �̻��

    [Header("���� ����")]
    [SerializeField] private float formationSpacing = 2.0f; // �����̼� ���� ����
    /// <summary>
    /// TeamManager�� �ο��� �ڽ��� �����̼� ���� ��ȣ�� �̿���
    /// �÷��̾� �ֺ��� ��ǥ ��ġ�� ���
    /// </summary>
    private Vector3 CalculateFormationPosition()
    {
        // �ڽ��� �����̼� ���� ��ȣ�� �����ɴϴ�.
        int slotIndex = allyInfo.FormationSlot;
        allyInfo.FormationSlot = slotIndex;

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

