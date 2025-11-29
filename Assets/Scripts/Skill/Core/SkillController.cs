using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace Combat.Skills
{
    public class SkillController : MonoBehaviour
    {
        [SerializeField] private SkillSpecAsset equippedSpecAsset;
        [SerializeField] private SkillPipeline pipeline;

        private ISkillTargetSensor _TargetSensor;

        //의존성을 받을 private 필드
        private ITimeSource _timeSource;
        private ISpawner _spawner;

        [Header("스킬 쿨타임")]
        [SerializeField] private float nextReadyTime;

        [Header("Input (New Input System)")]
        [SerializeField] private InputActionReference fireAction; // 액션 맵에서 연결

        // 스킬 게이지 UI용 필드
        private Slider _skillGauge;
        private float _currentCooldownDuration = 0f; // 쿨타임 진행률
        private float _cooldownStartTime = 0f;       // 최근 스킬 시전 시간

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
            nextReadyTime = 1f;
        }

        private void OnEnable() { fireAction?.action.Enable(); }
        private void OnDisable() { fireAction?.action.Disable(); }

        public void Equip(SkillSpecAsset spec) => equippedSpecAsset = spec;

        private void Update()   
        {
            TryCast(); // 스킬 시전 시도
            UpdateSkillGaugeUI(); // 스킬 게이지 갱신
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
            // 쿨타임 진행률 갱신
            _currentCooldownDuration = (now - _cooldownStartTime) / (nextReadyTime - _cooldownStartTime); 
            // 조건 체크 및 비용 소모
            if (!spec.costPolicy.CheckAndConsume(in ctx, now, ref nextReadyTime)) return;
            // 스킬 시전
            pipeline.Execute(in ctx);
            // 스킬 시전 성공 시간 갱신
            _cooldownStartTime = now;
        }


        // Information이 호출하여 스킬 게이지 슬라이더를 넘김
        public void SetSkillGauge(Slider gauge)
        {
            _skillGauge = gauge;
            if (_skillGauge != null)
            {
                _skillGauge.value = 1.0f; // 초기값 1 (꽉 참)
            }
        }

        private void UpdateSkillGaugeUI()
        {
            if (_skillGauge == null) return; // UI가 없으면 종료
            if (_currentCooldownDuration >= 1f)
            {   // 쿨타임 돌면 1 고정
                _skillGauge.value = 1f;
            }
            else
            {   // 쿨타임 게이지 갱신
                _skillGauge.value = _currentCooldownDuration;
            }
        }
    }
}
