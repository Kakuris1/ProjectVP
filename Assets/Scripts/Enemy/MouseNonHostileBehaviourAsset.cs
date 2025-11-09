using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/Behavior/Mouse Behavior")]
public class MouseBehaviorAsset : NonHostileBehaviorAsset
{
    [Header("이 이상 가까워지면 도망")]
    public float fleeDistance = 10f; 
    [Header("이 이상 멀어지면 집합")]
    public float convergeDistance = 20f;
    private bool CheckRun = false;

    public override void Execute(EnemyInformation info, EnemyController control, EnemySensorSight sensor)
    {
        // 타겟이 없으면 할 게 없음
        if (info.CurrentTarget == null) return;

        // 거리 별 행동
        float distance = (info.transform.position - info.CurrentTarget.position).magnitude;
        if (distance < fleeDistance) // 멀리 도망 가야함
        {
            CheckRun = true;
            awayFromTarget(info, control);
        }
        else if (distance < convergeDistance) // 쳐다보면서 경계
        {
            if (CheckRun) // 반대로 도망가는 중
            {
                awayFromTarget(info, control); 
            }
            else // 쳐다보며 경계하는 중
            {
                info.transform.LookAt(info.CurrentTarget.position);
            }
        }
        else // 어느정도 멀어졌으니 생쥐들끼리 뭉침
        {
            CheckRun = false;
            control.OrderMoveTo(MouseManager.Instance.CurrentConvengePoint);
        }
    }

    private void awayFromTarget(EnemyInformation info, EnemyController control)
    {
        // 타겟으로 부터 멀어지는 방향으로 조금 앞 부분으로 도망
        Vector3 dir = (info.transform.position - info.CurrentTarget.position).normalized;
        Vector3 targetpos = info.transform.position + dir * 3;
        control.OrderMoveTo(targetpos);
    }
}