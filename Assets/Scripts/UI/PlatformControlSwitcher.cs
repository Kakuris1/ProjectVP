using UnityEngine;

public class PlatformControlSwitcher : MonoBehaviour
{
    [Header("Platform-Specific Containers")]
    [SerializeField] private GameObject m_pcControls;     // PC_Controls 그룹 연결
    [SerializeField] private GameObject m_mobileControls; // Mobile_Controls 그룹 연결

    void Awake()
    {
        // 현재 플랫폼이 모바일(Android, iOS)인지 알려줌
        if (Application.isMobilePlatform)
        {
            // 모바일이면 PC UI 끄고, 모바일 UI 켠다.
            m_pcControls.SetActive(false);
            m_mobileControls.SetActive(true);
        }
        else
        {
            // PC UI 켜고, 모바일 UI 끔
            m_pcControls.SetActive(true);
            m_mobileControls.SetActive(false);
        }

        // Common_Controls는 스크립트에서 아예 건드리지 않으므로
        // 항상 활성화된 상태를 유지
        
    }
}