using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

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
        [SerializeField] private InputActionReference fireAction; // �׼� �ʿ��� ����

        private ITimeSource TimeSrc => timeSourceMb as ITimeSource;
        private ISpawner Spawner => spawnerMb as ISpawner;
        private IResourceWallet Wallet => walletMb as IResourceWallet;

        private float nextReadyTime = 0f;

        private void OnEnable() { fireAction?.action.Enable(); }
        private void OnDisable() { fireAction?.action.Disable(); }

        public void Equip(SkillSpecAsset spec) => equippedSpecAsset = spec;

        private void Update()
        {
            TryCast();
        }

        private void TryCast()
        {
            if (equippedSpecAsset == null || pipeline == null) return;

            float now = TimeSrc?.Now ?? Time.time;
            if (now < nextReadyTime) return; // ��Ÿ�� üũ

            var spec = SkillRuntimeSpec.From(equippedSpecAsset);
            var ctx = new SkillContext
            {
                Caster = transform,
                Origin = transform.position + Vector3.up,
                Direction = transform.forward,
                Spec = spec,
                Time = TimeSrc,
                Spawner = Spawner,
                Wallet = Wallet
            };

            // �� ���� üũ �� ��� �Ҹ�
            if (!spec.costPolicy.CheckAndConsume(in ctx, now, out nextReadyTime)) return;

            pipeline.Execute(in ctx);
            Debug.Log("execute");
        }

    }
}
