using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("エディタ用：遷移先シーンの設定")]
    [SerializeField] private UnityEditor.SceneAsset normalSceneAsset;
    [SerializeField] private UnityEditor.SceneAsset scene2SceneAsset;
    [SerializeField] private UnityEditor.SceneAsset scene3SceneAsset;
    [SerializeField] private UnityEditor.SceneAsset scene4SceneAsset;
    [SerializeField] private UnityEditor.SceneAsset scene5SceneAsset; // ←追加：Normal5用

    private void OnValidate()
    {
        if (normalSceneAsset != null) { normalSceneName = normalSceneAsset.name; }
        if (scene2SceneAsset != null) { scene2SceneName = scene2SceneAsset.name; }
        if (scene3SceneAsset != null) { scene3SceneName = scene3SceneAsset.name; }
        if (scene4SceneAsset != null) { scene4SceneName = scene4SceneAsset.name; }
        if (scene5SceneAsset != null) { scene5SceneName = scene5SceneAsset.name; } // ←追加：Normal5用
    }
#endif

    [HideInInspector][SerializeField] private string normalSceneName;
    [HideInInspector][SerializeField] private string scene2SceneName;
    [HideInInspector][SerializeField] private string scene3SceneName;
    [HideInInspector][SerializeField] private string scene4SceneName;
    [HideInInspector][SerializeField] private string scene5SceneName; // ←追加：Normal5用

    [Header("フェード用のUI設定")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("判定用のフラグチェッカー参照")]
    [SerializeField] private SceneFragChecker fragChecker;

    private bool isFading = false;

    public void ChangeScene()
    {
        if (isFading || fragChecker == null) return;

        string targetScene = "";

        // 各フラグの状態をチェックして遷移先を決定
        if (fragChecker.IsNormal)
        {
            targetScene = normalSceneName;
        }
        else if (fragChecker.IsNormal2)
        {
            targetScene = scene2SceneName;
        }
        else if (fragChecker.IsNormal3)
        {
            targetScene = scene3SceneName;
        }
        else if (fragChecker.IsNormal4)
        {
            targetScene = scene4SceneName;
        }
        else if (fragChecker.IsNormal5) // ←追加：Normal5がtrueのときの処理
        {
            targetScene = scene5SceneName;
        }
        else
        {
            Debug.Log("対応するフラグ（normal～Normal5）がどれも true ではないため、遷移しません。");
            return;
        }

        if (!string.IsNullOrEmpty(targetScene))
        {
            StartCoroutine(FadeOutAndLoadScene(targetScene));
        }
        else
        {
            Debug.LogWarning("対応する遷移先シーンが設定されていません。");
        }
    }

    private IEnumerator FadeOutAndLoadScene(string nextSceneName)
    {
        isFading = true;

        if (fadeImage != null)
        {
            fadeImage.raycastTarget = true;
            float timer = 0f;
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Clamp01(timer / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("フェード用のImageが割り当てられていません。フェードなしで移動します。");
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
