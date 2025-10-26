using UnityEngine;
using UnityEngine.InputSystem;

// AllyInformation�� �����͸� ������� �Ǵ��� ������ ���¸� �����ϴ� '�γ�' ����.
[RequireComponent(typeof(AllyInformation))]
public class AllyController : MonoBehaviour
{
    private AllyInformation allyInfo;
    private AllySensorSight allySensor;

    // TODO: �� �������� �÷��̾��� ��ü ��� �ý��۰� �����Ǿ�� �մϴ�.
    private bool hasMoveCommand = false; // '�Ѱ��� ���̱�' ����� �޾Ҵ°�?
    private Vector3 moveCommandPosition; // '�Ѱ��� ���̱�' ����� ��ǥ ����

    // �� Ž�� ���� ����
    [Header("�� Ž��")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("���� ��ȯ �Ÿ�")]
    public float stopFollowingDistance = 4.0f; // �� �Ÿ� ������ ������ Idle
    public float startFollowingDistance = 10.0f; // �� �Ÿ����� �־����� Following

    void Awake()
    {
        allyInfo = GetComponent<AllyInformation>();
        allySensor = GetComponent<AllySensorSight>();
    }

    private void OnEnable()
    {
        // ������ "Ÿ�� ����" �̺�Ʈ�� �� �Լ��� ���(����)
        allySensor.OnTargetChanged += HandleTargetChanged;
    }

    private void OnDisable()
    {
        // ������Ʈ�� �ı��� �� ���� ���� (�޸� ���� ����)
        allySensor.OnTargetChanged -= HandleTargetChanged;
    }

    void Update()
    {
        // '�Ѱ��� ���̱�' ����� �ֿ켱 ����
        if (hasMoveCommand)
        {
            allyInfo.ChangeState(UnitState.MovingToCommand);
            allyInfo.SetCommandPosition(moveCommandPosition);
            // TODO: ��ǥ ���� ���� �� hasMoveCommand�� false�� �ٲ��ִ� ���� �ʿ�
            return;
        }

        // ���� ����� ��, ���� ������ ����
        if (TeamManager.Instance.IsCombatMode && allyInfo.CurrentTarget != null)
        {
            // ���� ����
            allyInfo.ChangeState(UnitState.Engaging);
            return;
        }

        // �⺻ �ൿ (�÷��̾� ���� �Ǵ� ���)
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

        if (distanceToPlayer > startFollowingDistance)
        {
            // �÷��̾ �ʹ� �־�����, ���� ���·� ����
            allyInfo.ChangeState(UnitState.Following);
        }
        else if (distanceToPlayer <= stopFollowingDistance)
        {
            // �÷��̾�� ����� ������, ��� ���·� ����
            allyInfo.ChangeState(UnitState.Idle);
        }
    }

    public void OrderMoveTo(Vector3 position)
    {
        hasMoveCommand = true;
        moveCommandPosition = position;
    }

    // �����κ��� "Ÿ���� ����Ǿ���"�� �˸��� �޴� �Լ�
    private void HandleTargetChanged(Transform newTarget)
    {
        // ���� �������� ���� ������ ��� ����
        if (allyInfo.CurrentTarget != null) { return; }

        // ������ ã���� Ÿ���� �� ������ ����
        allyInfo.CurrentTarget = newTarget;
    }
}
