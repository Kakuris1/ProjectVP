using Combat.Skills;
using UnityEngine;
// ���� ���� ������ �߽� Ŭ����
public class AllyInformation : MonoBehaviour
{
    [Header("���� ����")]
    public int targetAreaNumber;
    [Header("���� (State)")]
    // �ܺο����� �б⸸ �����ϵ��� private set�� ����մϴ�.
    public UnitState CurrentState; //������Ƽ�� �ٲ����!!!
    public Vector3 CommandTargetPosition { get; private set; }

    [Header("�ɷ�ġ (Stats)")]
    public float moveSpeed;
    public float attackRange;
    [Header("��ų (Skill")]
    public SkillSpecAsset Skill;
    public float NextSkillReadyTime { get; set; }
    public Transform CurrentTarget { get; set; }
    // ... ��Ÿ �ʿ��� �ɷ�ġ (�ִ� ü��, ���ݷ� ��)

    [Header("�����̼� ����")]
    public int pomationnum;
    public int FormationSlot { get; set; }

    // ���� ����
    public void ChangeState(UnitState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
    }

    //���� ���� ����� ����
    public void SetTarget(Transform newTarget)
    {
        CurrentTarget = newTarget;
    }

    //��ɹ��� ��ǥ ������ ����
    public void SetCommandPosition(Vector3 position)
    {
        CommandTargetPosition = position;
    }

    void Start()
    {
        // ���� ���� �� �⺻ ���´� �÷��̾ ���󰡴� ��.
        ChangeState(UnitState.Idle);
    }
}

// ������ ���� �� �ִ� ���µ��� Enum(������)���� �����մϴ�.
public enum UnitState
{
    Idle,           // ���
    Following,      // �÷��̾� ����
    Engaging,       // ���� ����
    MovingToCommand // ��� �������� �̵�
}



/*
 * 1. �Ʊ� ���� �̺�Ʈ

2. �Ʊ� ������ ����

3. �Ʊ� ���� ���� (�Է��̳� �ֺ� ��Ȳ�� ���� ���� ��ȭ)

4. �Ʊ� ��ü ��� ���� �ڵ� (��� ���� �� �� ��ü�� ���� ����)

5. ��ȣ�ۿ� ����

6. �Ʊ�/����/�߸� �Ǿ� ��ȯ

7. ��ų ��� ������Ʈ(�̹� �ִ� �÷��̾� ��ų �״�� �ֱ�)

8. ü��/��� ����

9. AllyInformation (�Ʊ� ���� ���� �����͸� ��� ���� ������ �߽� Ŭ����)*/