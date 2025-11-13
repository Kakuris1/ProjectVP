using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// 구역 관리 매니저
public class AreaManager : MonoBehaviour
{
    [Header("구역 번호")]
    public int areaNumber;
    [Header("스폰 설정")]
    public int NumberOfMouse;
    public int NumberOfConch;
    public int NumberOfUrchin1;
    public int NumberOfUrchin2;
    [Tooltip("이 반경 내에서 적들이 스폰됩니다.")]
    public float spawnRadius = 20f; // 스폰 반경

    [Header("거리 체크 설정")]
    [Tooltip("플레이어가 이 거리 안으로 들어오면 스폰 시작")]
    public float spawnDistance = 200f;
    [Tooltip("플레이어가 이 거리 밖으로 나가면 디스폰. 스폰 거리보다 커야 합니다.")]
    public float despawnDistance = 300f;
    [Tooltip("거리 체크 주기(초)")]
    public float checkInterval = 5.0f;

    [Header("클리어 조건 처치 수")]
    [SerializeField]
    [Tooltip("0 이면 생쥐 제외 모든 유닛 처치")]
    private int NumberOfClearCondition = 0;   // 클리어 조건 수
    private int CountDefeatedEnemyInArea = 0; // 구역 처치된 적 개체 수
    private bool isCleared = false;            // 구역 클리어 여부
    private List<GameObject> _activeEnemies = new List<GameObject>(); // 스폰된 적 추적 리스트
    private Transform _playerTransform; // 플레이어 참조
    private bool _isSpawned = false; // 현재 스폰되어 있는지 상태 플래그

    private void Awake()
    {
        if(NumberOfClearCondition == 0)
        {
            // 생쥐 제외 유닛 모두 처치시 클리어
            NumberOfClearCondition = NumberOfConch + NumberOfUrchin1 + NumberOfUrchin2;
        }

        // despawnDistance가 spawnDistance보다 작으면 경고 및 자동 보정
        if (despawnDistance <= spawnDistance)
        {
            Debug.LogWarning($"AreaManager {areaNumber}: 디스폰 거리({despawnDistance}m)가 스폰 거리({spawnDistance}m)보다 작거나 같습니다. 자동 보정합니다.");
            despawnDistance = spawnDistance + 50f;
        }
    }
    private void Start()
    {
        _playerTransform = Player.Instance.transform;

        // 0초 후 즉시 1회 실행, 그 후 checkInterval(5초)마다 CheckDistanceToPlayer 반복 호출
        InvokeRepeating(nameof(CheckDistanceToPlayer), 0f, checkInterval);
    }
    private void OnEnable()
    {   
        EventManager.Instance.OnEnemyDefeated += HandleEnemyDefeated;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnEnemyDefeated -= HandleEnemyDefeated;
        }
    }

    private void HandleEnemyDefeated(EnemyType enemyType, int EnemyID)
    {
        // 클리어 시 가드 클로즈
        if (isCleared) { return; }
        // 생쥐가 아닌 유닛만 카운트
        if(enemyType != EnemyType.Mouse) CountDefeatedEnemyInArea++;
        // 조건만큼 처치시 구역 클리어 판정
        if (NumberOfClearCondition <= CountDefeatedEnemyInArea)
        {
            isCleared = true;
            EventManager.Instance.AreaCleared(areaNumber);
        }
    }

    private void CheckDistanceToPlayer()
    {
        // 거리 계산 (Y축은 무시하고 XZ 평면 2D 거리로 계산)
        Vector3 playerPosXZ = new Vector3(_playerTransform.position.x, 0, _playerTransform.position.z);
        Vector3 areaPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
        float distanceSqr = (playerPosXZ - areaPosXZ).sqrMagnitude; // 제곱 거리가 Vector3.Distance보다 연산이 빠름

        // 1. 스폰 조건 (가까움 + 아직 스폰 안 됨 + 클리어 안 됨)
        if (distanceSqr < spawnDistance * spawnDistance && !_isSpawned && !isCleared)
        {
            Debug.Log($"Area {areaNumber}: 플레이어 접근, 스폰 시작.");
            SpawnAllEnemies();
            _isSpawned = true;
        }
        // 2. 디스폰 조건 (멈 + 현재 스폰된 상태 + 클리어 안 됨)
        else if (distanceSqr > despawnDistance * despawnDistance && _isSpawned && !isCleared)
        {
            Debug.Log($"Area {areaNumber}: 플레이어 이탈, 디스폰 시작.");
            DespawnAllEnemies();
            _isSpawned = false;

            // "다시 돌아오면 복구" 처치 카운트 리셋
            CountDefeatedEnemyInArea = 0;
        }
    }

    private void SpawnAllEnemies()
    {
        Debug.Log(areaNumber + "구역 적 스폰");
        SpawnEnemiesByType(0, NumberOfMouse);
        SpawnEnemiesByType(1, NumberOfConch);
        SpawnEnemiesByType(2, NumberOfUrchin1);
        SpawnEnemiesByType(3, NumberOfUrchin2);
    }

    private void SpawnEnemiesByType(int enemyType, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetRandomNavMeshPosition(transform.position, spawnRadius);
            GameObject Instance =  EnemySpawner.Instance.SpawnEnemy(areaNumber, enemyType, spawnPos);
            EnemyInformation spawnedEnemy = Instance.GetComponent<EnemyInformation>();
            if (spawnedEnemy != null)
            {
                spawnedEnemy.area = this;   // 스폰 유닛의 areaManager 추적
            }
            else Debug.LogError("적 유닛 스폰 후 초기화 실패");
            _activeEnemies.Add(Instance); // 추적 리스트에 추가
        }
    }
    private void DespawnAllEnemies()
    {
        Debug.Log(areaNumber + "구역 적 디스폰");
        foreach (GameObject enemy in _activeEnemies)
        {
            if (enemy != null && enemy.activeSelf) // 이미 죽지 않았다면
            {
                PoolManager.Instance.Despawn(enemy);
            }
        }
        _activeEnemies.Clear(); // 리스트 비우기 (다시 돌아오면 새로 스폰)
    }

    private Vector3 GetRandomNavMeshPosition(Vector3 origin, float radius)
    {
        // 1. 반경 내 랜덤한 지점 선택
        Vector3 randomDir = Random.insideUnitSphere * radius;
        randomDir += origin;

        // 2. NavMesh에서 가장 가까운 유효한 지점을 5m 반경 내에서 찾음
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return origin; // 못 찾으면 그냥 중앙에 스폰
        }
    }
}