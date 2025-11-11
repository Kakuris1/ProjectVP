using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("Mixer Settings")]
    public AudioMixerGroup sfxMixerGroup;
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    [Header("BGM Playlist")]
    public AudioClip[] bgmPlaylist; // 인스펙터에서 BGM 파일을 연결
    private int currentBgmIndex = 0;
    private Coroutine bgmCoroutine; // 재생 중인 코루틴을 저장할 변수
    [Header("SFX Pool")]
    public int sfxPoolSize = 10; // 인스펙터에서 10개로 설정
    private List<AudioSource> sfxSources;

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ▼ SFX 오디오 소스 풀(Pool) 생성 ▼
        sfxSources = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            // 모든 SFX 소스는 'SFX' 믹서 그룹으로 출력
            source.outputAudioMixerGroup = sfxMixerGroup; 
            sfxSources.Add(source);
        }
    }

    void OnEnable()
    {
        EventManager.Instance.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnGameOver -= HandleGameOver;
        }
    }

    void Start()
    {
        // BGM 소스가 있고, 재생할 목록이 있다면 플레이리스트 재생 시작
        if (bgmSource != null && bgmPlaylist != null && bgmPlaylist.Length > 0)
        {
            // 코루틴 시작
            bgmCoroutine = StartCoroutine(PlayBgmPlaylist());
        }
    }

    private IEnumerator PlayBgmPlaylist()
    {
        // 이 코루틴은 게임오버로 중지되기 전까지 무한 반복
        while (true)
        {
            // 1. 현재 인덱스의 BGM 클립을 bgmSource에 할당
            bgmSource.clip = bgmPlaylist[currentBgmIndex];

            // 2. BGM 재생
            bgmSource.Play();

            // 3. (중요) 현재 BGM이 끝날 때까지 기다림
            //    (bgmSource.clip.length 만큼 기다리거나, isPlaying으로 체크)
            yield return new WaitUntil(() => !bgmSource.isPlaying && bgmSource.enabled);

            // 4. 다음 곡으로 인덱스 이동
            currentBgmIndex++;

            // 5. 마지막 곡까지 재생했으면 다시 처음(0번)으로
            if (currentBgmIndex >= bgmPlaylist.Length)
            {
                currentBgmIndex = 0;
            }

            // (무한 루프 방지를 위해 1프레임 대기)
            yield return null;
        }
    }

    // ▼ "놀고 있는" 오디오 소스 찾기 ▼
    private AudioSource GetAvailableSFXSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        // 모든 소스가 재생 중이면 그냥 첫 번째 것 반환 (오래된 소리 끊김)
        return sfxSources[0];
    }

    // 스킬 스크립트에서 이 함수를 호출합니다.
    public void PlaySFX(string soundName, float volumeScale = 1.0f)
    {
        // 1. Resources 폴더에서 'soundName'으로 오디오 클립 로드
        // (폴더 경로 "SFX/"를 포함해야 함)
        AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/" + soundName);

        if (clip == null)
        {
            Debug.LogWarning("SFX  Not Found: " + soundName);
            return;
        }

        // 2. 놀고 있는 AudioSource 찾기
        AudioSource sourceToPlay = GetAvailableSFXSource();

        // 3. PlayOneShot으로 볼륨 조절해서 재생
        sourceToPlay.PlayOneShot(clip, volumeScale);
    }

    private void HandleGameOver()
    {
        // 1. BGM 중지
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }

        // 2. 게임 오버 SFX 재생
        PlaySFX("GameOverSound", 0.4f);
    }
}