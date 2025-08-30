using UnityEngine;

public class TopDownFollowCamera : MonoBehaviour
{
    public Transform target; // 타겟 = player
    public Vector3 offset = new Vector3(0, 10f, 0);
    public float acceleration = 5f; // 가속도
    public float deceleration = 5f; // 감속도
    public float maxSpeed = 10f; // 최고 속도
    private Vector3 velocity = Vector3.zero;

    public void HandleFollow()
    {
        if (target == null) return;
        Vector3 desiredPos = target.position + offset; //목표 위치 설정

        Vector3 toTarget = desiredPos - transform.position; // 목표 위치 - 카메라 위치
        Vector3 direction = toTarget.normalized; // 방향 정규화
        float distance = toTarget.magnitude;     // 현재 거리

        if (distance < 0.001f) return; // 미세 떨림 방지 최소 스냅

        velocity = direction * velocity.magnitude; // 방향 수정

        Vector3 desiredVelocity = direction * maxSpeed; // 목표 속도(벡터)
        Vector3 accelerationVector = (desiredVelocity - velocity).normalized * acceleration; // 가속도

        // 정지 거리 이내면 감속 시작
        if(distance < velocity.sqrMagnitude / (2 * deceleration))
        {
            accelerationVector = -velocity.normalized * deceleration;   // 감속 (가속도 음수)
        }

        // 속도 업데이트
        velocity += accelerationVector * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed); // 속도 클램핑

        // 위치 이동
        transform.position += velocity * Time.deltaTime;

        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // 고정 탑뷰
    }

    private void OnEnable()
    {
        Player.OnPlayerSpawned += SetTarget;
    }

    void OnDisable()
    {
        Player.OnPlayerSpawned -= SetTarget;
    }

    private void SetTarget(Transform player)
    {
        target = player;
    }
}