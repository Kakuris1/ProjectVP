using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    public int EnemyID; // 인스펙터에서 이 몬스터의 ID를 설정 (예: 1)

    // ... (TakeDamage, Die 함수 로직)

    public void Die()
    {
        Debug.Log($"몬스터 ID {EnemyID} 처치!");

        // EventManager에 자신의 ID를 함께 전달
        EventManager.Instance.EnemyDefeated(EnemyID);
        Destroy(gameObject);
    }
}