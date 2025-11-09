using Combat.Skills;
using System;
using UnityEngine;

public class MouseInformation : EnemyInformation
{
    public override void SetTarget(Transform newTarget)
    {
        CurrentTarget = newTarget;

        if (firstTimeToMeet && newTarget != null)
        {
            MouseManager.Instance.MeetPlayer(this);
            firstTimeToMeet = false;
        }
    }
    public override void Internal_Die()
    {
        if (IsDead) return; // 중복 실행 방지
        MouseManager.Instance.RemoveMouse(this);
        base.Internal_Die();
    }
}