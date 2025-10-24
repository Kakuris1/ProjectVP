using UnityEngine;
// ���� ���� �Ŵ���
public class AreaManager : MonoBehaviour
{
    [Header("���� ��ȣ")]
    public int areaNumber;
    [Header("�� ��ü ��ȣ")]
    [SerializeField]
    private int AreaEnemyID;    // ���� �� ��ü�� ��ȣ
    [Header("Ŭ���� ���� óġ ��")]
    [SerializeField]
    private int NumberOfEnmeyInArea;    // ���� �� ��ü��
    private int CountDefeatedEnemyInArea = 0; // ���� óġ�� �� ��ü ��
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