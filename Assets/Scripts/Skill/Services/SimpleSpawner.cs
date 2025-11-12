using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace Combat.Skills
{
    public class SimpleSpawner : MonoBehaviour, ISpawner
    {
        public GameObject Spawn(GameObject prefab, float scl, Vector3 pos, Quaternion rot) 
        {
            if (!prefab) return null;
            GameObject instance = Instantiate(prefab, pos, rot);
            instance.transform.localScale = new Vector3(scl, scl, scl);
            return instance;
        }
        public void SpawnOneShot(GameObject prefab, float scl,Vector3 pos, Quaternion rot) 
        {
            if (prefab)
            {
                GameObject instance = Instantiate(prefab, pos, rot);
                instance.transform.localScale = new Vector3(scl, scl, scl);

                // 임시방편으로 5초후 오브젝트 제거 후에 수정
                Destroy(instance, 5f);
            }
        }
        public void SpawnOneShot(GameObject prefab, float scl, Vector3 pos, Quaternion rot, float duration)
        {
            if (prefab)
            {
                GameObject instance = Instantiate(prefab, pos, rot);
                instance.transform.localScale = new Vector3(scl, scl, scl);

                // 전달받은 'duration' 값으로 오브젝트를 제거 (예: 1.0초)
                Destroy(instance, duration);
            }
        }
    }
}
