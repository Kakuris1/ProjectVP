using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class VisibilityFader : MonoBehaviour
{
    public Renderer[] renderers;          // 비우면 자동 수집

    [Header("페이드 속도 (초)")]
    [Tooltip("더 빠르게 (보일 때)")]
    public float fadeInDuration = 0.15f;
    [Tooltip("더 천천히 (사라질 때)")]
    public float fadeOutDuration = 1f;

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

        // '보이기'(v=true)일 경우 fadeInDuration을, '숨기기'(v=false)일 경우 fadeOutDuration을 선택
        float duration = v ? fadeInDuration : fadeOutDuration;

        // Fade 코루틴에 선택한 duration 값을 매개변수로 넘김
        _co = StartCoroutine(Fade(duration));
    }

    IEnumerator Fade(float duration)
    {
        // (안전 장치) 만약 duration이 0이면 즉시 값을 적용하고 코루틴 종료
        if (duration <= 0f)
        {
            _current = _target;
            Apply(_current);
            _co = null; // 코루틴 참조 비우기
            yield break; // 코루틴 즉시 종료
        }

        float start = _current, t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            _current = Mathf.Lerp(start, _target, t / duration);
            Apply(_current);
            yield return null;
        }
        _current = _target; Apply(_current);
        _co = null; // 코루틴이 완료되었으므로 참조 비우기
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