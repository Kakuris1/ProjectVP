using UnityEngine;

public class Player : MonoBehaviour
{
    // �̱��� ���� Player
    public static Player Instance { get; private set; }

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
