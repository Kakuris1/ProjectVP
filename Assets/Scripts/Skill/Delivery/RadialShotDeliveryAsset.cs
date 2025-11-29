using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Radial Projectile Shot")]
    public class RadialShotDeliveryAsset : DeliveryAsset
    {
        [Tooltip("한 번에 발사할 투사체 개수 (예: 8)")]
        public int projectileCount = 8;

        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            // 1. 투사체 프리팹 확인
            if (ctx.Spec.skillVfx == null)
            {
                Debug.LogError($"SkillSpec '{ctx.Spec.skillName}'에 'Skill Vfx' (투사체 프리팹)가 없습니다!", ctx.Caster);
                return;
            }

            // 2. 시전자 위치에 CastVfx 스폰
            if (ctx.Spec.castVfx)
                ctx.Spawner.SpawnOneShot(ctx.Spec.castVfx, ctx.Spec.castVfxSize, ctx.Origin, Quaternion.LookRotation(ctx.Direction), 1.0f); // 1초 지속 예시

            float angleStep = 360f / projectileCount;
            // 3. 8방향(projectileCount)으로 투사체 발사
            for (int i = 0; i < projectileCount; i++)
            {
                float currentAngle = angleStep * i;
                Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);

                Vector3 setY;
                if (ctx.Origin.y > 1.5)
                {
                    setY = ctx.Origin - new Vector3(0, 1f, 0);
                }
                else setY = ctx.Origin;
                // 4. 스포너로 투사체 스폰 (풀링 사용)
                GameObject projectileInstance = ctx.Spawner.Spawn(ctx.Spec.skillVfx, ctx.Spec.skillVfxSize, setY, rotation);
                if (projectileInstance == null) continue;
                // 5. 투사체 헬퍼 스크립트(Projectile.cs) 초기화
                // (ProjectileDeliveryAsset과 동일한 로직, 하지만 IProjectileLogic 사용 권장 - 아래 2번 항목 참조)
                if (projectileInstance.TryGetComponent<IProjectileLogic>(out var projectileLogic))
                {
                    projectileLogic.Initialize(ctx);
                }
                else
                {
                    Debug.LogError($"'{ctx.Spec.skillVfx.name}' 프리팹에 'Projectile.cs' 스크립트가 없습니다!", ctx.Spec.skillVfx);
                }
            }
        }
    }
}