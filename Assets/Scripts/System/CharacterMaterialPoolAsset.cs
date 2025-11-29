using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Utilities/Material Pool")]
public class MaterialPoolAsset : ScriptableObject
{
    public List<Material> materials;
}