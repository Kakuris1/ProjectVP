using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    public int EnemyID; // �ν����Ϳ��� �� ������ ID�� ���� (��: 1)

    // ... (TakeDamage, Die �Լ� ����)

    public void Die()
    {
        Debug.Log($"���� ID {EnemyID} óġ!");

        // EventManager�� �ڽ��� ID�� �Բ� ����
        EventManager.Instance.EnemyDefeated(EnemyID);
        Destroy(gameObject);
    }
}