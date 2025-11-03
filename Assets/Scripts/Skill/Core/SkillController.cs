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
        [SerializeField] private InputActionReference fireAction; // 액션 맵에서 연결

        private ISkillTargetSensor _TargetSensor;

        //의존성을 받을 private 필드
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
            pipeline = GetComponent<SkillPipeline>();
            _TargetSensor = GetComponentInChildren<ISkillTargetSensor>();
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
            var spec = SkillRuntimeSpec.From(equippedSpecAsset);
            var ctx = new SkillContext
            {
                Caster = transform,
                Origin = transform.position + Vector3.up,
                Direction = transform.forward,
                TargetSensor = _TargetSensor,
                Spec = spec,
                Time = _timeSource,
                Spawner = _spawner
            };

            float now = _timeSource?.Now ?? Time.time;
            if (now < nextReadyTime) return; // 쿨타임 체크
            // ▼ 조건 체크 및 비용 소모
            if (!spec.costPolicy.CheckAndConsume(in ctx, now, ref nextReadyTime)) return;

            pipeline.Execute(in ctx);
        }

    }
}
