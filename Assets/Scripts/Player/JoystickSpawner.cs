using UnityEngine;

// 씬에 빈 오브젝트 배치후 컴포넌트 연결
public class JoystickSpawner : MonoBehaviour
{
    public GameObject joystickPrefab;
    public Canvas canvas; // 메인 캔버스에 연결
    private MobileInput mobileInput;
    private PlayerMovement player;

    void Start()
    {
    #if UNITY_ANDROID || UNITY_IOS
        mobileInput = new MobileInput();
        player = FindObjectOfType<PlayerMovement>();
        player.SetInput(mobileInput);
    #endif  
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && touch.position.x < Screen.width / 2f)
            {
                SpawnJoystick(touch.position);
            }
        }
    }

    void SpawnJoystick(Vector2 position)
    {
        GameObject obj = Instantiate(joystickPrefab, canvas.transform);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.position = position;

        JoystickUI ui = obj.GetComponent<JoystickUI>();
        ui.Initialize(mobileInput); // 공유된 MobileInput 넘겨줌
    }
}