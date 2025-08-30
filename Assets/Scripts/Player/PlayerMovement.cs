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

    // ����Ͽ��� MobileInput �� ����
    public void SetInput(IPlayerInput input)
    {
        playerInput = input; 
    }

    void FixedUpdate()
    {
        // �̵�
        Vector2 input = playerInput.GetMovementInput();
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        rb.MovePosition(transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        // ȸ��
        float yawInput = playerInput.GetRotationInput();
        if(Mathf.Abs(yawInput) > 0f)
        {
            float yawDelta = yawInput * rotationSpeedDeg * Time.fixedDeltaTime;
            Quaternion delta = Quaternion.Euler(0f, yawDelta, 0f);
            rb.MoveRotation(rb.rotation * delta);
        }
    }

}
