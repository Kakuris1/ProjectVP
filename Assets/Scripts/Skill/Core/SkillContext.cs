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
        public float damage;
        public float cooldown;
        public float manaCost;
        public float skillRange;

        public TargetingAsset targeting;
        public DeliveryAsset delivery;
        public ImpactAsset[] impacts;
        public CostAsset costPolicy;

        public GameObject castVfx, hitVfx;
        public float castVfxSize, hitVfxSize;

        public static SkillRuntimeSpec From(SkillSpecAsset src) => new SkillRuntimeSpec
        {
            damage = src.damage,
            cooldown = src.cooldown,
            manaCost = src.manaCost,
            skillRange = src.skillRange,
            targeting = src.targeting,
            delivery = src.delivery,
            impacts = src.impacts,
            costPolicy = src.costPolicy,
            castVfx = src.castVfx,
            hitVfx = src.hitVfx,
            castVfxSize = src.castVfxSize,
            hitVfxSize = src.hitVfxSize
        };
    }

    // 외부 서비스 계약(간단 버전)
    public interface ITimeSource { float Now { get; } }
    public interface ISpawner
    {
        GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot);
        void SpawnOneShot(GameObject prefab, float scl, Vector3 pos, Quaternion rot);
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
        // 주의: 투사체형은 이 안에서 스폰만 하고, 충돌 순간에 Impact를 호출하는 식으로 설계 가능
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
