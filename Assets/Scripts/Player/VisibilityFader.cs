using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class VisibilityFader : MonoBehaviour
{
    public Renderer[] renderers;

    [Header("페이드 속도 (초)")]
    public float fadeInDuration = 0.15f;
    [Tooltip("이 시간(초)만큼 시야에서 사라져도 '안 보임' 처리를 유예합니다.")]
    public float fadeOutDelay = 0.3f; // [✨ 1. 유예 시간 추가]
    public float fadeOutDuration = 1f;

    [Header("체력, 스킬 게이지 UI")]
    private GameObject _linkedUI;

    public string colorProp = "_BaseColor";
    float _target = 0f, _current = 0f;
    Coroutine _co; // 현재 실행 중인 페이드 또는 딜레이 코루틴
    MaterialPropertyBlock _mpb;

    // ... (Awake, SetLinkedUI 함수는 동일) ...
    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();
        Apply(_current);
    }

    public void SetLinkedUI(GameObject uiRoot)
    {
        _linkedUI = uiRoot;
        if (_linkedUI != null)
        {
            bool isTargetVisible = (_target > 0f);
            _linkedUI.SetActive(isTargetVisible);
        }
    }


    // ▼▼▼ [ 2. SetVisible 로직 수정 ] ▼▼▼
    public void SetVisible(bool v)
    {
        float newTarget = v ? 1f : 0f;

        // 1. 이미 원하는 상태(혹은 그 상태로 '변하는 중')이면 무시
        if (newTarget == _target)
        {
            return;
        }

        // 2. 새로운 목표 상태로 갱신
        _target = newTarget;

        // 3. 현재 진행 중인 모든 페이드/딜레이 코루틴 중지
        if (_co != null)
        {
            StopCoroutine(_co);
        }

        // 4. 상태에 따라 새 코루틴 시작
        if (_target == 1f) // "보이기" 명령
        {
            // 즉시 Fade-In 시작
            _co = StartCoroutine(Fade(fadeInDuration, true));
        }
        else // "숨기기" 명령
        {
            // '유예 시간'을 가진 Fade-Out 코루틴 시작
            _co = StartCoroutine(FadeOutWithDelay());
        }
    }

    // ▼▼▼ [ 3. 새로운 Fade-Out 딜레이 코루틴 ] ▼▼▼
    IEnumerator FadeOutWithDelay()
    {
        // 1. 'fadeOutDelay' (예: 0.5초) 만큼 기다림
        yield return new WaitForSeconds(fadeOutDelay);

        // (이 0.5초 안에 SetVisible(true)가 호출되면,
        //  StopCoroutine에 의해 이 코루틴은 '여기서' 중지됨)

        // 2. 0.5초가 지났는데도 _target이 여전히 0f(숨기기)라면,
        //    '진짜' Fade-Out 코루틴을 시작함.
        _co = StartCoroutine(Fade(fadeOutDuration, false));
    }

    IEnumerator Fade(float duration, bool isFadingIn)
    {
        // ... (duration 0 이하일 때 안전 장치 코드는 동일) ...
        if (duration <= 0f)
        {
            _current = _target;
            Apply(_current);
            _co = null;
            if (_linkedUI != null) _linkedUI.SetActive(_target > 0f);
            yield break;
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

        if (_target == 0f && _linkedUI != null) _linkedUI.SetActive(false);

        _co = null;
    }

    // ... (Apply 함수는 동일) ...
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