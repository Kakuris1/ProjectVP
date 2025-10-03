using UnityEngine;
// 구역 관리 매니저
public class AreaManager : MonoBehaviour
{
    [Header("구역 번호")]
    public int areaNumber;
    [Header("적 개체 번호")]
    [SerializeField]
    private int AreaEnemyID;    // 구역 적 개체의 번호
    [Header("클리어 조건 처치 수")]
    [SerializeField]
    private int NumberOfEnmeyInArea;    // 구역 적 개체수
    private int CountDefeatedEnemyInArea = 0; // 구역 처치된 적 개체 수
    private bool isCleared = false;

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

    private void HandleEnemyDefeated(int EnemyID)
    {
        if (isCleared) { return; }

        if(AreaEnemyID == EnemyID) CountDefeatedEnemyInArea++;
        if(NumberOfEnmeyInArea <= CountDefeatedEnemyInArea)
        {
            isCleared = true;
            EventManager.Instance.AreaCleared(areaNumber);
        }
    }
}