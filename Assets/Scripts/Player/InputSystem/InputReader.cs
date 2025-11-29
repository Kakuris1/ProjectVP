using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, GameInput.IPlayerActions
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool AttackToggle { get; private set; }
    public bool PausePressed { get; private set; }
    public Vector2 MouseScreenPosition { get; private set; }
    public float ZoomScroll { get; private set; }

    private GameInput _input;

    private void OnEnable()
    {
        if(_input == null)
        {
            _input = new GameInput();         //래퍼 인스턴스 생성
            _input.Player.SetCallbacks(this);
        }
        _input.Player.Enable();                 // Player 액션 맵 활성화
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Move = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        Look = ctx.ReadValue<Vector2>();
    }

    public void OnLookMouse(InputAction.CallbackContext ctx)
    {
        MouseScreenPosition = ctx.ReadValue<Vector2>();
    }

    public void OnAttackToggle(InputAction.CallbackContext ctx)
    {
        if (ctx.started) AttackToggle = !AttackToggle;
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.started) PausePressed = true;
    }

    public void OnCameraZoom(InputAction.CallbackContext ctx)
    {
        // New Input System의 Scroll은 Vector2로 오며,
        // 위/아래 스크롤은 .y 값으로 전달됩니다.
        // (보통 0, +120, -120 같은 델타 값이 들어옵니다)
        ZoomScroll = ctx.ReadValue<Vector2>().y;
    }

    // ★★★ 아주 중요 ★★★
    // 프레임이 끝날 때 신호를 자동으로 리셋
    // 이렇게 하면 PauseManager가 신호를 놓치지 않고,
    // 한 번의 누름이 여러 프레임에 걸쳐 처리되지 않음
    private void LateUpdate()
    {
        PausePressed = false;

        // ZoomScroll은 축(Axis)처럼 프레임마다 0이 아닌 값을 유지하는 게 아니라,
        // '이벤트'처럼 스크롤이 발생한 그 프레임에만 값을 전달받고,
        // 다음 프레임에는 다시 0으로 리셋되어야 함.
        ZoomScroll = 0f;
    }
}
