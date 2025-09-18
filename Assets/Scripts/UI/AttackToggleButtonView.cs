using UnityEngine;
using UnityEngine.UI;  // Graphic

public class AttackToggleButtonView : MonoBehaviour
{
    [SerializeField] private InputReader input;   // Player�� InputReader �巡��
    [SerializeField] private Graphic target;      // �ٲ� �̹��� (��� ��ư)
    [SerializeField] private Color onColor = new Color(1f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color offColor = new Color(1f, 1f, 1f, 1f);

    void Reset() { target = GetComponent<Graphic>(); }

    void LateUpdate()
    {
        if (!target || !input) return;
        target.color = input.AttackToggle ? onColor : offColor;
    }
}
