using System;

/// <summary>
/// 유닛 데이터를 소유하고 관련 이벤트를 제공하는
/// 모든 'Information' 클래스가 구현해야 하는 인터페이스
/// </summary>
public interface IUnitDataHub
{
    // --- 데이터 (Properties) ---
    float MaxHP { get; }
    float CurrentHP { get; }
    bool IsDead { get; }

    // --- 이벤트 (Methods) ---
    // 외부에서 이 함수들을 호출하여 데이터를 변경
    void Internal_TakeDamage(float amount);
    void Internal_Die();
     
    // --- 이벤트 (Events) ---
    // 외부(UI, Animator)에서 이 이벤트를 구독
    event Action<float, float> OnHPChanged;
    event Action OnDeath;
}
