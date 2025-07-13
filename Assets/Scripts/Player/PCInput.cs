using UnityEngine;

// PC 입력 전달
public class PCInput : IPlayerInput
{
    public Vector2 GetMovementInput()
    {
        // 방향 이동 입력받음
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        // 벡터 크기 1로 정규화
        return new Vector2(h, v).normalized;
    }
}
