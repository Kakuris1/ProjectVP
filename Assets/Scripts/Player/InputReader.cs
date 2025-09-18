using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, GameInput.IPlayerActions
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool AttackToggle { get; private set; }
    public bool GamePause { get; private set; }

    private GameInput _input;

    private void OnEnable()
    {
        if(_input == null)
        {
            _input = new GameInput();         //���� �ν��Ͻ� ����
            _input.Player.SetCallbacks(this);
        }
        _input.Player.Enable();                 // Player �׼� �� Ȱ��ȭ
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

    public void OnAttackToggle(InputAction.CallbackContext ctx)
    {
        if (ctx.started) AttackToggle = !AttackToggle;
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if(ctx.started) GamePause = !GamePause;
    }
}
