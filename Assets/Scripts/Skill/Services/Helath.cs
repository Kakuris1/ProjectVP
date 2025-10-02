using UnityEngine;

namespace Combat.Skills
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHp = 100f; float hp;
        void Awake() { hp = maxHp; }
        public void ApplyDamage(DamagePayload dmg)
        {
            hp -= dmg.amount;
            if (hp <= 0f) Destroy(gameObject);
        }
    }
}