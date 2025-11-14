using Combat.Skills;
using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
// 적 유닛 데이터 중심 클래스
public class EnemyInformation : MonoBehaviour, IUnitDataHub
{
    [SerializeField] private EnemyType enemyType;
    [Header("적 유닛 ID")]
    public int EnemyID;
    public AreaManager area;
    [Header("상태 (State)")]
    public EnemyUnitState CurrentState; //프로퍼티로 바꿔야함!!!
    // 플레이어와의 조우시 적대적 = 즉시 교전, 비적대적 = 도망
    public bool Hostile; // 프로퍼티로 바꿔야함
    [Header("비적대적일 때 행동 SO")]
    public NonHostileBehaviorAsset nonHostileBehavior;
    public Vector3 PatrolOrigin;
    [Header("BT 제어 플래그")]
    [Tooltip("BT가 공격을 허가할 때만 true가 됨")]
    public bool CanAttack = false; // 기본값은 false
    // 적 최초 조우 여부
    protected bool firstTimeToMeet = true;
    // 유닛 이동 명령 여부
    public bool hasMoveCommand = false;
    public Vector3 CommandTargetPosition { get; private set; }

    [Header("능력치 (Stats)")]
    public float patrolSpeed = 5f;
    public float engagingSpeed = 8f;
    public float runSpeed = 10f;
    public float skillRange = 5f;
    [SerializeField] private float maxHealth = 100f;
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; } = false;
    [Header("시야 설정")]
    [Tooltip("적을 감지할 최대 반경 (XZ 평면 기준)")]
    public float detectionRadius = 12f;
    [Header("스킬 (Skill")]
    public SkillSpecAsset Skill;
    private SkillController _skillController;
    private EnemyMovement _enemyMovement;
    [Tooltip("현재 목표물")]
    public Transform CurrentTarget;

    [Header("UI")]
    public GameObject uiPrefab; // UI 프리팹
    private GameObject UnitUI; // 생성할 UI 오브젝트 담을 변수
    public Vector3 UIOffset; // UI 생성 위치 (기본값 플레이어 유닛 기준이라 유닛별 재조정 필요)
    // start()에서 연결
    [HideInInspector] public Slider hpSlider;
    [HideInInspector] public Slider skillSlider;

    // 이벤트
    public event Action<float, float> OnHPChanged; // 현재, 최대 체력
    public event Action OnDeath;

    protected virtual void Awake()
    {
        // 최대체력 설정
        MaxHP = maxHealth;

        _skillController = GetComponent<SkillController>();
        if(_skillController != null && Skill!=null)
        {
            _skillController.Equip(Skill);
            skillRange = Skill.skillRange;
        }
        _enemyMovement = GetComponent<EnemyMovement>();
    }

    public virtual void Initialize(int AreaNumber, Vector3 spawnPos)
    {
        EnemyID = AreaNumber;
        ChangeState(EnemyUnitState.Patrol);
        Hostile = true;
        CanAttack = false;
        firstTimeToMeet = false;
        hasMoveCommand = false;
        CurrentHP = MaxHP;
        IsDead = false;
        CurrentTarget = null;
        ConnectUI();
        PatrolOrigin = spawnPos;
        _enemyMovement.Initialize();
    }

    protected virtual void ConnectUI()
    {
        // 1. UI 프리팹 생성 및 연결
        if (uiPrefab != null)
        {
            UnitUI = PoolManager.Instance.Spawn(uiPrefab, 1f, transform.position, Quaternion.identity);

            // 하이어라키 창 정리
            UnitUI.transform.SetParent(UnitUIContainer.Instance.transform);

            // 2. UI가 '나'를 따라다니도록 Target 연결
            FollowTargetWithOffset followScript = UnitUI.GetComponent<FollowTargetWithOffset>();
            if (followScript != null)
            {
                followScript.target = this.transform;
                followScript.cameraRelativeOffset = UIOffset;
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

            // 6. UI Fade 연결
            VisibilityFader fader = GetComponent<VisibilityFader>();
            if (fader != null)
            {
                fader.SetLinkedUI(UnitUI); // UI의 루트 오브젝트를 넘겨줌
            }
            else
            {
                // VisibilityFader가 없는 유닛(예: 플레이어)일 수 있으므로
                // 적 유닛이라면 Warning을 띄우는 것이 좋습니다.
                Debug.LogWarning(this.name + "에서 VisibilityFader를 찾을 수 없습니다. UI 숨기기가 작동하지 않을 수 있습니다.");
            }

            // 7. 보이지 않게 비활성화
            UnitUI.SetActive(false);
        }
        else
        {
            Debug.LogError(this.name + "에 uiPrefab이 할당되지 않았습니다!");
        }
    }

    // 상태 변경
    public void ChangeState(EnemyUnitState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
    }

    //현재 공격 대상을 설정
    public virtual void SetTarget(Transform newTarget)
    {
        CurrentTarget = newTarget;
    }

    //명령받은 목표 지점을 설정
    public void SetCommandPosition(Vector3 position)
    {
        CommandTargetPosition = position;
    }
    
    public void SetCurrentHP(float value)
    {
        CurrentHP = value;
    }

    public virtual void Internal_TakeDamage(float amount)
    {
        if (IsDead) return;
        CurrentHP -= amount;
        if (CurrentHP >= MaxHP) CurrentHP = MaxHP;
        // HP 변경 알림
        OnHPChanged?.Invoke(CurrentHP, MaxHP);

        // 체력바 UI 업데이트
        if (hpSlider != null)
        {
            // 값을 0~1 사이의 비율로 변환
            hpSlider.value = CurrentHP / MaxHP;
        }

        Debug.Log($"Hit, {gameObject.name} HP : {CurrentHP}");
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Internal_Die(); // 사망 처리
        }
    }

    public virtual void Internal_Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeath?.Invoke(); // 사망 알림 내부
        EventManager.Instance.EnemyDefeated(enemyType, EnemyID); // 사망 알림 전역
        Debug.Log($"몬스터 ID {EnemyID} 처치!");

        // UI 오브젝트도 함께 파괴
        if (UnitUI != null)
        {
            PoolManager.Instance.Despawn(UnitUI);
        }

        // 오브젝트 파괴
        PoolManager.Instance.Despawn(gameObject);
    }
}

// 적 유닛이 가질 수 있는 상태들
public enum EnemyUnitState
{
    StopAndWatching,// 경계
    Patrol,         // 순찰
    Engaging,       // 적과 교전
    MovingToCommand,// 명령 지점으로 이동
    Dead            // 죽음
}

public enum EnemyType
{
    Mouse,
    Conch,
    Urchin1_Ranged,
    Urchin2_Melee
}