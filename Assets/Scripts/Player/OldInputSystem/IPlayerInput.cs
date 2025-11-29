using UnityEngine;

// 입력 인터페이스
public interface IPlayerInput
{
    // 이동 입력 인터페이스
    Vector2 GetMovementInput();
    // 회전 입력 인터페이스
    float GetRotationInput();
}
