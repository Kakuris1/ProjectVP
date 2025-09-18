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

        [Header("Pipeline")]
        public TargetingAsset targeting;
        public DeliveryAsset delivery;
        public ImpactAsset[] impacts;
        public CostAsset costPolicy;

        [Header("FX")]
        public GameObject castVfx;
        public GameObject hitVfx;
    }
}
