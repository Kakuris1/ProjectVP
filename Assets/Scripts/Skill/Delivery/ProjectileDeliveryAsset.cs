using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Projectile")]
    public class ProjectileDeliveryAsset : DeliveryAsset
    {
        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            // 1. 투사체 프리팹이 등록됐는지 확인
            if (ctx.Spec.skillVfx == null)
            {
                Debug.LogError($"SkillSpec '{ctx.Spec.skillName}'에 'Skill Vfx' (투사체 프리팹)가 없습니다!", ctx.Caster);
                return;
            }

            // 2. 시전자 위치에 CastVfx 스폰
            if (ctx.Spec.castVfx)
                ctx.Spawner?.SpawnOneShot(ctx.Spec.castVfx, ctx.Spec.castVfxSize, ctx.Origin, Quaternion.LookRotation(ctx.Direction));

            // 3. ISpawner.Spawn으로 '투사체 프리팹'을 씬에 스폰
            //    (SpawnOneShot 아님! Projectile.cs가 스스로 파괴를 관리함)
            GameObject projectileInstance = ctx.Spawner.Spawn(ctx.Spec.skillVfx, ctx.Spec.skillVfxSize, ctx.Origin, Quaternion.LookRotation(ctx.Direction));
            if (projectileInstance == null) return;

            // 4. 스폰된 투사체에서 Projectile 스크립트를 찾음
            if (projectileInstance.TryGetComponent<IProjectileLogic>(out var projectileLogic))
            {
                // 5. 투사체에게 SkillContext와 발사 방향을 전달하여 초기화
                projectileLogic.Initialize(ctx);
            }
            else
            {
                Debug.LogError($"'{ctx.Spec.skillVfx.name}' 프리팹에 'Projectile.cs' 스크립트가 없습니다!", ctx.Spec.skillVfx);
            }
        }
    }
}