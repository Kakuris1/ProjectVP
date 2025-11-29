using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuPanel; // 기존 UI 패널

    [SerializeField]
    private InputReader inputReader; // ★ InputReader를 인스펙터에서 연결!

    private bool isPaused = false;

    void Update()
    {
        // 'ESC' 키 감지 부분을 InputReader로 교체합니다.
        // if (Input.GetKeyDown(KeyCode.Escape)) // <- 이 줄 삭제

        if (inputReader.PausePressed) // <- 이 줄 추가
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- 일시정지 로직 ---
    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
    }

    // --- UI 버튼과 연결할 함수들 (전혀 수정할 필요 없음) ---

    // '게임으로 돌아가기' 버튼용
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }

    // '메인메뉴로 나가기' 버튼용
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Boot");
    }

    // 게임 종료 버튼
    public void QuitGame()
    {
        Debug.Log("게임 종료!");
        // 빌드된 게임에서만 실제로 종료
        // 에디터에서는 Play 모드가 중지
        Application.Quit();

        //에디터용 코드 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // '설정' 톱니바퀴 버튼용
    public void OpenPauseMenu()
    {
        if (!isPaused)
        {
            PauseGame();
        }
    }
}