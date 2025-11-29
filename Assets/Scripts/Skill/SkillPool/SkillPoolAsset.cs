using UnityEngine;
using System.Collections.Generic;
using Combat.Skills;

[CreateAssetMenu(menuName = "Combat/Skill Pool")]
public class SkillPoolAsset : ScriptableObject
{
    public List<SkillSpecAsset> skills;
}