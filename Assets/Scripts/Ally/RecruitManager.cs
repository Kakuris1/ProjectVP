using Combat.Skills;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
// 아군 영입 이벤트 관리 클래스
public class RecruitManager : MonoBehaviour
{
    private AllyInformation Info;
    [Header("합류 전 방어막 이펙트 설정")]
    [SerializeField]
    private GameObject auraPrefab;
    [SerializeField]
    private float auraScale = 1f;
    [SerializeField]
    [Tooltip("이펙트가 스폰되는 주기 (초)")]
    private float spawnInterval = 4.0f;
    [SerializeField]
    [Tooltip("스폰된 이펙트가 지속되는 시간 (초)")]
    private float effectDuration = 4.0f;

    [Header("팀 합류시 이펙트")]
    [SerializeField]
    private GameObject recruitVFX;

    private ISpawner _spawner;

    private void Awake()
    {
        Info = GetComponent<AllyInformation>();
    }

    private void OnEnable()
    {
        EventManager.Instance.OnAreaCleared += HandleAreaCleared;
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnAreaCleared -= HandleAreaCleared;
        }
    }
    void Start()
    {
        // 1. SkillManager에서 ISpawner(PooledSpawner)를 가져옵니다.
        if (SkillManager.Instance != null)
        {
            _spawner = SkillManager.Instance.Spawner;
        }
        else
        {
            Debug.LogError("SkillManager.Instance를 찾을 수 없습니다! AllyPeriodicAura가 작동하지 않습니다.", this);
            return; // 스포너가 없으면 코루틴을 시작하지 않습니다.
        }

        // 2. 프리팹이 할당되었는지 확인합니다.
        if (auraPrefab == null)
        {
            Debug.LogError("Aura Prefab이 할당되지 않았습니다! AllyPeriodicAura가 작동하지 않습니다.", this);
            return;
        }

        // 3. 스폰 루프 코루틴을 시작합니다.
        StartCoroutine(SpawnAuraLoop());
    }


    private void HandleAreaCleared(int AreaNumber)
    {
        if(AreaNumber == Info.targetAreaNumber) { Recruit(); }
    }

    // 동료 영입
    private void Recruit()
    {
        // 0. 영입시 발생 이벤트
        // 영입 이펙트 재생
        if (_spawner != null && recruitVFX != null)
        {
            _spawner.SpawnOneShot(recruitVFX, 3f, transform.position, transform.rotation, 2f);
        }
        //카메라컷씬 이동
        CameraManager.Instance.SwitchToCutscene(transform);

        // 1. TeamManager에 자신을 아군으로 등록해달라고 요청
        TeamManager.Instance.RecruitAlly(Info);

        // 2. 이 유닛의 상태를 '아군'으로 변경
        // 예: info.ChangeFaction(Faction.Player);

        // 3. 영입 후 필요한 컴포넌트 활성화
        GetComponent<AllyController>().enabled = true;
        GetComponent<AllyMovement>().enabled = true;
        GetComponent<AllySensorSight>().enabled = true;
        GetComponent<SkillController>().enabled = true;
        GetComponent<SkillPipeline>().enabled = true;

        Debug.Log($"{name} 영입 절차 완료!");

        // 4. 영입 이벤트는 한 번만 일어나야 하므로, 이 스크립트는 비활성화
        this.enabled = false;
    }


    /// <summary>
    /// 팀 합류 전까지 spawnInterval(4초)마다 effectDuration(4초) 동안 지속되는 이펙트를 스폰하는 루프
    /// </summary>
    private IEnumerator SpawnAuraLoop()
    {
        // 이 컴포넌트(스크립트)가 활성화되어 있는 동안 무한 반복합니다.
        // (유닛이 죽어서 비활성화되면 루프도 자동으로 멈춥니다)
        while (this.enabled)
        {
            // 1. ISpawner의 SpawnOneShot을 사용해 이펙트를 스폰합니다.
            //    이 메서드는 effectDuration(4초) 뒤에 자동으로 Despawn(풀링 반납)을 처리합니다.
            _spawner.SpawnOneShot(
                auraPrefab,
                auraScale,
                transform.position,     // 이 Ally 유닛의 현재 위치에
                Quaternion.identity,    // 기본 회전값으로
                effectDuration          // 4초 뒤에 자동 파괴(반납)
            );

            // 2. 다음 스폰까지 spawnInterval(4초) 만큼 대기합니다.
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
