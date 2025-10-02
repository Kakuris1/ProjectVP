using UnityEngine;
namespace Combat.Skills 
{ 
    public class UnityTimeSource : MonoBehaviour, ITimeSource 
    { 
        public float Now => Time.time; 
    } 
}
