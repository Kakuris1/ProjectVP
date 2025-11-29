using UnityEngine;
using System.Collections.Generic;

namespace Combat.Skills
{
    // 실행 컨텍스트 (한 번의 시전에 필요한 모든 것)
    public struct SkillContext
    {
        public Transform Caster;
        public Vector3 Origin;
        public Vector3 Direction;
        public ISkillTargetSensor TargetSensor;
        // spec은 구조체 - 값 전달이므로 변경 가능
        public SkillRuntimeSpec Spec;

        public ITimeSource Time;
        public ISpawner Spawner;
    }

    // 런타임 사본 (SpecAsset → RuntimeSpec)
    public struct SkillRuntimeSpec
    {
        public string skillName;
        public float damage;
        public float cooldown;
        public float manaCost;
        public float skillRange;
        public float skillDelay;
        public int maxTargetCount;
        public int pierceCount;

        public TargetingAsset targeting;
        public DeliveryAsset delivery;
        public ImpactAsset[] impacts;
        public CostAsset costPolicy;

        public GameObject castVfx, hitVfx, skillVfx;
        public float castVfxSize, hitVfxSize, skillVfxSize;

        public static SkillRuntimeSpec From(SkillSpecAsset src) => new SkillRuntimeSpec
        {
            skillName = src.name,
            damage = src.damage,
            cooldown = src.cooldown,
            manaCost = src.manaCost,
            skillRange = src.skillRange,
            skillDelay = src.skillDelay,
            maxTargetCount = src.maxTargetCount,
            pierceCount = src.pierceCount,
            targeting = src.targeting,
            delivery = src.delivery,
            impacts = src.impacts,
            costPolicy = src.costPolicy,
            castVfx = src.castVfx,
            hitVfx = src.hitVfx,
            skillVfx = src.skillVfx,
            castVfxSize = src.castVfxSize,
            hitVfxSize = src.hitVfxSize,
            skillVfxSize = src.skillVfxSize
        };
    }

    // 외부 서비스 계약(간단 버전)
    public interface ITimeSource { float Now { get; } }
    public interface ISpawner
    {
        GameObject Spawn(GameObject prefab, float scl, Vector3 pos, Quaternion rot);
        void SpawnOneShot(GameObject prefab, float scl, Vector3 pos, Quaternion rot);
        void SpawnOneShot(GameObject prefab, float scl, Vector3 pos, Quaternion rot, float duration);
        void Despawn(GameObject instance);
    }
    public interface IDamageable
    {
        void ApplyDamage(DamagePayload payload);
    }
    public struct DamagePayload
    {
        public float amount;
        public Vector3 hitPoint;
        public Transform source;
    }

    // ---- 4단계 파이프라인의 전략 추상 SO ----

    // 1) Targeting: 누구/어디를 노릴지
    public abstract class TargetingAsset : ScriptableObject
    {
        public abstract int AcquireTargets(in SkillContext ctx, List<Transform> targets);
    }

    // 2) Delivery: 어떻게 닿게 만들지(근접/투사체/장판…)
    public abstract class DeliveryAsset : ScriptableObject
    {
        public abstract void Deliver(in SkillContext ctx, List<Transform> targets);
    }

    // 3) Impact: 닿았을 때 어떤 효과를 줄지(대미지/넉백/상태이상…)
    public abstract class ImpactAsset : ScriptableObject
    {
        public abstract void Apply(in SkillContext ctx, Transform target);
    }

    // 4) Cost/Cooldown: 자원 소모·쿨다운 정책
    public abstract class CostAsset : ScriptableObject
    {
        // 반환값: 시전 가능/불가
        public abstract bool CheckAndConsume(in SkillContext ctx, float now, ref float nextReadyTime);
    }
}
