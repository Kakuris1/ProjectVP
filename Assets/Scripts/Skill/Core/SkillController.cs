using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

namespace Combat.Skills
{
    public class SkillController : MonoBehaviour
    {
        [SerializeField] private SkillSpecAsset equippedSpecAsset;
        [SerializeField] private SkillPipeline pipeline;
        [Header("Input (New Input System)")]
        [SerializeField] private InputActionReference fireAction; // �׼� �ʿ��� ����

        //�������� ���� private �ʵ�
        private ITimeSource _timeSource;
        private ISpawner _spawner;

        private float nextReadyTime = 0f;

        private void Awake()
        {
            SkillManager manager = SkillManager.Instance;
            if (manager == null)
            {
                Debug.LogError("SkillManager.Instance not found in scene!");
                return;
            }
            _timeSource = manager.TimeSource;
            _spawner = manager.Spawner;
        }

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

            float now = _timeSource?.Now ?? Time.time;
            if (now < nextReadyTime) return; // ��Ÿ�� üũ

            var spec = SkillRuntimeSpec.From(equippedSpecAsset);
            var ctx = new SkillContext
            {
                Caster = transform,
                Origin = transform.position + Vector3.up,
                Direction = transform.forward,
                Spec = spec,
                Time = _timeSource,
                Spawner = _spawner
            };

            // �� ���� üũ �� ��� �Ҹ�
            if (!spec.costPolicy.CheckAndConsume(in ctx, now, out nextReadyTime)) return;

            pipeline.Execute(in ctx);
        }

    }
}
