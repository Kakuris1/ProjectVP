using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    // 배열 값 : 몬스터 넘버
    // 0.Mouse 1.Conch 2.Urchin1 3.Urchin2
    public GameObject[] enemyPrefab;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject SpawnEnemy(int areaNum, int enemyNum, Vector3 spawnPosition)
    {
        GameObject enemyInstance = PoolManager.Instance.Spawn(enemyPrefab[enemyNum], spawnPosition, Quaternion.identity);

        EnemyInformation info = enemyInstance.GetComponent<EnemyInformation>();

        if (info != null)
        {
            info.Initialize(areaNum);
        }
        return enemyInstance;
    }
}