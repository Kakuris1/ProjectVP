using UnityEngine;

// AllyInformation�� �����͸� ������� �Ǵ��� ������ ���¸� �����ϴ� '�γ�' ����.
[RequireComponent(typeof(AllyInformation))]
public class AllyController : MonoBehaviour
{
    private AllyInformation allyInfo;

    // TODO: �� �������� �÷��̾��� ��ü ��� �ý��۰� �����Ǿ�� �մϴ�.
    private bool hasMoveCommand = false; // '�Ѱ��� ���̱�' ����� �޾Ҵ°�?
    private Vector3 moveCommandPosition; // '�Ѱ��� ���̱�' ����� ��ǥ ����

    // �� Ž�� ���� ����
    [Header("�� Ž��")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;

    void Awake()
    {
        allyInfo = GetComponent<AllyInformation>();
    }

    void Update()
    {
        // '�Ѱ��� ���̱�' ����� �ֿ켱 ������ �����ϴ�.
        if (hasMoveCommand)
        {
            allyInfo.ChangeState(UnitState.MovingToCommand);
            allyInfo.SetCommandPosition(moveCommandPosition);
            // TODO: ��ǥ ���� ���� �� hasMoveCommand�� false�� �ٲ��ִ� ���� �ʿ�
            return;
        }

        // ���� ����� ���� ���� Ž���ϰ� �����մϴ�.
        if (TeamManager.Instance.IsCombatMode)
        {
            FindAndEngageEnemy();
        }
        else
        {
            // ������ ����� ���� �׻� �÷��̾ ����ٴմϴ�.
            allyInfo.ChangeState(UnitState.Following);
        }
    }

    private void FindAndEngageEnemy()
    {
        // �ֺ��� ���� Ž���մϴ�.
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

        if (enemies.Length > 0)
        {
            // ���� ����� ���� ��ǥ�� �����մϴ�.
            currentTarget = enemies[0].transform;
            allyInfo.SetTarget(currentTarget);
            allyInfo.ChangeState(UnitState.Engaging);
        }
        else
        {
            // �ֺ��� ���� ������ �ٽ� �÷��̾ ���󰩴ϴ�.
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
