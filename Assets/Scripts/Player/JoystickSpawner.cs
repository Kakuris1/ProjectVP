using UnityEngine;

// ���� �� ������Ʈ ��ġ�� ������Ʈ ����
public class JoystickSpawner : MonoBehaviour
{
    public GameObject joystickPrefab;
    public Canvas canvas; // ���� ĵ������ ����
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
        ui.Initialize(mobileInput); // ������ MobileInput �Ѱ���
    }
}