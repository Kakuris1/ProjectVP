using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckAttackCounter", story: "CheckAttackCounter", category: "Conditions", id: "a869f876e8834816675b607ad8935715")]
public partial class CheckAttackCounterCondition : Condition
{
    // 컴포넌트를 캐시할 private 변수
    private EnemyController controller;

    public override void OnStart()
    {
        GameObject selfGO = this.GameObject;
        if (selfGO != null)
        {
            controller = selfGO.GetComponent<EnemyController>();
        }
    }

    // ★★★ 핵심: 로직을 OnStart()가 아닌 IsTrue()로 이동 ★★★
    public override bool IsTrue()
    {
        if (controller == null) return false;

        // ★ 핵심 로직 ★
        if (controller.lungeAttackCounter >= 2)
        {
            return true; // 조건 만족 (true 반환)
        }

        return false; // 조건 불만족 (false 반환)
    }

    public override void OnEnd()
    {
        controller = null; // 정리
    }
}
