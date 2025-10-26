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

    [Header("�þ� ����")]
    [Tooltip("���� ������ �ִ� �ݰ� (XZ ��� ����)")]
    public float detectionRadius = 5f;

    [Header("��ų (Skill")]
    public SkillSpecAsset Skill;
    public float NextSkillReadyTime { get; set; }
    [Tooltip("���� ��ǥ��")]
    public Transform CurrentTarget { get; set; }
    // ... ��Ÿ �ʿ��� �ɷ�ġ (�ִ� ü��, ���ݷ� ��)

    [Header("�����̼� ����")]
    public int FormationSlot;

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
}

// ������ ���� �� �ִ� ���µ�
public enum UnitState
{
    Idle,           // ���
    Following,      // �÷��̾� ����
    Engaging,       // ���� ����
    MovingToCommand // ��� �������� �̵�
}