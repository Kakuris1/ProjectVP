using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class LaunchAtTargetOnce : MonoBehaviour
{
    [Tooltip("�÷��̾� Transform �Ҵ� (������ Player �±׸� �ڵ� �˻�)")]
    public Transform target;

    [Header("�߻� ����")]
    public float initialSpeed = 15f;        // ���� �ӵ� (m/s)
    public bool useGravity = true;           // �߷� ��� ����

    public GameObject thisgameobject;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = useGravity;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // �ͳθ� ����
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        // Ÿ�� �ڵ� �˻�(����)
        if (!target)
            target = GameObject.FindWithTag("Player")?.transform;

        if (!target)
        {
            Debug.LogWarning("[LaunchAtTargetOnce] target�� �����ϴ�. �ν����Ϳ��� �����ϰų� Player �±׸� ����ϼ���.");
            return;
        }

        // Ÿ���� ���� ���� ���� ����
        Vector3 dir = (target.position - transform.position).normalized;

        // �� ���� �ӵ� �ο�: ���� ��� �̵�
        rb.linearVelocity = dir * initialSpeed;
        // �Ǵ� ���� �� �ٷ� ���� �����ϰ� ��� �ӵ� ��ȭ(�� �� �ϳ� ��1):
        // rb.AddForce(dir * initialSpeed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //DestroyObject(thisgameobject);
    }
}
