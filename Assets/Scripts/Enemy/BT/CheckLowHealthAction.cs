using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckLowHealth", story: "CheckLowHealth", category: "Action", id: "3fab53e5e948df3590fabff6c6cd02e7")]
public partial class CheckLowHealthAction : Action
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
        if (info == null) return Status.Failure;

        if (info.CurrentHP < MouseManager.Instance.runAwayHP)
        {
            return Status.Success; // 조건 만족
        }
        return Status.Failure; // 조건 불만족
    }

    protected override void OnEnd()
    {
    }
}

