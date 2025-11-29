using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneFadeController : MonoBehaviour
{
    public Image uguiFadePanel;

    public float fadeDuration = 1.0f; // 페이드 인에 걸리는 시간 (초)

    void Start()
    {
        // 스크립트가 활성화될 때 (씬 로드 시) 페이드 인 시작
        if (uguiFadePanel != null)
        {
            uguiFadePanel.color = Color.black;
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = 1 - (timer / fadeDuration); // 1 (불투명) -> 0 (투명)

            Color currentColor = uguiFadePanel.color;
            currentColor.a = progress;
            uguiFadePanel.color = currentColor;

            yield return null;
        }

        Color finalColor = uguiFadePanel.color;
        finalColor.a = 0;
        uguiFadePanel.color = finalColor; // 확실히 투명하게
        uguiFadePanel.gameObject.SetActive(false); // 패널 비활성화 (클릭 막는 기능도 제거)
    }
}