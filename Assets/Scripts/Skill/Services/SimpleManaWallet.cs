using UnityEngine;

namespace Combat.Skills
{
    public class SimpleManaWallet : MonoBehaviour, IResourceWallet
    {
        [SerializeField] float max = 100f, current = 100f;
        public bool TryConsumeMana(float amount)
        {
            if (amount <= 0) return true;
            if (current < amount) return false;
            current -= amount; return true;
        }
    }
}