using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DefaultAttack", story: "DefaultAttack", category: "Action", id: "1dd744682df1fc9154c2994c43ddebdc")]
public partial class DefaultAttackAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        GameObject selfGO = this.GameObject;
        if (selfGO == null) return Status.Failure;

        EnemyInformation info = selfGO.GetComponent<EnemyInformation>();
        EnemyMovement movement = selfGO.GetComponent<EnemyMovement>();
        if (info == null || movement == null) return Status.Failure;

        info.CanAttack = true; // 공격 허가!
        movement.HandleEngagingMovement(); // C# 도구(함수) 호출 -> 타겟 추격

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

