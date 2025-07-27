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
    void LateUpdate()
    {
        switch (currentState)
        {
            case CameraState.TopDown:
                topDownFollow.HandleFollow();
                break;

            case CameraState.Cutscene:
                cutsceneController.HandleCutscene();
                break;
        }
    }

    public void ShakeCamera(float duration, float strength)
    {
        shakeHandler.TriggerShake(duration, strength);
    }

    public void SwitchToCutscene(Transform target)
    {
        currentState = CameraState.Cutscene;
        cutsceneController.StartCutscene(target);
    }

    public void SwitchToTopDown()
    {
        currentState = CameraState.TopDown;
    }
}