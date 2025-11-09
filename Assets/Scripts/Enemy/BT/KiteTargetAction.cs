using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "KiteTarget", story: "KiteTarget", category: "Action", id: "860ee3eaea380d7af7bf59137f34eda0")]
public partial class KiteTargetAction : Action
{
    public float kiteDistance = 20f;
    // MouseManager를 캐시할 변수
    private MouseManager mouseManager;
    protected override Status OnStart()
    {
        mouseManager = MouseManager.Instance;

        GameObject selfGO = this.GameObject;
        if (selfGO == null) return Status.Failure;

        EnemyInformation info = selfGO.GetComponent<EnemyInformation>();
        EnemyMovement movement = selfGO.GetComponent<EnemyMovement>();
        if (info == null || movement == null) return Status.Failure;

        return Status.Running; // OnUpdate로 즉시 넘어감
    }

    protected override Status OnUpdate()
    {
        // 전원 돌격 명령이 내려왔는지 확인
        if (mouseManager != null && mouseManager.overrideKitingAndAttack)
        {
            return Status.Failure;
        }

        GameObject selfGO = this.GameObject;
        if (selfGO == null) return Status.Failure;

        EnemyInformation info = selfGO.GetComponent<EnemyInformation>();
        EnemyMovement movement = selfGO.GetComponent<EnemyMovement>();
        if (info == null || movement == null) return Status.Failure;

        info.CanAttack = false; // 공격 중지
        movement.KiteTarget(20f); // C# 도구(함수) 호출

        // 이 행동은 끝나지 않고 "계속 실행되어야" 하므로 Running을 반환
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

