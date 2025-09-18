using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    VisualElement root;
    VisualElement mainPanel;
    VisualElement optionPanel;

    Button startBtn, optionBtn, quitBtn, backBtn;

    void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        mainPanel = root.Q<VisualElement>("MainPanel");
        optionPanel = root.Q<VisualElement>("OptionPanel");

        startBtn = root.Q<Button>("StartButton");
        optionBtn = root.Q<Button>("OptionButton");
        quitBtn = root.Q<Button>("QuitButton");
        backBtn = root.Q<Button>("BackButton");

        startBtn.clicked += OnStart;
        optionBtn.clicked += ShowOptions;
        quitBtn.clicked += OnQuit;
        backBtn.clicked += ShowMain;

        ShowMain();
    }

    void OnDisable()
    {
        startBtn.clicked -= OnStart;
        optionBtn.clicked -= ShowOptions;
        quitBtn.clicked -= OnQuit;
        backBtn.clicked -= ShowMain;
    }

    void ShowOptions()
    {
        mainPanel.style.display = DisplayStyle.None;
        optionPanel.style.display = DisplayStyle.Flex;
    } 

    void ShowMain()
    {
        optionPanel.style.display = DisplayStyle.None;
        mainPanel.style.display = DisplayStyle.Flex;
    }

    void OnStart()
    {
        StartCoroutine(LoadGameFlow());
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에선 Play 중지
#else
        Application.Quit(); // 빌드에서 종료
#endif
    }

    IEnumerator LoadGameFlow()
    {
        // 게임 플레이 상주 씬 로드
        yield return SceneManager.LoadSceneAsync("GameplayPersistent", LoadSceneMode.Additive);

        // 레벨 씬 로드, 활성씬 설정
        yield return SceneManager.LoadSceneAsync("SimpleNaturePack_Demo", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("SimpleNaturePack_Demo"));

        //메뉴 언 로드
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
