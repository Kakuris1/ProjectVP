using UnityEngine;

// "비적대적 상태일 때 어떤 행동을 할 것인가"에 대한 추상 전략
public abstract class NonHostileBehaviorAsset : ScriptableObject
{
    // Controller가 매 프레임 이 함수를 호출
    public abstract void Execute(EnemyInformation info, EnemyController control, EnemySensorSight sensor);
}