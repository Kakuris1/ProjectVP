using UnityEngine;

public class UnitUIContainer : MonoBehaviour
{
    public static UnitUIContainer Instance;
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
