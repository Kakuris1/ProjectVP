using UnityEngine;

public class CutsceneCameraController : MonoBehaviour
{
    public Transform Player;
    public Transform target;
    public float moveSpeed = 2f;

    public void StartCutscene(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void HandleCutscene()
    {
        if (target == null) return;

        transform.position = target.position;
        //transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * moveSpeed);
        transform.LookAt(Player);
    }
}