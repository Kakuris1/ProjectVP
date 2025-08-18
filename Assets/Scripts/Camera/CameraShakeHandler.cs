using UnityEngine;

public class CameraShakeHandler : MonoBehaviour
{
    private Vector3 originalPos;
    private float shakeDuration = 0f;
    private float shakeStrength = 0.1f;

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    public void TriggerShake(float duration, float strength)
    {
        shakeDuration = duration;
        shakeStrength = strength;
        StopAllCoroutines(); // 중복 흔들림 방지
        StartCoroutine(ShakeCoroutine());
    }

    private System.Collections.IEnumerator ShakeCoroutine()
    {
        while (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeStrength;
            shakeDuration -= Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}