using UnityEngine;

public class Player : MonoBehaviour
{
    // 싱글톤 구조 Player
    public static Player Instance { get; private set; }

    // 오브젝트 생성 시 호출
    private void Awake()
    {
        // 생성 시 Instance가 이미 존재하면 생성 취소
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // 생성 시 Instance 에 등록
        Instance = this;
    }

    // 오브젝트 제거 시 호출
    private void OnDestroy()
    {
        // 싱글톤 인스턴스 해제
        if(Instance == this)
        {
            Instance = null;
        }
    }
}
