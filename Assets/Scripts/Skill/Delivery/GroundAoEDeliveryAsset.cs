using System.Collections.Generic;
using UnityEngine;

namespace Combat.Skills
{
    [CreateAssetMenu(menuName = "Combat/Delivery/Ground AoE (Spawn Prefab)")]
    public class GroundAoEDeliveryAsset : DeliveryAsset
    {
        public override void Deliver(in SkillContext ctx, List<Transform> targets)
        {
            // 타겟이 없거나, 스킬VFX가 등록 안 됐으면 중단
            if (targets == null || targets.Count == 0 || targets[0] == null) return;
            if (ctx.Spec.skillVfx == null)
            {
                Debug.LogError($"SkillSpec '{ctx.Spec.skillName}'에 'Skill Vfx' (장판 프리팹)가 없습니다!", ctx.Caster);
                return;
            }

            // 캐스트 VFX (시전 타이밍)
            Vector3 spawnPos = ctx.Origin + ctx.Direction * 1.5f;
            if (ctx.Spec.castVfx != null)
                ctx.Spawner?.SpawnOneShot(ctx.Spec.castVfx, ctx.Spec.castVfxSize, spawnPos, Quaternion.LookRotation(ctx.Direction), 1f);

            // 장판이 설치될 위치 = 첫 번째 타겟의 위치
            Vector3 spawnPosition = targets[0].position;
            
            // '장판 프리팹'을 씬에 스폰 (SpawnOneShot 아님!)
            GameObject aoeInstance = ctx.Spawner.Spawn(ctx.Spec.skillVfx, ctx.Spec.skillVfxSize, spawnPosition, Quaternion.identity);
            if (aoeInstance == null) return;
            
            // 스폰된 장판 프리팹에서 GroundAoE 스크립트를 찾음
            if (aoeInstance.TryGetComponent<GroundAoE>(out var aoeLogic))
            {
                // 장판 로직에게 SkillContext를 전달하여 초기화
                aoeLogic.Initialize(ctx);
            }
            else
            {
                Debug.LogError($"'{ctx.Spec.skillVfx.name}' 프리팹에 'GroundAoE.cs' 스크립트가 없습니다!", ctx.Spec.skillVfx);
            }
        }
    }
}