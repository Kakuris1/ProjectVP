using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
        SceneManager.LoadScene("SimpleNaturePack_Demo");
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에선 Play 중지
#else
        Application.Quit(); // 빌드에서 종료
#endif
    }
}
