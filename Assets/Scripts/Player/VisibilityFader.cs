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
    [Header("체력, 스킬 게이지 UI")]
    private GameObject _linkedUI;

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

    // EnemyInformation이 UI를 등록하기 위한 메서드 
    public void SetLinkedUI(GameObject uiRoot)
    {
        _linkedUI = uiRoot;
        if (_linkedUI != null)
        {
            bool isTargetVisible = (_target > 0f);
            _linkedUI.SetActive(isTargetVisible);
        }
    }

    public void SetVisible(bool v)
    {
        _target = v ? 1f : 0f;
        if (_co != null) StopCoroutine(_co);

        // '보이기'(v=true)일 경우 fadeInDuration을, '숨기기'(v=false)일 경우 fadeOutDuration을 선택
        float duration = v ? fadeInDuration : fadeOutDuration;

        // Fade 코루틴에 선택한 duration 값을 매개변수로 넘김
        _co = StartCoroutine(Fade(duration, v));
    }

    IEnumerator Fade(float duration, bool isFadingIn)
    {
        // (안전 장치) 만약 duration이 0이면 즉시 값을 적용하고 코루틴 종료
        if (duration <= 0f)
        {
            _current = _target;
            Apply(_current);
            _co = null; // 코루틴 참조 비우기
            // 위험 예방 상 UI 반영
            if (_linkedUI != null) _linkedUI.SetActive(_target > 0f);

            yield break; // 코루틴 즉시 종료
        }

        if (isFadingIn && _linkedUI != null)
        {
            _linkedUI.SetActive(true);
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

        // FADE OUT 완료 시 UI 비활성화
        if (_target == 0f && _linkedUI != null) _linkedUI.SetActive(false);

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