using UnityEngine;

namespace Combat.Skills
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHp = 100f; float hp;
        private EnemyDie enemyDie;

        private void Awake() 
        { 
            hp = maxHp;
            enemyDie = GetComponent<EnemyDie>();
        }
        public void ApplyDamage(DamagePayload dmg)
        {
            hp -= dmg.amount;
            Debug.Log($"Hit, {enemyDie.EnemyID} HP : {hp}");
            if (hp <= 0f)
            {
                enemyDie.Die();
                Destroy(gameObject);
            }
        }
    }
}