using Unity.VisualScripting;
using UnityEngine;
// �Ʊ� ���� �̺�Ʈ ���� Ŭ����
public class RecruitManager : MonoBehaviour
{
    private AllyInformation Info;

    private void Awake()
    {
        Info = GetComponent<AllyInformation>();
    }

    private void OnEnable()
    {
        EventManager.Instance.OnAreaCleared += HandleAreaCleared;
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnAreaCleared -= HandleAreaCleared;
        }
    }

    private void HandleAreaCleared(int AreaNumber)
    {
        if(AreaNumber == Info.targetAreaNumber) { Recruit(); }
    }

    // ���� ����
    private void Recruit()
    {
        // 0. ���Խ� �߻� �̺�Ʈ

        // 1. TeamManager�� �ڽ��� �Ʊ����� ����ش޶�� ��û
        TeamManager.Instance.RecruitAlly(Info);

        // 2. �� ������ ���¸� '�Ʊ�'���� ����
        // ��: info.ChangeFaction(Faction.Player);

        // 3. ���� �� �ʿ��� ������Ʈ Ȱ��ȭ
        GetComponent<AllyController>().enabled = true;
        GetComponent<AllyMovement>().enabled = true;

        Debug.Log($"{name} ���� ���� �Ϸ�!");

        // 4. ���� �̺�Ʈ�� �� ���� �Ͼ�� �ϹǷ�, �� ��ũ��Ʈ�� ��Ȱ��ȭ
        this.enabled = false;
    }
}
