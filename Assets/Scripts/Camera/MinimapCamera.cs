using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Vector3 fixedRotation = new Vector3(90, 0, 0);

    void LateUpdate()
    {
        // 부모(플레이어)가 어떻게 회전하든 상관없이
        // 이 오브젝트의 회전 값을 우리가 원하는 fixedRotation으로 매 프레임 고정
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}