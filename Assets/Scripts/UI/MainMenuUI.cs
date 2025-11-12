using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    VisualElement root;
    VisualElement mainPanel;
    VisualElement optionPanel;
    VisualElement fadePanel;
    public float fadeDuration = 1.0f; // 페이드 아웃/인에 걸리는 시간 (초)

    Button startBtn, optionBtn, quitBtn, backBtn;

    [Header("Audio")]
    public AudioMixer mainMixer; // 오디오 믹서
    public AudioClip StartClickSound; // 스타트 버튼 사운드
    public AudioClip ButtonClickSound; // 나머지 버튼 사운드

    private AudioSource bgmAudioSource; // BGM 전용
    private AudioSource sfxAudioSource; // SFX (클릭음) 전용

    // 사운드 설정 슬라이더
    private Slider bgmSlider;
    private Slider sfxSlider;

    void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;
        // AudioSource 가져오기
        // ▼ AudioSource가 2개이므로, 배열로 모두 가져옴
        AudioSource[] audioSources = GetComponents<AudioSource>();

        // BGM 클립이 할당된 것을 BGM 소스로, 아닌 것을 SFX 소스로 인식
        bgmAudioSource = audioSources[0].clip != null ? audioSources[0] : audioSources[1];
        sfxAudioSource = audioSources[0].clip == null ? audioSources[0] : audioSources[1];

        mainPanel = root.Q<VisualElement>("MainPanel");
        optionPanel = root.Q<VisualElement>("OptionPanel");
        fadePanel = root.Q<VisualElement>("FadePanel");

        startBtn = root.Q<Button>("StartButton");
        optionBtn = root.Q<Button>("OptionButton");
        quitBtn = root.Q<Button>("QuitButton");
        backBtn = root.Q<Button>("BackButton");

        bgmSlider = root.Q<Slider>("BGMController");
        sfxSlider = root.Q<Slider>("EffectController");

        if (bgmSlider != null && mainMixer != null)
        {
            // 3. 슬라이더 값 범위 설정 (0은 Log10 오류를 일으키므로 0.0001 사용)
            bgmSlider.lowValue = 0.0001f;
            bgmSlider.highValue = 1f;

            // 4. 현재 믹서 볼륨 값(dB)을 가져와 슬라이더 값(0~1)으로 변환
            mainMixer.GetFloat("BGM_Volume", out float bgmVol);
            bgmSlider.value = Mathf.Pow(10, bgmVol / 20);

            // 5. 슬라이더 값이 변경될 때마다 OnBGMVolumeChanged 함수 호출
            bgmSlider.RegisterValueChangedCallback(OnBGMVolumeChanged);
        }

        if (sfxSlider != null && mainMixer != null)
        {
            sfxSlider.lowValue = 0.0001f;
            sfxSlider.highValue = 1f;

            mainMixer.GetFloat("SFX_Volume", out float sfxVol);
            sfxSlider.value = Mathf.Pow(10, sfxVol / 20);

            sfxSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);
        }

        startBtn.clicked += OnStart;
        optionBtn.clicked += ShowOptions;
        quitBtn.clicked += OnQuit;
        backBtn.clicked += ShowMain;

        startBtn.clicked += PlayStartClickSound;
        optionBtn.clicked += PlayClickSound;
        quitBtn.clicked += PlayClickSound;
        backBtn.clicked += PlayClickSound;

        ShowMain();
    }

    void OnDisable()
    {
        startBtn.clicked -= OnStart;
        optionBtn.clicked -= ShowOptions;
        quitBtn.clicked -= OnQuit;
        backBtn.clicked -= ShowMain;
        startBtn.clicked -= PlayStartClickSound;
        optionBtn.clicked -= PlayClickSound;
        quitBtn.clicked -= PlayClickSound;
        backBtn.clicked -= PlayClickSound;
        if (bgmSlider != null)
        {
            bgmSlider.UnregisterValueChangedCallback(OnBGMVolumeChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.UnregisterValueChangedCallback(OnSFXVolumeChanged);
        }
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
        StartCoroutine(StartGameWithFade());
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에선 Play 중지
#else
        Application.Quit(); // 빌드에서 종료
#endif
    }
    void PlayStartClickSound()
    {
        if (StartClickSound != null)
        {
            sfxAudioSource.PlayOneShot(StartClickSound, 0.2f); // 소리가 커서 0.2로 줄임
        }
    }
    void PlayClickSound()
    {
        if (ButtonClickSound != null)
        {
            sfxAudioSource.PlayOneShot(ButtonClickSound);
        }
    }

    // BGM 슬라이더가 움직일 때 호출될 함수
    private void OnBGMVolumeChanged(ChangeEvent<float> evt)
    {
        // 7. 슬라이더 값(0~1)을 믹서의 데시벨(-80~0) 값으로 변환
        mainMixer.SetFloat("BGM_Volume", Mathf.Log10(evt.newValue) * 20);
    }

    // SFX 슬라이더가 움직일 때 호출될 함수
    private void OnSFXVolumeChanged(ChangeEvent<float> evt)
    {
        mainMixer.SetFloat("SFX_Volume", Mathf.Log10(evt.newValue) * 20);
    }

    IEnumerator StartGameWithFade()
    {
        // 1. 페이드 아웃 시작 (화면이 점점 검게 변함)
        yield return StartCoroutine(FadeOut());

        // 2. 페이드 아웃이 완료된 후, 게임 씬 로드 로직 시작
        yield return StartCoroutine(LoadGameFlow());
    }

    IEnumerator FadeOut()
    {
        if (fadePanel == null) yield break;

        // 페이드 패널이 모든 이벤트를 막도록 설정
        fadePanel.BringToFront();
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            fadePanel.style.opacity = progress; // 0 (투명) -> 1 (불투명)
            yield return null;
        }
        fadePanel.style.opacity = 1; // 확실히 불투명하게
    }

    IEnumerator LoadGameFlow()
    {
        // 게임 플레이 상주 씬 로드
        yield return SceneManager.LoadSceneAsync("GameplayPersistent", LoadSceneMode.Additive);

        // 레벨 씬 로드, 활성씬 설정
        yield return SceneManager.LoadSceneAsync("MainLevelScene", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainLevelScene"));

        //메뉴 언 로드
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
