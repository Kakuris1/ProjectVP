using UnityEngine;

public class WorldSpaceUI_LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 성능을 위해 시작할 때 메인 카메라를 찾아둡니다.
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // 캔버스가 항상 카메라를 정면으로 바라보게 합니다.
        // 카메라의 반대 방향을 바라보게(LookAt) 하거나, 카메라와 동일한 회전값을 갖게 합니다.
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);

        // 더 간단한 방법 (Z축이 뒤집힐 수 있음)
        // transform.rotation = mainCamera.transform.rotation;
    }
}