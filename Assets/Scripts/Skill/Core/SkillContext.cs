using UnityEngine;
using System.Collections.Generic;

namespace Combat.Skills
{
    // �ܺ� ���� ���(���� ����)
    public interface ITimeSource { float Now { get; } }
    public interface ISpawner
    {
        GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot);
        void SpawnOneShot(GameObject prefab, float scl, Vector3 pos, Quaternion rot);
    }
    public interface IResourceWallet
    {
        bool TryConsumeMana(float amount);
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

    // ��Ÿ�� �纻 (SpecAsset �� RuntimeSpec)
    public struct SkillRuntimeSpec
    {
        public float damage;
        public float cooldown;
        public float manaCost;

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

    // ���� ���ؽ�Ʈ (�� ���� ������ �ʿ��� ��� ��)
    public struct SkillContext
    {
        public Transform Caster;
        public Vector3 Origin;
        public Vector3 Direction;
        // spec�� ����ü - �� �����̹Ƿ� ���� ����
        public SkillRuntimeSpec Spec;

        public ITimeSource Time;
        public ISpawner Spawner;
        public IResourceWallet Wallet;
    }

    // ---- 4�ܰ� ������������ ���� �߻� SO ----

    // 1) Targeting: ����/��� �븱��
    public abstract class TargetingAsset : ScriptableObject
    {
        public abstract int AcquireTargets(in SkillContext ctx, List<Transform> results);
    }

    // 2) Delivery: ��� ��� ������(����/����ü/���ǡ�)
    public abstract class DeliveryAsset : ScriptableObject
    {
        // ����: ����ü���� �� �ȿ��� ������ �ϰ�, �浹 ������ Impact�� ȣ���ϴ� ������ ���� ����
        public abstract void Deliver(in SkillContext ctx, List<Transform> targets);
    }

    // 3) Impact: ����� �� � ȿ���� ����(�����/�˹�/�����̻�)
    public abstract class ImpactAsset : ScriptableObject
    {
        public abstract void Apply(in SkillContext ctx, Transform target);
    }

    // 4) Cost/Cooldown: �ڿ� �Ҹ���ٿ� ��å
    public abstract class CostAsset : ScriptableObject
    {
        // ��ȯ��: ���� ����/�Ұ�
        public abstract bool CheckAndConsume(in SkillContext ctx, float now, out float nextReadyTime);
    }
}
