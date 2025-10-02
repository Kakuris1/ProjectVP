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

                // �ӽù������� 5���� ������Ʈ ���� �Ŀ� ����
                Destroy(instatnce, 5f);
            }
        }
    }
}
