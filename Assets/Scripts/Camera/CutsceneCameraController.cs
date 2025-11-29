using UnityEngine;

public class CutsceneCameraController : MonoBehaviour
{
    public Transform target;
    private Vector3 desiredPos;

    // 이동 관련 (TopDownFollowCamera의 로직)
    private Vector3 velocity = Vector3.zero;

    // 컷씬 상태 관련
    private enum CutsceneState { Moving, Waiting, Done }
    private CutsceneState state;

    [Header("컷씬 설정")]
    [Tooltip("타겟을 보여줄 시간 (초)")]
    public float waitDuration = 1.5f;
    private float waitTimer = 0f;

    [Tooltip("타겟 도착으로 간주할 최소 거리")]
    public float arrivalThreshold = 0.1f;

    // 참조
    private CameraManager cameraManager;
    private TopDownFollowCamera topDownFollow;


    private void Awake()
    {
        // CameraManager는 같은 게임오브젝트에 있습니다.
        cameraManager = GetComponent<CameraManager>();
        // TopDownFollow 카메라는 CameraManager가 알고 있습니다.
        topDownFollow = cameraManager.topDownFollow;
    }

    public void StartCutscene(Transform targetTransform)
    {
        target = targetTransform;

        // 이동 로직에 필요한 값들 초기화
        velocity = Vector3.zero;
        waitTimer = 0f;
        state = CutsceneState.Moving;
    }

    public void HandleCutscene()
    {
        if (target == null || state == CutsceneState.Done || topDownFollow == null) return;

        // 목표 위치 설정 (TopDown 카메라의 offset 값을 사용)
        desiredPos = target.position + topDownFollow.offset;

        switch (state)
        {
            case CutsceneState.Moving:
                MoveToTarget();
                break;

            case CutsceneState.Waiting:
                WaitAtTarget();
                break;
        }
    }

    private void MoveToTarget()
    {
        // --- TopDownFollowCamera.HandleFollow() 로직 시작 ---
        // Time.deltaTime 대신 Time.unscaledDeltaTime 사용

        Vector3 toTarget = desiredPos - transform.position;
        Vector3 direction = toTarget.normalized;
        float distance = toTarget.magnitude;

        // 1. 타겟에 도착했는지 확인
        if (distance < arrivalThreshold)
        {
            transform.position = desiredPos; // 정확한 위치로 스냅
            velocity = Vector3.zero;
            state = CutsceneState.Waiting; // 다음 상태(대기)로 변경
            return;
        }

        // 2. TopDownFollowCamera와 동일한 이동/가감속 로직 수행
        velocity = direction * velocity.magnitude;

        // TopDownFollow 스크립트에서 설정값들을 가져옵니다.
        float accel = topDownFollow.acceleration;
        float decel = topDownFollow.deceleration;
        float maxSpd = topDownFollow.maxSpeed;

        Vector3 desiredVelocity = direction * maxSpd;
        Vector3 accelerationVector = (desiredVelocity - velocity).normalized * accel;

        if (distance < velocity.sqrMagnitude / (2 * decel))
        {
            accelerationVector = -velocity.normalized * decel;
        }

        // 속도 및 위치 업데이트 (UnscaledDeltaTime 사용)
        velocity += accelerationVector * Time.unscaledDeltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpd);
        transform.position += velocity * Time.unscaledDeltaTime;
        // 로직 종료

        // 요청대로 카메라 각도는 탑다운 뷰로 고정
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void WaitAtTarget()
    {
        // 대기 시간 (UnscaledDeltaTime 사용)
        waitTimer += Time.unscaledDeltaTime;

        if (waitTimer >= waitDuration)
        {
            state = CutsceneState.Done;
            // CameraManager에게 컷씬 종료 및 복귀 신호를 보냅니다.
            cameraManager.SwitchToTopDown();
        }
    }
}