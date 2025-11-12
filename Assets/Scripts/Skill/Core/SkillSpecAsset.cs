using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/SkillSpec")]
    public class SkillSpecAsset : ScriptableObject
    {
        [Header("Numbers")]
        public float damage = 20f;
        public float cooldown = 0.5f;
        public float manaCost = 10f;
        public float skillRange = 5f;
        [Tooltip("스킬 시전 후 실제 임팩트가 적용되기까지의 시간 (예: 유성 낙하 시간)")]
        public float skillDelay = 0f;
        [Tooltip("이 스킬이 영향을 줄 수 있는 최대 타겟 수. (0 이하는 무제한)")]
        public int maxTargetCount = 1;
        [Header("Projectile Settings")]
        [Tooltip("투사체 관통 횟수. (1 = 단일 타겟, 0 = 무한 관통)")]
        public int pierceCount = 1;

        [Header("Pipeline")]
        public TargetingAsset targeting;
        public DeliveryAsset delivery;
        public ImpactAsset[] impacts;
        public CostAsset costPolicy;

        [Header("시전자 위치에서 스폰되는 이펙트")]
        public GameObject castVfx;
        public float castVfxSize;
        [Header("피격자 위치에서 스폰되는 이펙트")]
        public GameObject hitVfx;
        public float hitVfxSize;
        [Header("스킬 자체의 메인 이펙트")]
        public GameObject skillVfx;
        public float skillVfxSize;
    }
}
