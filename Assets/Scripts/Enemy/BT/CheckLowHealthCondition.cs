using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckLowHealth", story: "CheckLowHealth", category: "Conditions", id: "ead333fda135c5ccbfa1f4b559f28906")]
public partial class CheckLowHealthCondition : Condition
{
    // 컴포넌트를 캐시할 private 변수
    private EnemyInformation info;

    // IsTrue()는 매번 호출되므로, OnStart()에서 컴포넌트를 한 번만 찾아둡니다.
    public override void OnStart()
    {
        GameObject selfGO = this.GameObject;
        if (selfGO != null)
        {
            info = selfGO.GetComponent<EnemyInformation>();
        }
    }

    // ★★★ 핵심: 로직을 OnStart()가 아닌 IsTrue()로 이동 ★★★
    public override bool IsTrue()
    {
        if (info == null) return false; // info가 없으면 false

        // ★ 핵심 로직 ★
        if (info.CurrentHP / info.MaxHP < 0.3f)
        {
            return true; // 조건 만족 (true 반환)
        }

        return false; // 조건 불만족 (false 반환)
    }

    public override void OnEnd()
    {
        info = null; // 정리
    }
}
