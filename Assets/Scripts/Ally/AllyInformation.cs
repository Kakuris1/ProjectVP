using Combat.Skills;
using System;
using UnityEngine;
using UnityEngine.UI;
// 동료 유닛 데이터 중심 클래스
public class AllyInformation : MonoBehaviour, IUnitDataHub
{
    [Header("최초 구역")]
    public int targetAreaNumber;
    [Header("포메이션 정보")]
    public int FormationSlot;
    [Header("상태 (State)")]
    // 외부에서는 읽기만 가능하도록 private set을 사용합니다.
    public AllyUnitState CurrentState; //프로퍼티로 바꿔야함!!!
    public Vector3 CommandTargetPosition { get; private set; }

    [Header("능력치 (Stats)")]
    public float moveSpeed;
    public float skillRange;
    [SerializeField] private float maxHealth = 100f;
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; } = false;
    [Header("시야 설정")]
    [Tooltip("적을 감지할 최대 반경 (XZ 평면 기준)")]
    public float detectionRadius = 5f;
    [Header("스킬 (Skill")]
    public SkillSpecAsset Skill;
    private SkillController _skillController;
    [Tooltip("현재 목표물")]
    public Transform CurrentTarget;

    [Header("UI")]
    public GameObject uiPrefab; // UI 프리팹
    private GameObject UnitUI; // 생성할 UI 오브젝트 담을 변수
    // start()에서 연결
    [HideInInspector] public Slider hpSlider;
    [HideInInspector] public Slider skillSlider;

    // 이벤트
    public event Action<float, float> OnHPChanged;
    public event Action OnDeath;

    private void Awake()
    {
        // 체력 초기화
        CurrentHP = maxHealth;
        MaxHP = maxHealth;

        _skillController = GetComponent<SkillController>();
        if (_skillController != null)
        {
            _skillController.Equip(Skill);
            skillRange = Skill.skillRange;
        }
    }

    protected virtual void Start()
    {
        // 1. UI 프리팹 생성 및 연결
        if (uiPrefab != null)
        {
            UnitUI = Instantiate(uiPrefab, transform.position, Quaternion.identity);

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

    // 상태 변경
    public void ChangeState(AllyUnitState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
    }

    //현재 공격 대상을 설정
    public void SetTarget(Transform newTarget)
    {
        CurrentTarget = newTarget;
    }

    //명령받은 목표 지점을 설정
    public void SetCommandPosition(Vector3 position)
    {
        CommandTargetPosition = position;
    }
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
        ChangeState(AllyUnitState.Dead);
        OnDeath?.Invoke(); // 사망 알림
    }
}


// 아군 유닛이 가질 수 있는 상태들
public enum AllyUnitState
{
    Idle,           // 대기
    Following,      // 플레이어 추적
    Engaging,       // 적과 교전
    MovingToCommand,// 명령 지점으로 이동
    Dead            // 죽음
}