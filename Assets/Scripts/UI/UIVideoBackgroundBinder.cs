using UnityEngine;
using UnityEngine.UIElements; // Background, BackgroundSizeType, DisplayStyle 등을 위해 필수!
using UnityEngine.Video;

public class UIVideoBackgroundBinder : MonoBehaviour
{
    public RenderTexture videoTexture;
    public VideoPlayer videoPlayer;

    private VisualElement videoContainer;
    private VisualElement videoPlaceholder;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        videoContainer = root.Q("background-video-container");
        videoPlaceholder = root.Q("video-placeholder");

        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();

        if (videoContainer != null && videoTexture != null)
        {
            // --- 1번 오류 수정 (두 번째) ---
            // 'newBackground.texture =' 대신
            // Background.FromRenderTexture() 헬퍼 함수 사용
            videoContainer.style.backgroundImage =
                Background.FromRenderTexture(videoTexture);
            // --- 1번 수정 끝 ---

            // --- 2번 오류 수정 (두 번째) ---
            // 'ScaleMode.Cover' 대신
            // new BackgroundSize(BackgroundSizeType.Cover) 사용
            videoContainer.style.backgroundSize =
                new StyleBackgroundSize(new BackgroundSize(BackgroundSizeType.Cover));
            // --- 2번 수정 끝 ---

            // (이 부분은 이전에 수정한 내용)
            videoPlaceholder.style.display = DisplayStyle.Flex;
            videoContainer.style.display = DisplayStyle.None;

            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
    }

    void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        // (이 부분은 이전에 수정한 내용)
        videoPlaceholder.style.display = DisplayStyle.None;
        videoContainer.style.display = DisplayStyle.Flex;

        source.Play();
    }
}