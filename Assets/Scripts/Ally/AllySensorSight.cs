using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
public class AllySensorSight : MonoBehaviour, ISkillTargetSensor
{
    AllyInformation Info;

    [Header("시야 설정")]
    [Tooltip("적을 감지할 수 있는 최대 수직(Y) 높이 차이")]
    [SerializeField] private float maxVerticalDistance = 3f;

    [Tooltip("시야 확인(Linecast)을 시작할 눈의 높이")]
    [SerializeField] private float eyeHeight = 1.6f;

    [Header("스캔 설정")]
    [Tooltip("초당 적을 스캔하는 횟수")]
    [SerializeField] private float updateHz = 5f;

    [Header("필터링 레이어")]
    [Tooltip("감지할 적의 레이어 마스크")]
    [SerializeField] private LayerMask enemyLayerMask;

    [Tooltip("시야를 가로막는 장애물(벽, 지형 등)의 레이어 마스크")]
    [SerializeField] private LayerMask obstacleLayerMask;

    /// <summary>
    /// 시야에 들어온 타겟이 변경되었을 때 발생하는 이벤트
    /// 타겟을 새로 찾았을 때: (Transform)
    /// 타겟을 잃었을 때: (null)
    /// </summary>
    public event Action<Transform> OnTargetChanged;

    // 매 갱신되는 가장 가까운 대상
    public Transform currentNearestTarget { get; private set; }
    private float scanTimer;

    // 매번 메모리를 할당하지 않도록 콜라이더 배열을 미리 캐시
    private Collider[] detectedColliders = new Collider[30];
    private List<Transform> targetColliders = new List<Transform>();

    private void Awake()
    {
        Info = GetComponent<AllyInformation>();
    }
    private void Update()
    {
        scanTimer += Time.deltaTime;
        if (scanTimer < 1f/updateHz) return;
        else ScanForEnemies();
        scanTimer = 0f;
    }

    public List<Transform> GetCurrentTargetList()
    {
        return targetColliders;
    }

    public Transform GetNearestTarget()
    {
        return currentNearestTarget;
        
    }

    // 주변의 적을 스캔하고 유효한 타겟을 찾음
    private void ScanForEnemies()
    {
        // 스킬 타겟 리스트 clear
        targetColliders.Clear();
        // 1. OverlapSphere로 1차 감지
        // Y축 높이도 고려해야 하므로 detectionRadius를 그대로 사용
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            Info.detectionRadius,
            detectedColliders,
            enemyLayerMask
        );

        Transform bestTarget = null;
        float closestDistanceSqr = float.MaxValue; // 제곱 거리를 사용해 불필요한 Sqrt 연산 방지

        for (int i = 0; i < hitCount; i++)
        {
            Transform potentialTarget = detectedColliders[i].transform;

            // 2. Y축 높이 차이 필터링
            float verticalDistance = Mathf.Abs(transform.position.y - potentialTarget.position.y);
            if (verticalDistance > maxVerticalDistance)
            {
                continue; // 너무 높거나 낮으면 무시
            }

            // 3. 시야(Line of Sight) 확인 (Narrow Phase)
            if (!HasLineOfSight(potentialTarget))
            {
                continue; // 장애물에 가려져 있으면 무시
            }

            // 스킬 타겟 리스트에 추가
            targetColliders.Add(potentialTarget);

            // 4. 가장 가까운 타겟 찾기
            float distanceSqr = (potentialTarget.position - transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                bestTarget = potentialTarget;
            }
        }

        // 5. 타겟이 변경되었는지 확인하고 이벤트 발생
        if (currentNearestTarget != bestTarget)
        {
            currentNearestTarget = bestTarget;
            OnTargetChanged?.Invoke(currentNearestTarget);
        }
    }

    // 지정된 타겟까지 장애물(벽)이 가로막고 있는지 확인
    private bool HasLineOfSight(Transform target)
    {
        // Linecast의 시작점 (자신의 눈 위치)
        Vector3 eyePosition = transform.position + (Vector3.up * eyeHeight);

        // Linecast의 끝점 (타겟의 가슴 높이 정도)
        Vector3 targetPosition = target.position + (Vector3.up * 1.0f);

        // Physics.Linecast는 두 지점 사이에 obstacleLayerMask에 해당하는 것이
        // 하나라도 감지되면 true를 반환합니다.
        if (Physics.Linecast(eyePosition, targetPosition, obstacleLayerMask))
        {
            return false; // 장애물에 가로막힘 (시야 X)
        }

        return true; // 시야가 깨끗함
    }
}
