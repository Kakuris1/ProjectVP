using System.Collections;
using UnityEngine;

public class BuildingClipper : MonoBehaviour
{
    // [수정] 2개의 머티리얼 에셋을 인스펙터에서 직접 연결
    public Material opaqueMaterial; // ZWrite=On (기존 머티리얼)
    public Material transparentMaterial; // ZWrite=Off (새 머티리얼)

    public float cutoffHeight = 10f;
    public float roofHeight = 100f;
    public float fadeDuration = 1.0f;

    private string heightPropertyName = "_CutoffHeight";
    private string alphaPropertyName = "_FadeAlpha";

    private Renderer buildingRenderer; // [수정] 인스턴스 대신 렌더러만 저장
    private Coroutine currentFadeCoroutine = null;

    // [삭제] ZWritePropertyID 관련 코드 모두 삭제

    void Start()
    {
        buildingRenderer = GetComponent<Renderer>();
        if (buildingRenderer != null)
        {
            // 1. 런타임 복사본을 만들기 위해 '불투명' 머티리얼로 시작
            buildingRenderer.material = opaqueMaterial;

            // 2. 현재 머티리얼(Opaque 인스턴스)을 초기화
            buildingRenderer.material.SetFloat(heightPropertyName, roofHeight);
            buildingRenderer.material.SetFloat(alphaPropertyName, 1.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && buildingRenderer != null)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            // 1. [수정] 머티리얼을 ZWrite=Off 버전으로 *교체*
            buildingRenderer.material = transparentMaterial;

            // 2. 새 머티리얼 인스턴스의 높이를 즉시 설정
            buildingRenderer.material.SetFloat(heightPropertyName, cutoffHeight);

            // 3. 알파 값 페이드 아웃 시작
            currentFadeCoroutine = StartCoroutine(FadeAlpha(0.0f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && buildingRenderer != null)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            // 1. [수정] 머티리얼을 ZWrite=On 버전으로 *교체*
            buildingRenderer.material = opaqueMaterial;

            // 2. 새 머티리얼 인스턴스의 알파를 즉시 1로 (혹은 페이드 인 시작 값)
            // (페이드 인 코루틴이 덮어쓸 것임)

            // 3. 알파 값 페이드 인 시작
            currentFadeCoroutine = StartCoroutine(FadeAlpha(1.0f, true));
        }
    }

    private IEnumerator FadeAlpha(float targetAlpha, bool restoreHeightOnComplete = false)
    {
        // [수정] 현재 렌더러에 할당된 머티리얼 인스턴스를 가져옴
        Material currentMatInstance = buildingRenderer.material;

        float elapsedTime = 0f;
        float startAlpha = currentMatInstance.GetFloat(alphaPropertyName);

        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            currentMatInstance.SetFloat(alphaPropertyName, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentMatInstance.SetFloat(alphaPropertyName, targetAlpha);

        if (restoreHeightOnComplete)
        {
            // 페이드 인이 끝난 후 높이를 복구
            currentMatInstance.SetFloat(heightPropertyName, roofHeight);
        }

        currentFadeCoroutine = null;
    }
}