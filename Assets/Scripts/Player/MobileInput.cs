using UnityEngine;

public class MobileInput : IPlayerInput
{
    private Vector2 joystickInput = Vector2.zero;

    public void SetJoystickInput(Vector2 input)
    {
        joystickInput = input;
    }

    public Vector2 GetMovementInput()
    {
        return joystickInput;
    }
}
