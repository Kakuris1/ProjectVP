using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeedDeg = 90f;
    private Rigidbody rb;

    private IPlayerInput playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

#if UNITY_EDITOR || UNITY_STANDALONE
        playerInput = new PCInput();
#endif

    }

    // 모바일에서 MobileInput 과 연결
    public void SetInput(IPlayerInput input)
    {
        playerInput = input; 
    }

    void FixedUpdate()
    {
        // 이동
        Vector2 input = playerInput.GetMovementInput();
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        rb.MovePosition(transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        // 회전
        float yawInput = playerInput.GetRotationInput();
        if(Mathf.Abs(yawInput) > 0f)
        {
            float yawDelta = yawInput * rotationSpeedDeg * Time.fixedDeltaTime;
            Quaternion delta = Quaternion.Euler(0f, yawDelta, 0f);
            rb.MoveRotation(rb.rotation * delta);
        }
    }

}
