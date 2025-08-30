using UnityEngine;

public class TopDownFollowCamera : MonoBehaviour
{
    public Transform target; // Ÿ�� = player
    public Vector3 offset = new Vector3(0, 10f, 0);
    public float acceleration = 5f; // ���ӵ�
    public float deceleration = 5f; // ���ӵ�
    public float maxSpeed = 10f; // �ְ� �ӵ�
    private Vector3 velocity = Vector3.zero;

    public void HandleFollow()
    {
        if (target == null) return;
        Vector3 desiredPos = target.position + offset; //��ǥ ��ġ ����

        Vector3 toTarget = desiredPos - transform.position; // ��ǥ ��ġ - ī�޶� ��ġ
        Vector3 direction = toTarget.normalized; // ���� ����ȭ
        float distance = toTarget.magnitude;     // ���� �Ÿ�

        if (distance < 0.001f) return; // �̼� ���� ���� �ּ� ����

        velocity = direction * velocity.magnitude; // ���� ����

        Vector3 desiredVelocity = direction * maxSpeed; // ��ǥ �ӵ�(����)
        Vector3 accelerationVector = (desiredVelocity - velocity).normalized * acceleration; // ���ӵ�

        // ���� �Ÿ� �̳��� ���� ����
        if(distance < velocity.sqrMagnitude / (2 * deceleration))
        {
            accelerationVector = -velocity.normalized * deceleration;   // ���� (���ӵ� ����)
        }

        // �ӵ� ������Ʈ
        velocity += accelerationVector * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed); // �ӵ� Ŭ����

        // ��ġ �̵�
        transform.position += velocity * Time.deltaTime;

        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // ���� ž��
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