using Combat.Skills;
using System;
using UnityEngine;

public class MouseInformation : EnemyInformation
{
    public override void Initialize(int AreaNumber, Vector3 spawnPos)
    {
        base.Initialize(AreaNumber, spawnPos);
        Hostile = false; // 생쥐만 적대적 초기값이 달라서 추가
    }

    public override void SetTarget(Transform newTarget)
    {
        CurrentTarget = newTarget;
        Debug.Log("플레이어 만남");
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