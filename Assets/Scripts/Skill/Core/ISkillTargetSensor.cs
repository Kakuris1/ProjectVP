using System.Collections.Generic;
using UnityEngine;
// Player 와 Ally 의 Sensor 시스템이 상속
public interface ISkillTargetSensor
{
    List<Transform> GetCurrentTargetList();
    Transform GetNearestTarget();
}
