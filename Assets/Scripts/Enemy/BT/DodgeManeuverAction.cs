using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DodgeManeuver", story: "DodgeManeuver", category: "Action", id: "76ea0b0198278a3419ed71b16ffb47f7")]
public partial class DodgeManeuverAction : Action
{
    private EnemyInformation info;
    private EnemyController controller;
    private EnemyMovement movement;
    protected override Status OnStart()
    {
        GameObject selfGO = this.GameObject;
        if (selfGO == null) return Status.Failure;

        info = selfGO.GetComponent<EnemyInformation>();
        controller = selfGO.GetComponent<EnemyController>();
        movement = selfGO.GetComponent<EnemyMovement>();
        if (info == null || controller == null || movement == null) return Status.Failure;

        // 처음 호출 되었을 경우
        info.CanAttack = false; // 공격 중지
        movement.PerformDodgeManeuver(); // 회피 코루틴 실행

        // "회피를 시작했음" (그리고 코루틴이 끝날 때까지 Running 상태 유지)
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 이미 회피 기동(코루틴)이 실행 중인지 확인
        if (controller.isDodging)
        {
            // 코루틴이 아직 안 끝났으므로 "계속 실행 중"
            return Status.Running;
        }
        else
        {
            // "회피 기동 완료!" -> Success 반환
            // (BT는 이 Success를 받고 3순위(DefaultAttack)로 넘어감)
            return Status.Success;
        }
    }

    protected override void OnEnd()
    {
    }
}

