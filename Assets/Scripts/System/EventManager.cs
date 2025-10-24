using System;
using UnityEngine;
// �̺�Ʈ �Ŵ��� : ��� �̺�Ʈ ����, �̱���
public class EventManager : MonoBehaviour
{
    // �̱��� Instance
    public static EventManager Instance { get; private set; }
    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �� ��ü �ı��� �߻��ϴ� �̺�Ʈ
    public event Action<int> OnEnemyDefeated;
    // �� ��ü�� ID�� �޾Ƽ� �ѱ�
    public void EnemyDefeated(int EnemyID)
    {
        OnEnemyDefeated?.Invoke(EnemyID);
    }

    // ���� Ŭ����� �߻��ϴ� �̺�Ʈ
    public event Action<int> OnAreaCleared;
    // ������ ��ȣ�� �޾Ƽ� �ѱ�
    public void AreaCleared(int AreaID) 
    {
        OnAreaCleared?.Invoke(AreaID); 
    }
}
