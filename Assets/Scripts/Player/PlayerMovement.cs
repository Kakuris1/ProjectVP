using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Refs")]
    public InputReader input;
    private Rigidbody rb;

    [Header("Move")]
    public float maxSpeed = 5f; // �ְ� �ӵ�
    public float accel = 30f;   // ����
    public float decel = 40f;   // ����

    Vector3 _moveDir; // �̵�����
    Vector3 _lookDir; // �ٶ󺸴� ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector2 moveInput = input.Move;
        Vector2 lookInput = input.Look;
        _moveDir = moveInput.sqrMagnitude > 0.01f ? new Vector3(moveInput.x, 0f, moveInput.y).normalized : Vector3.zero;
        _lookDir = lookInput.sqrMagnitude > 0.01f ? new Vector3(lookInput.x, 0f, lookInput.y).normalized : _moveDir;
    }
    private void FixedUpdate()
    {
        Vector3 current = rb.linearVelocity;
        Vector3 target = _moveDir * maxSpeed;

        float rate = (_moveDir == Vector3.zero) ? decel : accel;
        Vector3 nextVelocity = Vector3.MoveTowards(current, target, rate * Time.fixedDeltaTime);
        rb.linearVelocity = nextVelocity;

        if(_lookDir.sqrMagnitude > 0.01f) rb.MoveRotation(Quaternion.LookRotation(_lookDir, Vector3.up));
    }






    /*
    // (Old) Input System �ڵ�

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
    */
}
