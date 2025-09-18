using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] string corePersistent = "CorePersistent";
    [SerializeField] string mainMenu = "MainMenu";
    IEnumerator Start()
    {
        // ���� �ʱ� ���� (ǰ��, ���, ���̺� �ε� ��)

        // �ھ� ���� �� �ε�
        yield return SceneManager.LoadSceneAsync(corePersistent, LoadSceneMode.Additive);

        // �޴� �ε�
        yield return SceneManager.LoadSceneAsync(mainMenu, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainMenu));

        // Boot ��ε�
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
