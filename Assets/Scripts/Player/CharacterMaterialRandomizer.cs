using UnityEngine;

/// <summary>
/// OnEnable 시점에 이 오브젝트의 SkinnedMeshRenderer를 찾아
/// 지정된 MaterialPoolAsset에서 랜덤 머티리얼을 적용합니다.
/// </summary>
[RequireComponent(typeof(SkinnedMeshRenderer))] // [수정] SkinnedMeshRenderer로 변경
public class SimpleMaterialRandomizer : MonoBehaviour
{
    [SerializeField]
    [Tooltip("사용할 머티리얼 목록이 담긴 ScriptableObject")]
    private MaterialPoolAsset materialPool;

    // 이 스크립트가 붙어있는 오브젝트의 렌더러
    private SkinnedMeshRenderer targetRenderer;

    void Awake()
    {
        // 1. 자기 자신의 SkinnedMeshRenderer 컴포넌트를 찾습니다.
        targetRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    // OnEnable은 씬 시작 시(Player) 또는 풀에서 Spawn 시(Ally) 모두 호출됩니다.
    void OnEnable()
    {
        ApplyRandomMaterial();
    }

    private void ApplyRandomMaterial()
    {
        if (targetRenderer == null || materialPool == null || materialPool.materials.Count == 0)
        {
            return; // 적용할 대상이 없거나, 머티리얼 풀이 비어있으면 중단
        }

        // 1. 머티리얼 풀에서 랜덤 인덱스를 뽑습니다.
        int randomIndex = Random.Range(0, materialPool.materials.Count);
        Material randomMat = materialPool.materials[randomIndex];

        // 2. 렌더러의 'sharedMaterial'을 교체합니다.
        // ('.material'은 새 인스턴스를 만들어 메모리 누수가 발생할 수 있습니다)
        if (randomMat != null)
        {
            targetRenderer.sharedMaterial = randomMat;
        }
    }
}