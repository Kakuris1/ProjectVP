using System;
using UnityEngine;
public class AllySensorSight : MonoBehaviour
{
    AllyInformation Info;

    [Header("�þ� ����")]
    [Tooltip("���� ������ �� �ִ� �ִ� ����(Y) ���� ����")]
    [SerializeField] private float maxVerticalDistance = 3f;

    [Tooltip("�þ� Ȯ��(Linecast)�� ������ ���� ����")]
    [SerializeField] private float eyeHeight = 1.6f;

    [Header("��ĵ ����")]
    [Tooltip("���� ��ĵ�ϴ� �ֱ� (��)")]
    [SerializeField] private float scanInterval = 0.5f;

    [Header("���͸� ���̾�")]
    [Tooltip("������ ���� ���̾� ����ũ")]
    [SerializeField] private LayerMask enemyLayerMask;

    [Tooltip("�þ߸� ���θ��� ��ֹ�(��, ���� ��)�� ���̾� ����ũ")]
    [SerializeField] private LayerMask obstacleLayerMask;

    /// <summary>
    /// �þ߿� ���� Ÿ���� ����Ǿ��� �� �߻��ϴ� �̺�Ʈ
    /// Ÿ���� ���� ã���� ��: (Transform)
    /// Ÿ���� �Ҿ��� ��: (null)
    /// </summary>
    public event Action<Transform> OnTargetChanged;

    // �� ���ŵǴ� ���� ����� ���
    public Transform currentNearestTarget { get; private set; }
    private float scanTimer;

    // �Ź� �޸𸮸� �Ҵ����� �ʵ��� �ݶ��̴� �迭�� �̸� ĳ��
    private Collider[] detectedColliders = new Collider[30];

    private void Awake()
    {
        Info = GetComponent<AllyInformation>();
    }
    private void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            scanTimer = scanInterval;
            ScanForEnemies();
        }
    }

    // �ֺ��� ���� ��ĵ�ϰ� ��ȿ�� Ÿ���� ã��
    private void ScanForEnemies()
    {
        // 1. OverlapSphere�� 1�� ����
        // Y�� ���̵� ����ؾ� �ϹǷ� detectionRadius�� �״�� ���
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            Info.detectionRadius,
            detectedColliders,
            enemyLayerMask
        );

        Transform bestTarget = null;
        float closestDistanceSqr = float.MaxValue; // ���� �Ÿ��� ����� ���ʿ��� Sqrt ���� ����

        for (int i = 0; i < hitCount; i++)
        {
            Transform potentialTarget = detectedColliders[i].transform;

            // 2. Y�� ���� ���� ���͸�
            float verticalDistance = Mathf.Abs(transform.position.y - potentialTarget.position.y);
            if (verticalDistance > maxVerticalDistance)
            {
                continue; // �ʹ� ���ų� ������ ����
            }

            // 3. �þ�(Line of Sight) Ȯ�� (Narrow Phase)
            if (!HasLineOfSight(potentialTarget))
            {
                continue; // ��ֹ��� ������ ������ ����
            }

            // 4. ���� ����� Ÿ�� ã��
            float distanceSqr = (potentialTarget.position - transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                bestTarget = potentialTarget;
            }
        }

        // 5. Ÿ���� ����Ǿ����� Ȯ���ϰ� �̺�Ʈ �߻�
        if (currentNearestTarget != bestTarget)
        {
            currentNearestTarget = bestTarget;
            OnTargetChanged?.Invoke(currentNearestTarget);
        }
    }

    // ������ Ÿ�ٱ��� ��ֹ�(��)�� ���θ��� �ִ��� Ȯ��
    private bool HasLineOfSight(Transform target)
    {
        // Linecast�� ������ (�ڽ��� �� ��ġ)
        Vector3 eyePosition = transform.position + (Vector3.up * eyeHeight);

        // Linecast�� ���� (Ÿ���� ���� ���� ����)
        Vector3 targetPosition = target.position + (Vector3.up * 1.0f);

        // Physics.Linecast�� �� ���� ���̿� obstacleLayerMask�� �ش��ϴ� ����
        // �ϳ��� �����Ǹ� true�� ��ȯ�մϴ�.
        if (Physics.Linecast(eyePosition, targetPosition, obstacleLayerMask))
        {
            return false; // ��ֹ��� ���θ��� (�þ� X)
        }

        return true; // �þ߰� ������
    }
}
