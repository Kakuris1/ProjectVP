using System;
using UnityEngine;

public class Player : MonoBehaviour, IUnitDataHub
{
    public static Player Instance { get; private set; }


    // --- 데이터 (Properties) ---
    [SerializeField] private float maxHealth = 100f;
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; } = false;

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
    }

    private void Start()
    {
        // Player가 생성되면 자신의 Transform을 이벤트로 알림
        OnPlayerSpawned?.Invoke(this.transform);
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
    }
}
