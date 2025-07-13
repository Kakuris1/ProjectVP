using UnityEngine;

// PC �Է� ����
public class PCInput : IPlayerInput
{
    public Vector2 GetMovementInput()
    {
        // ���� �̵� �Է¹���
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        // ���� ũ�� 1�� ����ȭ
        return new Vector2(h, v).normalized;
    }
}
