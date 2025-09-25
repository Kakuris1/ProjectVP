using UnityEngine;

namespace Combat.Skills
{
    public class SimpleSpawner : MonoBehaviour, ISpawner
    {
        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot) => prefab ? Instantiate(prefab, pos, rot) : null;
        public void SpawnOneShot(GameObject prefab, Vector3 pos, Quaternion rot) { if (prefab) Instantiate(prefab, pos, rot); }
    }
}
