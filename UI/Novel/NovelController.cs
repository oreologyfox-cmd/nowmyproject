using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ExpressionType
{
    Bad1,     // 通常
    Bad2,     // 笑顔
    Normal1,  // 悲しい
    Normal2   // 怒り
}

public class NovelController : MonoBehaviour
{
    [System.Serializable]
    public class NovelLine
    {
        [TextArea(2, 5)]
        public string text;
        public float textSpeed = 0.05f;
        public ExpressionType expression;
    }

    [Header("References (Gages)")]
    [SerializeField] private SleepGageDown sleepGageDown;
    [SerializeField] private KoufunGageDown koufunGageDown;

    [Header("References (UI)")]
    [SerializeField] private TextMeshProUGUI textMeshPro;

    [Header("Fade Settings")]
    [Tooltip("画面を覆う暗転用のUI（ImageやCanvasGroupなど）")]
    [SerializeField] private Image fadeImage;
    [Tooltip("フェードアウトにかける時間（秒）")]
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Common UI Settings")] // --- [変更] 表示・非表示の対象を共通化
    [Tooltip("ノベル開始時に【表示】したいオブジェクト群")]
    [SerializeField] private List<GameObject> objectsToShow = new List<GameObject>();
    [Tooltip("ノベル開始時に【非表示】にしたいオブジェクト群")]
    [SerializeField] private List<GameObject> objectsToHide = new List<GameObject>();

    [Header("Bad End Settings")]
    [SerializeField] private GameObject badEndObject;
    [SerializeField] private List<NovelLine> badLines = new List<NovelLine>();
    [SerializeField] private string badEndNextSceneName;

    [Header("Continue Settings")]
    [SerializeField] private GameObject continueObject;
    [SerializeField] private List<NovelLine> continueLines = new List<NovelLine>();
    [SerializeField] private string continueNextSceneName;

    [Header("Expression Settings")]
    [SerializeField] private HyoujouKirikae hyoujouKirikae;

    private List<NovelLine> activeLines = null;
    private int currentLineIndex = 0;
    private Coroutine typeRoutine;
    private bool isTyping = false;
    private bool isNovelStarted = false;
    private bool isFading = false;
    private string nextSceneName = "";

    void Start()
    {
        if (badEndObject != null) badEndObject.SetActive(false);
        if (continueObject != null) continueObject.SetActive(false);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }

        if (!sleepGageDown) sleepGageDown = FindFirstObjectByType<SleepGageDown>();
        if (!koufunGageDown) koufunGageDown = FindFirstObjectByType<KoufunGageDown>();
    }

    void Update()
    {
        if (isFading) return;

        if (!isNovelStarted)
        {
            EvaluateGageConditionsAndStart();
        }

        if (isNovelStarted && Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            HandleInputProgress();
        }
    }

    private void EvaluateGageConditionsAndStart()
    {
        if (sleepGageDown == null || koufunGageDown == null)
        {
            Debug.LogError("[NovelController] 参照するゲージ（SleepGageDown / KoufunGageDown）が設定されていません。");
            return;
        }

        // --- [変更] 共通のオブジェクト表示・非表示をここで1回だけ実行 ---
        TriggerObjectsUI(objectsToShow, objectsToHide);

        // 条件判定
        bool isBadEndCondition = (sleepGageDown.time >= 0.99f) && (koufunGageDown.levelUp <= 1);

        if (isBadEndCondition)
        {
            nextSceneName = badEndNextSceneName;
            if (badEndObject != null) badEndObject.SetActive(true);
            StartNovel(badLines);
        }
        else
        {
            nextSceneName = continueNextSceneName;
            if (continueObject != null) continueObject.SetActive(true);
            StartNovel(continueLines);
        }
    }

    private void HandleInputProgress()
    {
        if (isTyping)
        {
            StopCoroutine(typeRoutine);
            textMeshPro.maxVisibleCharacters = textMeshPro.text.Length;
            isTyping = false;
        }
        else
        {
            currentLineIndex++;
            if (activeLines != null && currentLineIndex < activeLines.Count)
            {
                CheckAndTriggerExpression(activeLines[currentLineIndex]);
                typeRoutine = StartCoroutine(TypeText(activeLines[currentLineIndex]));
            }
            else
            {
                StartCoroutine(FadeOutAndLoadScene());
            }
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        isFading = true;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0f;
            Color originalColor = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                originalColor.a = alpha;
                fadeImage.color = originalColor;
                yield return null;
            }
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[NovelController] 次のシーン名が空です。");
            textMeshPro.text = "【おわり】";
            isFading = false;
        }
    }

    private void StartNovel(List<NovelLine> targetLines)
    {
        activeLines = targetLines;
        isNovelStarted = true;
        currentLineIndex = 0;

        if (activeLines != null && activeLines.Count > 0)
        {
            CheckAndTriggerExpression(activeLines[currentLineIndex]);
            typeRoutine = StartCoroutine(TypeText(activeLines[currentLineIndex]));
        }
    }

    private void TriggerObjectsUI(List<GameObject> toShow, List<GameObject> toHide)
    {
        foreach (GameObject obj in toShow) if (obj != null) obj.SetActive(true);
        foreach (GameObject obj in toHide) if (obj != null) obj.SetActive(false);
    }

    private void CheckAndTriggerExpression(NovelLine lineData)
    {
        if (hyoujouKirikae != null)
        {
            hyoujouKirikae.ChangeExpression(lineData.expression);
        }
    }

    private IEnumerator TypeText(NovelLine lineData)
    {
        isTyping = true;
        textMeshPro.text = lineData.text;
        textMeshPro.maxVisibleCharacters = 0;

        for (int i = 0; i <= lineData.text.Length; i++)
        {
            textMeshPro.maxVisibleCharacters = i;
            yield return new WaitForSeconds(lineData.textSpeed);
        }

        isTyping = false;
    }
}
