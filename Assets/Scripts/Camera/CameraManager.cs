using UnityEngine;

public enum CameraState { TopDown, Cutscene }   // 카메라 상태

public class CameraManager : MonoBehaviour
{
    // 싱글톤 카메라 매니저
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // 탑다운/컷씬 스크립트, 화면진동 스크립트 인스펙터 연결
    [Header("Modules")]
    public TopDownFollowCamera topDownFollow;
    public CutsceneCameraController cutsceneController;
    public CameraShakeHandler shakeHandler;

    //카메라 초기 상태 = Player 탑다운 뷰
    public CameraState currentState = CameraState.TopDown;
    void FixedUpdate()
    {
        if (currentState == CameraState.TopDown)
        {
            topDownFollow.HandleFollow();
        }
    }
    void Update()
    {
        if (currentState == CameraState.Cutscene)
        {
            cutsceneController.HandleCutscene();
        }
    }

    public void ShakeCamera(float duration, float strength)
    {
        shakeHandler.TriggerShake(duration, strength);
    }

    public void SwitchToCutscene(Transform target)
    {
        Debug.Log("컷씬 시작!");
        Time.timeScale = 0f; // 게임 일시 정지
        currentState = CameraState.Cutscene;
        cutsceneController.StartCutscene(target);
    }

    public void SwitchToTopDown()
    {
        Debug.Log("컷씬 종료!");
        Time.timeScale = 1f; // 게임 재개
        currentState = CameraState.TopDown;

        ////// 카메라를 플레이어 위치로 즉시 스냅
        //if (topDownFollow.target != null)
        //{
        //    transform.position = topDownFollow.target.position + topDownFollow.offset;
        //    topDownFollow.ResetVelocity(); // 팔로우 카메라의 속도 초기화 (순간이동 후 튀는 현상 방지)
        //}
    }
}