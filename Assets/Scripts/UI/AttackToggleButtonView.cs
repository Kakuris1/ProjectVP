using UnityEngine;
using UnityEngine.UI;  // Graphic

public class AttackToggleButtonView : MonoBehaviour
{
    [SerializeField] private InputReader input;   // Player의 InputReader 드래그
    [SerializeField] private Graphic togglebutton;      // 바꿀 이미지 (토글 버튼)
    [SerializeField] private bool _previousToggleState;
    [SerializeField] private Color onColor = new Color(1f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color offColor = new Color(1f, 1f, 1f, 1f);

    void Reset() { togglebutton = GetComponent<Graphic>(); }
    void Start()
    {
        if (!togglebutton || !input) return;

        // 현재 상태로 초기화
        _previousToggleState = input.AttackToggle;
        togglebutton.color = _previousToggleState ? onColor : offColor;
    }

    void LateUpdate()
    {
        if (!togglebutton || !input) return;
        bool currentToggleState = input.AttackToggle;

        if (currentToggleState == _previousToggleState) return;
        _previousToggleState = currentToggleState;
        TeamManager.Instance.ToggleCombatMode(currentToggleState);
        if (currentToggleState)
        {   // 전투 모드시
            togglebutton.color = onColor;
        }
        else
        {   // 비전투 모드시
            togglebutton.color = offColor;
        }
    }
}
