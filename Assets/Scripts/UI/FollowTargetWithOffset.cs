using UnityEngine;

/// <summary>
/// 3D 월드 공간에서 특정 타겟을 따라다니되,
/// 타겟의 회전이 아닌 '카메라'를 기준으로 한 상대적 위치(Offset)를 유지합니다.
/// (예: 항상 타겟의 '카메라 기준' 우측 하단에 위치)
/// 이 스크립트는 UI의 최상위 부모 (예: UI_Pivot)에 붙입니다.
/// </summary>
public class FollowTargetWithOffset : MonoBehaviour
{
    [Tooltip("따라다닐 대상 (플레이어, 적 등). ~Inforamtion.cs에서 자동으로 할당")]
    public Transform target;

    [Tooltip("기준이 될 메인 카메라 (비워두면 'MainCamera' 태그로 자동 검색)")]
    public Camera mainCamera;

    [Tooltip("카메라 시점 기준의 상대적 위치 (X:오른쪽, Y:위, Z:앞)")]
    public Vector3 cameraRelativeOffset = new Vector3(0.5f, -0.3f, 0f); // 우측 하단

    // 성능을 위해 Start에서 카메라를 찾아 저장
    void Start()
    {
        // "MainCamera" 태그를 가진 카메라 찾기
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            Debug.LogError("FollowTargetWithOffset: 'MainCamera' 태그를 가진 카메라를 찾을 수 없습니다. 인스펙터에서 수동으로 할당해주세요.");
        }
    }

    // 카메라와 타겟의 움직임이 모두 끝난 LateUpdate에서 위치를 계산해야
    // UI가 떨리거나 한 프레임 늦게 따라오는 현상이 없습니다.
    void LateUpdate()
    {
        // 타겟이 아직 할당되지 않았거나 파괴되었다면(예: 적 사망) 실행하지 않음
        if (target == null || mainCamera == null)
        {
            return;
        }

        // 1. 기준 위치 = 타겟(플레이어/적)의 현재 월드 위치
        Vector3 desiredPosition = target.position;

        // 2. 카메라의 '오른쪽(right)' 방향으로 offset.x 만큼 이동
        desiredPosition += mainCamera.transform.right * cameraRelativeOffset.x;

        // 3. 카메라의 '위쪽(up)' 방향으로 offset.y 만큼 이동
        desiredPosition += mainCamera.transform.up * cameraRelativeOffset.y;

        // 4. 카메라의 '앞쪽(forward)' 방향으로 offset.z 만큼 이동 (Z는 보통 0)
        desiredPosition += mainCamera.transform.forward * cameraRelativeOffset.z;

        // 5. 이 오브젝트(UI_Pivot)의 최종 위치를 계산된 위치로 설정
        transform.position = desiredPosition;
    }
}