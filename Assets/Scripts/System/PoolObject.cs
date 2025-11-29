using UnityEngine;

// 이 스크립트는 PoolManager가 런타임에 자동으로 추가합니다.
public class PooledObject : MonoBehaviour
{
    // 나의 '원본 프리팹'이 무엇인지 저장하는 변수
    public GameObject OriginalPrefab { get; private set; }

    public void SetOriginalPrefab(GameObject prefab)
    {
        OriginalPrefab = prefab;
    }
}