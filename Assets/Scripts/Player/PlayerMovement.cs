using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Refs")]
    public InputReader input;
    private Rigidbody rb;
    private Camera mainCamera; // 마우스 계산을 위한 카메라

    [Header("Move")]
    public float maxSpeed = 5f; // 최고 속도
    public float accel = 30f;   // 가속
    public float decel = 40f;   // 감속

    Vector3 _moveDir; // 이동방향
    Vector3 _lookDir; // 바라보는 방향

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 이동 방향 전달
        Vector2 moveInput = input.Move;
        _moveDir = moveInput.sqrMagnitude > 0.01f ? new Vector3(moveInput.x, 0f, moveInput.y).normalized : Vector3.zero;

        // 바라보는 방향 전달
        Vector2 lookInput = input.Look;
        // (조이스틱이나 방향키를 조작하고 있으면 마우스는 무시)
        if (lookInput.sqrMagnitude > 0.01f)
        {
            _lookDir = new Vector3(lookInput.x, 0f, lookInput.y).normalized;
        }
        else
        {
            // 마우스 평면 좌표 값
            Vector2 mouseScreenPos = input.MouseScreenPosition;

            // 카메라에서 마우스 위치로 레이(Ray)
            Ray cameraRay = mainCamera.ScreenPointToRay(mouseScreenPos);

            // 플레이어의 Y축 높이를 기준으로 하는 가상의 바닥(Plane)을 생성
            // 3D 환경(Top-Down/Quarter View)에서 마우스가 가리키는 월드 좌표 구함
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            float rayDistance;
            if (groundPlane.Raycast(cameraRay, out rayDistance))
            {
                Vector3 worldPoint = cameraRay.GetPoint(rayDistance);
                Vector3 direction = (worldPoint - transform.position);

                // 마우스가 플레이어와 너무 가까우면(클릭) 방향이 0이 될 수 있으므로 체크
                if (direction.sqrMagnitude > 0.1f)
                {
                    // 활성화 시: _lookDir를 마우스 방향으로 설정 (Y축은 0으로 고정)
                    _lookDir = new Vector3(direction.x, 0f, direction.z).normalized;
                }
                else
                {
                    // 마우스가 플레이어 바로 위(또는 유효하지 않은) 경우 Look 입력을 0으로 만듦
                    _lookDir = Vector3.zero;
                }
            }
            else
            {
                // 마우스가 하늘을 가리키는 등 Raycast가 실패하면 Look 입력을 0으로 만듦
                _lookDir = Vector3.zero;
            }
        }
        // 만약 _lookDir 값이 설정되지 않으면 이동방향으로 보기
        if (_lookDir.sqrMagnitude < 0.01f)
        {
            _lookDir = _moveDir;
        }
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
    // (Old) Input System 코드

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
    */
}
