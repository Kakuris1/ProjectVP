using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckAttackCounter", story: "CheckAttackCounter", category: "Action", id: "824e4ec4c7e3755b96debc552414deb6")]
public partial class CheckAttackCounterAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        GameObject selfGO = this.GameObject;
        if (selfGO == null) return Status.Failure;

        EnemyController controller = selfGO.GetComponent<EnemyController>();
        if (controller == null) return Status.Failure;

        if (controller.lungeAttackCounter >= 2)
        {
            // 조건 만족 (회피해라!)
            return Status.Success;
        }
        // 조건 불만족 (아직 공격 더 해도 됨)
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

