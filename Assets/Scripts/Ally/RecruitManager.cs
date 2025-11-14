using Combat.Skills;
using Unity.VisualScripting;
using UnityEngine;
// 아군 영입 이벤트 관리 클래스
public class RecruitManager : MonoBehaviour
{
    private AllyInformation Info;
    [Header("영입시 이펙트")]
    [SerializeField]
    private GameObject recruitVFX;

    private ISpawner _spawner;

    private void Awake()
    {
        Info = GetComponent<AllyInformation>();
        if (SkillManager.Instance != null)
        {
            _spawner = SkillManager.Instance.Spawner;
        }
        else
        {
            Debug.LogError("RecruitManager가 SkillManager.Instance.Spawner를 찾을 수 없습니다!");
        }
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

    private void HandleAreaCleared(int AreaNumber)
    {
        if(AreaNumber == Info.targetAreaNumber) { Recruit(); }
    }

    // 동료 영입
    private void Recruit()
    {
        // 0. 영입시 발생 이벤트
        if (_spawner != null && recruitVFX != null)
        {
            _spawner.SpawnOneShot(recruitVFX, 1f, transform.position, transform.rotation, 5f);
        }

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
}
