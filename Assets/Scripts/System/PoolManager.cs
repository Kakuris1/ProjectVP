using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 1. 싱글톤 설정
    public static PoolManager Instance { get; private set; }

    // 2. 풀링 '창고'
    //    Key = 프리팹 (원본)
    //    Value = 해당 프리팹으로 만든 인스턴스(비활성화된) 목록
    public Dictionary<GameObject, Queue<GameObject>> _poolDictionary;

    // 3. 풀링된 오브젝트들이 보관될 부모 Transform (씬 하이어라키 정리용)
    private Transform _poolParent;

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            _poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

            // "@PooledObjects" 라는 이름의 빈 오브젝트를 생성하고,
            // 모든 비활성화 오브젝트를 이 자식으로 넣어 씬을 깔끔하게 유지
            _poolParent = new GameObject("@PooledObjects").transform;
            DontDestroyOnLoad(_poolParent.gameObject); // (선택적) 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    // 4. 오브젝트 '스폰' (꺼내기)
    public GameObject Spawn(GameObject prefab, float scl, Vector3 position, Quaternion rotation)
    {
       // Debug.Log("PoolManager Spawn 호출");
        // 1. 이 프리팹에 대한 풀(Queue)이 존재하는지 확인
        if (!_poolDictionary.TryGetValue(prefab, out Queue<GameObject> poolQueue))
        {
            // 2. 풀이 없으면, 새로 Queue를 만들어서 사전에 추가
            poolQueue = new Queue<GameObject>();
            _poolDictionary.Add(prefab, poolQueue);
        }

        GameObject instance;

        // 3. 풀(Queue)에 비활성화된 인스턴스가 있는지 확인
        if (poolQueue.Count > 0)
        {
            // 4. 있으면: 풀에서 하나를 꺼냄 (Dequeue)
            instance = poolQueue.Dequeue();

            //Debug.Log($"[PoolManager] <color=green>재사용:</color> {prefab.name}", instance);
        }
        else
        {
            // 5. 없으면: 새로 Instantiate (이때만 GC 발생)
            instance = Instantiate(prefab);
            // 새로 생성된거면 사이즈 초기화
            Vector3 size = instance.transform.localScale;
            instance.transform.localScale = new Vector3(size.x * scl, size.y * scl, size.z * scl);
            // 6. 새로 만들었다면, 이 오브젝트의 '출신(원본 프리팹)'을
            //    기억하기 위해 PooledObject 헬퍼 컴포넌트를 붙임
            PooledObject helper = instance.AddComponent<PooledObject>();
            helper.SetOriginalPrefab(prefab);

            //Debug.Log($"[PoolManager] <color=yellow>신규 생성:</color> {prefab.name}", instance);
        }

        // 7.오브젝트를 @PooledObjects에서 분리
        instance.transform.SetParent(null);
        // 8. 오브젝트를 활성화하고 위치와 회전을 설정하여 씬에 배치
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);

        // 9. 씬 루트가 아닌 PoolManager 자식으로 이동 (정리용 - 선택적)
        //instance.transform.SetParent(this.transform);

        return instance;
    }

    // 5. 오브젝트 '디스폰' (반납하기)
    public void Despawn(GameObject instance)
    {
        //Debug.Log($"poolManager Despawn호출");
        if (instance == null) return;

        // 1. 비활성화
        instance.SetActive(false);

        // 2. 이 오브젝트의 '출신(원본 프리팹)'을 헬퍼에게 물어봄
        PooledObject helper = instance.GetComponent<PooledObject>();
        if (helper == null || helper.OriginalPrefab == null)
        {
            // 풀링된 오브젝트가 아니면(예: 씬에 수동 배치된 StaticZone)
            // 그냥 파괴
            Destroy(instance);
            return;
        }
        // 3. '출신'에 맞는 풀(Queue)을 찾아서 다시 넣음 (Enqueue)
        _poolDictionary[helper.OriginalPrefab].Enqueue(instance);

        // 4. @PooledObjects 자식으로 이동 (정리용)
        instance.transform.SetParent(_poolParent);
    }
}