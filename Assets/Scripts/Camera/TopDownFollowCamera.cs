using UnityEngine;

public class TopDownFollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10f, -10f);
    public float smoothSpeed = 5f;

    public void HandleFollow()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
        transform.position = smoothedPos;
        transform.LookAt(target);
    }
}