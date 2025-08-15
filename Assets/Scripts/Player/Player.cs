using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �̱��� ���� Player
    public static Player Instance { get; private set; }

    public static event Action<Transform> OnPlayerSpawned;

    // ������Ʈ ���� �� ȣ��
    private void Awake()
    {
        // ���� �� Instance�� �̹� �����ϸ� ���� ���
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // ���� �� Instance �� ���
        Instance = this;
    }

    // ��� Awake ���� ȣ��
    private void Start()
    {
        // Player�� �����Ǹ� �ڽ��� Transform�� �̺�Ʈ�� �˸�
        OnPlayerSpawned?.Invoke(this.transform);
    }

    // ������Ʈ ���� �� ȣ��
    private void OnDestroy()
    {
        // �̱��� �ν��Ͻ� ����
        if(Instance == this)
        {
            Instance = null;
        }
    }
}
