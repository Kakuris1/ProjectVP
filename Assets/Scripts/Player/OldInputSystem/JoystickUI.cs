using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class JoystickUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    // 배경, 핸들 이미지 연결 변수
    public RectTransform background;
    public RectTransform handle;

    // 핸들 이동 범위
    public float handleRange = 100f;

    private MobileInput mobileInput;

    private Vector2 input;

    // 외부에서 MobileInput 연결
    public void Initialize(MobileInput inputHandler)
    {
        mobileInput = inputHandler;
    }

    // 터치 시작시
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);  // 즉시 드래그 처리
    }

    // 드래그 중일 때 실행
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            null,
            out pos
        );

        // 핸들 위치 범위 제한
        pos = Vector2.ClampMagnitude(pos, handleRange);
        handle.anchoredPosition = pos;

        // 0~1 사이 비율 벡터로 변환
        input = pos / handleRange;

        // MobileInput으로 입력 전달
        mobileInput.SetJoystickInput(input);
    }

    // 손을 뗄 때
    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero; 
        input = Vector2.zero;
        mobileInput.SetJoystickInput(Vector2.zero);

        // 조이스틱 제거
        Destroy(gameObject);
    }
}
