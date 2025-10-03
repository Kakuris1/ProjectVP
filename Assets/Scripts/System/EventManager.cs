using System;
using UnityEngine;
// 이벤트 매니저 : 모든 이벤트 연결, 싱글톤
public class EventManager : MonoBehaviour
{
    // 싱글톤 Instance
    public static EventManager Instance { get; private set; }
    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 적 개체 파괴시 발생하는 이벤트
    public event Action<int> OnEnemyDefeated;
    // 적 개체의 ID를 받아서 넘김
    public void EnemyDefeated(int EnemyID)
    {
        OnEnemyDefeated?.Invoke(EnemyID);
    }

    // 구역 클리어시 발생하는 이벤트
    public event Action<int> OnAreaCleared;
    // 구역의 번호를 받아서 넘김
    public void AreaCleared(int AreaID) 
    {
        OnAreaCleared?.Invoke(AreaID); 
    }
}
