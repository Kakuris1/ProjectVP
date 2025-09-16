using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] string corePersistent = "CorePersistent";
    [SerializeField] string mainMenu = "MainMenu";
    IEnumerator Start()
    {
        // 전역 초기 설정 (품질, 언어, 세이브 로드 등)

        // 코어 상주 씬 로드
        yield return SceneManager.LoadSceneAsync(corePersistent, LoadSceneMode.Additive);

        // 메뉴 로드
        yield return SceneManager.LoadSceneAsync(mainMenu, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainMenu));

        // Boot 언로드
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
