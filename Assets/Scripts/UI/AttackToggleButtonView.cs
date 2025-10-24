using UnityEngine;
using UnityEngine.UI;  // Graphic

public class AttackToggleButtonView : MonoBehaviour
{
    [SerializeField] private InputReader input;   // Player�� InputReader �巡��
    [SerializeField] private Graphic togglebutton;      // �ٲ� �̹��� (��� ��ư)
    [SerializeField] private bool _previousToggleState;
    [SerializeField] private Color onColor = new Color(1f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color offColor = new Color(1f, 1f, 1f, 1f);

    void Reset() { togglebutton = GetComponent<Graphic>(); }
    void Start()
    {
        if (!togglebutton || !input) return;

        // ���� ���·� �ʱ�ȭ
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
        {   // ���� ����
            togglebutton.color = onColor;
        }
        else
        {   // ������ ����
            togglebutton.color = offColor;
        }
    }
}
