using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class JoystickUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    // ���, �ڵ� �̹��� ���� ����
    public RectTransform background;
    public RectTransform handle;

    // �ڵ� �̵� ����
    public float handleRange = 100f;

    private MobileInput mobileInput;

    private Vector2 input;

    // �ܺο��� MobileInput ����
    public void Initialize(MobileInput inputHandler)
    {
        mobileInput = inputHandler;
    }

    // ��ġ ���۽�
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);  // ��� �巡�� ó��
    }

    // �巡�� ���� �� ����
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            null,
            out pos
        );

        // �ڵ� ��ġ ���� ����
        pos = Vector2.ClampMagnitude(pos, handleRange);
        handle.anchoredPosition = pos;

        // 0~1 ���� ���� ���ͷ� ��ȯ
        input = pos / handleRange;

        // MobileInput���� �Է� ����
        mobileInput.SetJoystickInput(input);
    }

    // ���� �� ��
    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero; 
        input = Vector2.zero;
        mobileInput.SetJoystickInput(Vector2.zero);

        // ���̽�ƽ ����
        Destroy(gameObject);
    }
}
