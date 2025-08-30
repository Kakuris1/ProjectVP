using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class VisibilityFader : MonoBehaviour
{
    public Renderer[] renderers;          // 비우면 자동 수집
    public float fadeSeconds = 0.2f;      // 투명화 걸리는 시간
    public string colorProp = "_BaseColor"; // URP Lit
    float _target = 0f, _current = 0f;
    Coroutine _co;
    MaterialPropertyBlock _mpb;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();
        Apply(_current);
    }

    public void SetVisible(bool v)
    {
        _target = v ? 1f : 0f;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(Fade());
        Debug.Log("SetVisible : " + v);
    }

    IEnumerator Fade()
    {
        float start = _current, t = 0f;
        while (t < fadeSeconds)
        {
            t += Time.deltaTime;
            _current = Mathf.Lerp(start, _target, t / fadeSeconds);
            Apply(_current);
            yield return null;
        }
        _current = _target; Apply(_current);
    }

    void Apply(float a)
    {
        foreach (var r in renderers)
        {
            if (!r) continue;
            r.GetPropertyBlock(_mpb);
            if (r.sharedMaterial.HasProperty(colorProp))
            {
                var c = r.sharedMaterial.GetColor(colorProp);
                c.a = a; _mpb.SetColor(colorProp, c);
            }
            r.SetPropertyBlock(_mpb);
        }
    }
}
