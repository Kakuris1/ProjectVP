using UnityEngine;
using UnityEngine.UI;  // Graphic

public class AttackToggleButtonView : MonoBehaviour
{
    [SerializeField] private InputReader input;   // Player의 InputReader 드래그
    [SerializeField] private Graphic target;      // 바꿀 이미지 (토글 버튼)
    [SerializeField] private Color onColor = new Color(1f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color offColor = new Color(1f, 1f, 1f, 1f);

    void Reset() { target = GetComponent<Graphic>(); }

    void LateUpdate()
    {
        if (!target || !input) return;
        target.color = input.AttackToggle ? onColor : offColor;
    }
}
