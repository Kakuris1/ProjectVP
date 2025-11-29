using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    public static EnemyContainer Instance;
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
}
