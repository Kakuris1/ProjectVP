using UnityEngine;
using UnityEngine.InputSystem;

namespace Combat.Skills
{
    public class SkillController : MonoBehaviour
    {
        [SerializeField] private SkillSpecAsset equippedSpecAsset;
        [SerializeField] private SkillPipeline pipeline;

        [Header("Runtime Services")]
        [SerializeField] private MonoBehaviour timeSourceMb;   // ITimeSource
        [SerializeField] private MonoBehaviour spawnerMb;     // ISpawner
        [SerializeField] private MonoBehaviour walletMb;      // IResourceWallet

        [Header("Input (New Input System)")]
        [SerializeField] private InputActionReference fireAction; // 액션 맵에서 연결

        private ITimeSource TimeSrc => timeSourceMb as ITimeSource;
        private ISpawner Spawner => spawnerMb as ISpawner;
        private IResourceWallet Wallet => walletMb as IResourceWallet;

        private float nextReadyTime;

        private void OnEnable() { fireAction?.action.Enable(); }
        private void OnDisable() { fireAction?.action.Disable(); }

        public void Equip(SkillSpecAsset spec) => equippedSpecAsset = spec;

        private void Update()
        {
            if (fireAction != null && fireAction.action.WasPerformedThisFrame())
            {
                TryCast();
            }
        }

        private void TryCast()
        {
            if (equippedSpecAsset == null || pipeline == null) return;

            var spec = SkillRuntimeSpec.From(equippedSpecAsset);

            var ctx = new SkillContext
            {
                Caster = transform,
                Origin = transform.position + Vector3.up * 1.0f,
                Direction = transform.forward,
                Spec = spec,
                Time = TimeSrc,
                Spawner = Spawner,
                Wallet = Wallet
            };

            // 통합 쿨다운(코스트 정책 내부에서 nextReadyTime을 갱신)
            if (TimeSrc != null && TimeSrc.Now < nextReadyTime) return;

            pipeline.Execute(in ctx);
        }
    }
}
