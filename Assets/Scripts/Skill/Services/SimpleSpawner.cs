using UnityEngine;

namespace Combat.Skills
{
    public class SimpleSpawner : MonoBehaviour, ISpawner
    {
        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot) 
        {
            if (prefab) return null;

            GameObject instance = Instantiate(prefab, pos, rot);
            return instance;
        }
        public void SpawnOneShot(GameObject prefab, float scl,Vector3 pos, Quaternion rot) 
        {
            if (prefab)
            {
                GameObject instatnce = Instantiate(prefab, pos, rot);
                instatnce.transform.localScale = new Vector3(scl, scl, scl);

                // 임시방편으로 5초후 오브젝트 제거 후에 수정
                Destroy(instatnce, 5f);
            }
        }
    }
}
