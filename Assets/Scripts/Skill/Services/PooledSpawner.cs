using System.Collections;
using Combat.Skills;
using UnityEngine;

// SimpleSpawner를 완벽하게 대체하는 풀링 기반 스포너
public class PooledSpawner : MonoBehaviour, ISpawner
{
    // 1. ISpawner.Spawn 구현
    public GameObject Spawn(GameObject prefab, float scl, Vector3 pos, Quaternion rot)
    {
        //Debug.Log($"ISpawner.Spawn");
        if (!prefab) return null;
        // PoolManager에게 스폰 요청
        GameObject instance = PoolManager.Instance.Spawn(prefab, scl, pos, rot);
        return instance;
    }

    // 2. ISpawner.Despawn 구현
    public void Despawn(GameObject instance)
    {
        //Debug.Log($"ISpawner.Despawn");
        if (instance == null) return;
        // PoolManager에게 디스폰(반납) 요청
        PoolManager.Instance.Despawn(instance);
    }

    // 3. ISpawner.SpawnOneShot (5초 버전) 구현
    public void SpawnOneShot(GameObject prefab, float scl, Vector3 pos, Quaternion rot)
    {
        if (prefab == null) return;
        GameObject instance = PoolManager.Instance.Spawn(prefab, scl, pos, rot);
        //Debug.Log($"ISpawner.SpawnOneShot (5초 버전)");
        // 5초 뒤에 '파괴'가 아닌 'Despawn(반납)'을 하도록 코루틴 실행
        StartCoroutine(DespawnAfterDelay(instance, 5f));
    }

    // 4. ISpawner.SpawnOneShot (지정 시간 버전) 구현
    public void SpawnOneShot(GameObject prefab, float scl, Vector3 pos, Quaternion rot, float duration)
    {
        if (prefab == null) return;
        GameObject instance = PoolManager.Instance.Spawn(prefab, scl, pos, rot);
        //Debug.Log($"ISpawner.SpawnOneShot (지정 시간 버전)");
        // 'duration'초 뒤에 Despawn(반납) 하도록 코루틴 실행
        StartCoroutine(DespawnAfterDelay(instance, duration));
    }

    // 5. 지정된 시간 뒤에 Despawn을 실행하는 코루틴
    private IEnumerator DespawnAfterDelay(GameObject instance, float delay)
    {
        //Debug.Log($"ISpawner.Despawn 지정 시간 후");
        yield return new WaitForSeconds(delay);

        // 딜레이가 끝났을 때 오브젝트가 아직 활성화 상태라면 (이미 반납되지 않았다면)
        if (instance != null && instance.activeInHierarchy)
        {
            PoolManager.Instance.Despawn(instance);
        }
    }
}