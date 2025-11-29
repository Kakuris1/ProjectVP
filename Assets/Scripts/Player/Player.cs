using Combat.Skills;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IUnitDataHub
{
    public static Player Instance { get; private set; }


    // --- 데이터 (Properties) ---
    [SerializeField] private float maxHealth = 100f;
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; } = false;

    [Header("UI")]
    public GameObject uiPrefab; // UI 프리팹
    private GameObject UnitUI; // 생성할 UI 오브젝트 담을 변수
    // start()에서 연결
    [HideInInspector] public Slider hpSlider;
    [HideInInspector] public Slider skillSlider;

    [Header("스킬 (Skill")]
    [Tooltip("인스펙터에서 직접 지정할 스킬. 비워두면(Null) Pool에서 랜덤 선택")]
    public SkillSpecAsset Skill;
    [Tooltip("Skill 변수가 비어있을 경우, 이 풀에서 랜덤 스킬을 선택")]
    public SkillPoolAsset defaultSkillPool;
    SkillController _skillController;

    // --- 이벤트 (Events) ---
    // 외부(UI, Animator)에서 이 이벤트를 구독
    public event Action<float, float> OnHPChanged;
    public event Action OnDeath;

    public static event Action<Transform> OnPlayerSpawned;

    private void Awake()
    {
        // 생성 시 Instance가 이미 존재하면 생성 취소
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // 생성 시 Instance 에 등록
        Instance = this;

        _skillController = GetComponent<SkillController>();
        // 1. 인스펙터에서 직접 할당한 스킬이 있는지 확인
        if (Skill == null)
        {
            // 2. 할당된 스킬이 없다면, defaultSkillPool에서 랜덤으로 가져오기
            if (defaultSkillPool != null && defaultSkillPool.skills.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, defaultSkillPool.skills.Count);
                Skill = defaultSkillPool.skills[randomIndex];
            }
            else
            {
                // 풀이 비어있거나 설정되지 않은 경우
                Debug.LogWarning($"[{name}] : 스킬이 할당되지 않았고, Default Skill Pool이 비어있거나 없습니다.", this);
            }
        }

        // 3. 최종 결정된 스킬을 SkillController에 장착
        if (Skill != null)
        {
            _skillController.Equip(Skill);
            Debug.Log($"[{name}] 스킬 배정 완료, Skill : " + Skill.name);
        }
        else
        {
            Debug.LogError($"[{name}] : 최종적으로 장착할 스킬이 없습니다!", this);
        }
    }

    private void Start()
    {
        // Player가 생성되면 자신의 Transform을 이벤트로 알림
        OnPlayerSpawned?.Invoke(this.transform);

        // 체력 초기화
        MaxHP = maxHealth;
        CurrentHP = MaxHP;

        // 체력 스킬게이지 UI
        // 1. UI 프리팹 생성 및 연결
        if (uiPrefab != null)
        {
            UnitUI = Instantiate(uiPrefab, transform.position, Quaternion.identity);

            // 하이어라키 창 정리
            UnitUI.transform.SetParent(UnitUIContainer.Instance.transform);

            // 2. UI가 '나'를 따라다니도록 Target 연결
            FollowTargetWithOffset followScript = UnitUI.GetComponent<FollowTargetWithOffset>();
            if (followScript != null)
            {
                followScript.target = this.transform;
            }

            // 3. UI 참조 스크립트에서 슬라이더 가져오기
            UnitUIReferences uiRefs = UnitUI.GetComponent<UnitUIReferences>();
            if (uiRefs != null)
            {
                hpSlider = uiRefs.hpSlider;
                skillSlider = uiRefs.skillSlider;
            }
            else
            {
                Debug.LogError(this.name + "의 UI 프리팹에서 UnitUIReferences 스크립트를 찾을 수 없습니다.");
            }

            // 4. 초기값 설정
            if (hpSlider != null)
            {
                hpSlider.value = CurrentHP / MaxHP; // (1.0)
            }
            if (skillSlider != null)
            {
                skillSlider.value = 1.0f; // 스킬은 꽉 찬 상태로 시작
            }

            // 5. SkillController에 skillSlider 참조 넘겨주기
            if (_skillController != null && skillSlider != null)
            {
                _skillController.SetSkillGauge(skillSlider);
            }
        }
        else
        {
            Debug.LogError(this.name + "에 uiPrefab이 할당되지 않았습니다!");
        }
    }

    // 오브젝트 제거 시 호출
    private void OnDestroy()
    {
        // 싱글톤 인스턴스 해제
        if(Instance == this)
        {
            Instance = null;
        }
    }


    // 아래는 IUnitDataHub 인터페이스 메소드
    public void Internal_TakeDamage(float amount)
    {
        if (IsDead) return;
        CurrentHP -= amount;

        OnHPChanged?.Invoke(CurrentHP, MaxHP); // HP 변경 알림

        // 체력바 UI 업데이트
        if (hpSlider != null)
        {
            // 값을 0~1 사이의 비율로 변환
            hpSlider.value = CurrentHP / MaxHP;
        }

        if (CurrentHP >= MaxHP) CurrentHP = MaxHP;
        Debug.Log("Player 피해 입음. HP : " +  CurrentHP);
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Internal_Die(); // 사망 처리
        }
    }

    public void Internal_Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeath?.Invoke(); // 사망 알림
        EventManager.Instance.TriggerGameOver(); // 전역 사망 알림
    }
}
