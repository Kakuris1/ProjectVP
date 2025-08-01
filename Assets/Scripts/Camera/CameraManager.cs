using UnityEngine;

public enum CameraState { TopDown, Cutscene }   // ī�޶� ����

public class CameraManager : MonoBehaviour
{
    // �̱��� ī�޶� �Ŵ���
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // ž�ٿ�/�ƾ� ��ũ��Ʈ, ȭ������ ��ũ��Ʈ �ν����� ����
    [Header("Modules")]
    public TopDownFollowCamera topDownFollow;
    public CutsceneCameraController cutsceneController;
    public CameraShakeHandler shakeHandler;

    //ī�޶� �ʱ� ���� = Player ž�ٿ� ��
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