using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform rt; Rect last; ScreenOrientation lastOri;
    void Awake() { rt = GetComponent<RectTransform>(); Apply(); }
    void Update() { if (last != Screen.safeArea || lastOri != Screen.orientation) Apply(); }
    void Apply()
    {
        var sa = Screen.safeArea; last = sa; lastOri = Screen.orientation;
        Vector2 min = sa.position, max = sa.position + sa.size;
        min.x /= Screen.width; min.y /= Screen.height;
        max.x /= Screen.width; max.y /= Screen.height;
        rt.anchorMin = min; rt.anchorMax = max;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }
}
