using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 재시작을 위해

public class GameOverUIController : MonoBehaviour
{
    [Header("Game Over Panel")]
    public GameObject gameOverPanel; // 1. Inspector에서 'GameOverPanel'을 연결
    public float fadeDuration = 1.5f; // 페이드 인에 걸리는 시간

    private CanvasGroup gameOverCanvasGroup;

    void Awake()
    {
        // CanvasGroup 컴포넌트를 미리 찾아둡니다.
        if (gameOverPanel != null)
        {
            gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            gameOverPanel.SetActive(false); // 확실하게 비활성화로 시작
        }
    }

    // 2. 이벤트 구독
    void OnEnable()
    {
        EventManager.Instance.OnGameOver += ShowGameOverScreen;
    }

    // 3. 이벤트 구독 해제 (메모리 누수 방지)
    void OnDisable()
    {
        EventManager.Instance.OnGameOver -= ShowGameOverScreen;
    }

    // 4. 이벤트가 호출되면 실행될 함수
    private void ShowGameOverScreen()
    {
        StartCoroutine(FadeInGameOver());
    }

    // 5. 페이드 인 코루틴
    private IEnumerator FadeInGameOver()
    {
        // 패널을 활성화 (아직 Alpha=0이라 보이지 않음)
        gameOverPanel.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // CanvasGroup의 Alpha를 0에서 1로 서서히 변경
            gameOverCanvasGroup.alpha = timer / fadeDuration;
            yield return null;
        }
        gameOverCanvasGroup.alpha = 1; // 확실하게 1로 설정

        // 페이드 인이 끝나면 버튼 클릭이 가능하도록 설정
        gameOverCanvasGroup.interactable = true;
        gameOverCanvasGroup.blocksRaycasts = true;
    }

    // 6. 되돌리기 버튼이 호출할 함수
    public void RestartGame()
    {
        // (일시정지를 풀고 현재 씬을 다시 로드)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}