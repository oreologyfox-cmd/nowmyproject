using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;

public class SceneChangeManager : MonoBehaviour
{
    [SerializeField] private SceneFragChecker sceneFragChecker;
    [SerializeField] private GameObject novelControllerObject;
    [SerializeField] private NovelController novelController;

    // ★変更：クリック対象となるオブジェクトをインスペクターで指定します
    [Header("Click Target")]
    [Tooltip("テキスト終了後に、次のシーンへ遷移させるためにクリックするオブジェクト")]
    [SerializeField] private GameObject clickableTargetObject;

#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset nextSceneAsset;
    private void OnValidate() => nextSceneName = nextSceneAsset != null ? nextSceneAsset.name : "";
#endif

    [HideInInspector][SerializeField] private string nextSceneName;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;

    private bool isActivated, isSceneChanging;
    private Image fadeImage;
    private FieldInfo fieldIndex, fieldLines;

    void Start()
    {
        if (novelControllerObject) novelControllerObject.SetActive(false);
        if (!sceneFragChecker) sceneFragChecker = FindFirstObjectByType<SceneFragChecker>();
        if (!novelController && novelControllerObject) novelController = novelControllerObject.GetComponentInChildren<NovelController>(true);

        SetupFadeCanvas();

        // ★変更：対象オブジェクトにクリックイベントを動的に追加
        SetupClickTarget();
    }

    void Update()
    {
        if (isSceneChanging) return;

        // 1. アクティブ化の監視
        if (!isActivated && sceneFragChecker && sceneFragChecker.IsScene1 && novelControllerObject)
        {
            isActivated = true;
            novelControllerObject.SetActive(true);
            if (!novelController) novelController = novelControllerObject.GetComponentInChildren<NovelController>();

            if (novelController)
            {
                var t = novelController.GetType();
                fieldIndex = t.GetField("currentLineIndex", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldLines = t.GetField("lines", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            return;
        }
    }

    // ★追加：対象オブジェクトがクリックされたときに呼び出されるメソッド
    public void OnTargetClicked()
    {
        if (isSceneChanging || !isActivated || fieldIndex == null || fieldLines == null) return;

        int idx = (int)fieldIndex.GetValue(novelController);
        int count = ((System.Collections.IList)fieldLines.GetValue(novelController))?.Count ?? 0;

        // テキストをすべて読み終えている場合のみ、シーン遷移を開始する
        if (idx >= count)
        {
            StartCoroutine(TransitionSequence());
        }
    }

    // ★追加：対象オブジェクトにクリックを検知するコンポーネントを自動付与
    private void SetupClickTarget()
    {
        if (clickableTargetObject == null) return;

        // UI（Button）の場合の設定
        var button = clickableTargetObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnTargetClicked);
            return;
        }

        // 3Dオブジェクト（Colliderが必要）または通常のUIの場合
        var trigger = clickableTargetObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null) trigger = clickableTargetObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnTargetClicked(); });
        trigger.triggers.Add(entry);
    }

    private IEnumerator TransitionSequence()
    {
        if (string.IsNullOrEmpty(nextSceneName)) yield break;
        isSceneChanging = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;

        fadeImage.gameObject.SetActive(true);
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadeImage.color = Color.Lerp(new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0), fadeColor, t / fadeDuration);
            yield return null;
        }
        fadeImage.color = fadeColor;

        while (asyncLoad.progress < 0.9f) yield return null;
        asyncLoad.allowSceneActivation = true;
    }

    private void SetupFadeCanvas()
    {
        var canvasObj = new GameObject("FadeCanvas", typeof(Canvas), typeof(CanvasScaler));
        canvasObj.transform.SetParent(transform);
        canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.GetComponent<Canvas>().sortingOrder = 999;
        canvasObj.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        var imgObj = new GameObject("FadeImage", typeof(Image));
        imgObj.transform.SetParent(canvasObj.transform);
        fadeImage = imgObj.GetComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        fadeImage.rectTransform.anchoredPosition = Vector2.zero;
        fadeImage.rectTransform.sizeDelta = Vector2.zero;
        fadeImage.rectTransform.anchorMin = Vector2.zero;
        fadeImage.rectTransform.anchorMax = Vector2.one;
        imgObj.SetActive(false);
    }

    public void ResetActivator()
    {
        isActivated = isSceneChanging = false;
        if (novelControllerObject) novelControllerObject.SetActive(false);
        if (fadeImage)
        {
            fadeImage.gameObject.SetActive(false);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        }
    }
}
