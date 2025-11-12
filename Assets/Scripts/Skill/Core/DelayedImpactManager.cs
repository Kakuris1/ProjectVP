using UnityEngine;
using Combat.Skills;
using System.Collections.Generic;

// 씬에 동적으로 스폰될 헬퍼 스크립트
public class DelayedImpactManager : MonoBehaviour
{
    private SkillContext _ctx;
    private List<Transform> _targets;
    private bool _isInitialized = false;

    // 1. DeliveryAsset이 이 함수를 호출하여 초기화
    public void Initialize(in SkillContext ctx, List<Transform> targets)
    {
        _ctx = ctx;

        // SkillPipeline의 원본 리스트는 다음 프레임에 Clear()될 수 있으므로,
        // 반드시 타겟 리스트의 '사본'을 만들어 저장해야 함
        _targets = new List<Transform>(targets);
        _isInitialized = true;
    }

    // 2. 씬에 스폰된 직후 실행됨
    void Start()
    {
        if (!_isInitialized)
        {
            Debug.LogError("DelayedImpactManager가 Initialize되지 않았습니다!");
            Destroy(gameObject);
            return;
        }

        // 3. SkillSpecAsset에 정의된 skillDelay만큼 대기
        Invoke(nameof(ApplyImpacts), _ctx.Spec.skillDelay);
    }

    // 4. 딜레이 시간 경과 후 실행
    void ApplyImpacts()
    {
        if (_targets == null || _targets.Count == 0)
        {
            Destroy(gameObject); // 딜레이 동안 타겟이 사라졌을 수 있음
            return;
        }


        if (_ctx.Spec.impacts == null || _ctx.Spec.impacts.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        // maxTargetCount를 기반으로 실제 타겟 수 계산
        int count = (_ctx.Spec.maxTargetCount <= 0) ? _targets.Count : Mathf.Min(_targets.Count, _ctx.Spec.maxTargetCount);

        for (int i = 0; i < count; i++)
        {
            var t = _targets[i];
            if (t == null) continue; // 딜레이 동안 타겟이 죽었을 수 있음

            for (int j = 0; j < _ctx.Spec.impacts.Length; j++)
            {
                if (_ctx.Spec.impacts[j] != null)
                {
                    _ctx.Spec.impacts[j].Apply(_ctx, t);
                }
            }
        }
        // --- (임팩트 적용 로직 끝) ---

        // 5. 임무 완수 후 스스로 파괴
        Destroy(gameObject);
    }
}