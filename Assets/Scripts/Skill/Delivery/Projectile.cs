// Projectile.cs
using UnityEngine;

namespace Combat.Skills
{
    [DisallowMultipleComponent]
    public class Projectile : MonoBehaviour
    {
        // 이동 파라미터
        float _speed;
        float _life;

        // 충돌/타격
        LayerMask _hitMask;
        bool _destroyOnHit;

        // 임팩트에 필요한 컨텍스트 스냅샷
        SkillRuntimeSpec _spec;
        Transform _caster;
        ISpawner _spawner;

        // 이동 방향(월드)
        Vector3 _dir;

        public void Init(in SkillContext ctx, Vector3 dir, float speed, float lifetime, LayerMask hitMask, bool destroyOnHit)
        {
            _spec = ctx.Spec;            // 런타임 사본(값 복사)
            _caster = ctx.Caster;        // 참조
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
            // 레이어 필터
            if (((1 << other.gameObject.layer) & _hitMask.value) == 0) return;

            // 맞은 대상에 임팩트 적용
            if (_spec.impacts != null && other.transform != null)
            {
                // Hit VFX
                if (_spec.hitVfx != null)
                    _spawner?.SpawnOneShot(_spec.hitVfx, _spec.hitVfxSize, other.transform.position, Quaternion.identity);

                for (int i = 0; i < _spec.impacts.Length; i++)
                {
                    // SkillContext를 즉석 생성해 넘김(필요한 최소 필드만 세팅)
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
