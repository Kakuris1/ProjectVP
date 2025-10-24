// Projectile.cs
using UnityEngine;

namespace Combat.Skills
{
    [DisallowMultipleComponent]
    public class Projectile : MonoBehaviour
    {
        // �̵� �Ķ����
        float _speed;
        float _life;

        // �浹/Ÿ��
        LayerMask _hitMask;
        bool _destroyOnHit;

        // ����Ʈ�� �ʿ��� ���ؽ�Ʈ ������
        SkillRuntimeSpec _spec;
        Transform _caster;
        ISpawner _spawner;

        // �̵� ����(����)
        Vector3 _dir;

        public void Init(in SkillContext ctx, Vector3 dir, float speed, float lifetime, LayerMask hitMask, bool destroyOnHit)
        {
            _spec = ctx.Spec;            // ��Ÿ�� �纻(�� ����)
            _caster = ctx.Caster;        // ����
            _spawner = ctx.Spawner;

            _dir = dir;
            _speed = speed;
            _life = lifetime;

            _hitMask = hitMask;
            _destroyOnHit = destroyOnHit;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            transform.position += _dir * _speed * dt;

            _life -= dt;
            if (_life <= 0f) Despawn();
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Enter");
            // ���̾� ����
            if (((1 << other.gameObject.layer) & _hitMask.value) == 0) return;

            // ���� ��� ����Ʈ ����
            if (_spec.impacts != null && other.transform != null)
            {
                // Hit VFX
                if (_spec.hitVfx != null)
                    _spawner?.SpawnOneShot(_spec.hitVfx, _spec.hitVfxSize, other.transform.position, Quaternion.identity);

                for (int i = 0; i < _spec.impacts.Length; i++)
                {
                    // SkillContext�� �Ｎ ������ �ѱ�(�ʿ��� �ּ� �ʵ常 ����)
                    var hitCtx = new SkillContext
                    {
                        Caster = _caster,
                        Origin = transform.position,
                        Direction = _dir,
                        Spec = _spec,
                        Spawner = _spawner,
                        Time = null
                    };
                    _spec.impacts[i].Apply(hitCtx, other.transform);
                }
            }

            if (_destroyOnHit) Despawn();
        }

        void Despawn()
        {
            Destroy(gameObject);
        }
    }
}
