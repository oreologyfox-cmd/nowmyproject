using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class EffectHyoujouManager : MonoBehaviour
{
    [System.Serializable]
    public class ExpressionLevelGroup
    {
        [Tooltip("レベル（0〜3）")]
        public int level;

        [Header("Press Settings")]
        [Tooltip("クリック・ドラッグされている時の表情切り替え間隔（秒数）")]
        public float changeInterval = 3.0f;
        [Tooltip("クリック・ドラッグされている時にランダム表示させたいラベル名")]
        public List<string> spriteLabels = new List<string>();

        [Header("Idle Settings")]
        [Tooltip("クリック・ドラッグされていない時の表情切り替え間隔（秒数）")]
        public float idleChangeInterval = 5.0f;
        [Tooltip("クリック・ドラッグされていない時にランダム表示させたいラベル名")]
        public List<string> idleSpriteLabels = new List<string>();
    }

    [Header("References")]
    [Tooltip("ゲージ管理スクリプト（KoufunGageDown）")]
    public KoufunGageDown gageScript;

    [Tooltip("表情を変化させたいオブジェクトの SpriteResolver")]
    public SpriteResolver targetSpriteResolver;

    [Tooltip("条件判定スクリプト（SleepWakeChecker）")]
    [SerializeField] private SleepWakeChecker conditionChecker;

    [Header("Sprite Library Settings")]
    [Tooltip("使用する Sprite Library Asset")]
    public SpriteLibraryAsset libraryAsset;

    [Tooltip("Sprite Library Asset 内のカテゴリー名")]
    public string categoryName = "Face";

    [Header("Expression Groups")]
    public List<ExpressionLevelGroup> expressionGroups = new List<ExpressionLevelGroup>();

    [Header("Fade Settings")]
    [Tooltip("フェードアウトにかかる時間（秒）")]
    public float fadeDuration = 0.5f;

    private Dictionary<int, ExpressionLevelGroup> levelGroupMap = new Dictionary<int, ExpressionLevelGroup>();
    private int lastLevel = -1;
    private float pressTimer = 0f;
    private float idleTimer = 0f;

    private bool wasPressing = false;
    private SpriteRenderer targetSpriteRenderer;

    void Start()
    {
        foreach (var group in expressionGroups)
        {
            if (!levelGroupMap.ContainsKey(group.level))
            {
                levelGroupMap[group.level] = group;
            }
        }

        if (conditionChecker == null)
        {
            conditionChecker = FindFirstObjectByType<SleepWakeChecker>();
        }

        if (targetSpriteResolver != null)
        {
            targetSpriteRenderer = targetSpriteResolver.GetComponent<SpriteRenderer>();
        }

        int initialLevel = gageScript != null ? gageScript.levelUp : 0;
        UpdateExpression(false, initialLevel);
    }

    void Update()
    {
        if (conditionChecker != null && conditionChecker.IsConditionMet)
        {
            pressTimer = 0f;
            idleTimer = 0f;
            return;
        }

        if (gageScript == null) return;

        bool isPressing = false;
        if (Pointer.current != null)
        {
            isPressing = Pointer.current.press.isPressed;
        }

        int currentLevel = gageScript.levelUp;

        if (currentLevel != lastLevel || isPressing != wasPressing)
        {
            UpdateExpression(isPressing, currentLevel);
            pressTimer = 0f;
            idleTimer = 0f;

            lastLevel = currentLevel;
            wasPressing = isPressing;
            return;
        }

        if (levelGroupMap.TryGetValue(currentLevel, out ExpressionLevelGroup group))
        {
            if (isPressing)
            {
                idleTimer = 0f;
                pressTimer += Time.unscaledDeltaTime;

                if (pressTimer >= group.changeInterval)
                {
                    UpdateExpression(true, currentLevel);
                    pressTimer = 0f;
                }
            }
            else
            {
                pressTimer = 0f;
                idleTimer += Time.unscaledDeltaTime;

                if (idleTimer >= group.idleChangeInterval)
                {
                    UpdateExpression(false, currentLevel);
                    idleTimer = 0f;
                }
            }
        }
        else
        {
            pressTimer = 0f;
            idleTimer = 0f;
        }

        wasPressing = isPressing;
    }

    public void UpdateExpression(bool isPressing, int currentLevel)
    {
        if (targetSpriteResolver == null || targetSpriteRenderer == null) return;

        if (!levelGroupMap.TryGetValue(currentLevel, out ExpressionLevelGroup group))
        {
            Debug.LogWarning($"レベル {currentLevel} に対応する表情グループが設定されていません。");
            return;
        }

        string currentLabel = targetSpriteResolver.GetLabel();
        string selectedLabel = "";

        if (isPressing)
        {
            if (group.spriteLabels.Count > 0)
            {
                selectedLabel = GetRandomDifferentLabel(group.spriteLabels, currentLabel);
            }
            else
            {
                Debug.LogWarning($"レベル {currentLevel} の操作時(Press)の表情ラベルが設定されていません。");
                return;
            }
        }
        else
        {
            if (group.idleSpriteLabels.Count > 0)
            {
                selectedLabel = GetRandomDifferentLabel(group.idleSpriteLabels, currentLabel);
            }
            else
            {
                Debug.LogWarning($"レベル {currentLevel} の非操作時(Idle)の表情ラベルが設定されていません。");
                return;
            }
        }

        if (!string.IsNullOrEmpty(selectedLabel))
        {
            // 旧スプライトが存在し、非表示でなければ、手前にフェード用ゴーストを生成
            if (targetSpriteRenderer.sprite != null && targetSpriteRenderer.gameObject.activeInHierarchy)
            {
                CreateFadeOutGhost(targetSpriteRenderer.sprite);
            }

            // 新しい表情に即座に切り替え（ゴーストが手前にあるので、切り替わった瞬間はゴーストに隠れます）
            targetSpriteResolver.SetCategoryAndLabel(categoryName, selectedLabel);
        }
    }

    private void CreateFadeOutGhost(Sprite oldSprite)
    {
        GameObject ghostObj = new GameObject("Expression_FadeGhost");

        ghostObj.transform.SetParent(targetSpriteRenderer.transform.parent);
        ghostObj.transform.localPosition = targetSpriteRenderer.transform.localPosition;
        ghostObj.transform.localRotation = targetSpriteRenderer.transform.localRotation;
        ghostObj.transform.localScale = targetSpriteRenderer.transform.localScale;

        SpriteRenderer ghostRenderer = ghostObj.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = oldSprite;
        ghostRenderer.color = targetSpriteRenderer.color;
        ghostRenderer.material = targetSpriteRenderer.material;

        ghostRenderer.sortingLayerID = targetSpriteRenderer.sortingLayerID;

        // 【修正点】元の顔より「手前」に出すことで、古い顔が前面でフェードアウトしていくように変更
        ghostRenderer.sortingOrder = targetSpriteRenderer.sortingOrder + 1;

        FadeOutBehaviour fadeBehaviour = ghostObj.AddComponent<FadeOutBehaviour>();
        fadeBehaviour.StartFade(fadeDuration);
    }

    private string GetRandomDifferentLabel(List<string> labels, string currentLabel)
    {
        if (labels.Count <= 1) return labels.Count == 1 ? labels[0] : "";

        List<string> candidates = labels.FindAll(label => label != currentLabel);
        if (candidates.Count == 0) candidates = labels;

        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }
}

public class FadeOutBehaviour : MonoBehaviour
{
    private SpriteRenderer sr;
    private float duration;
    private float elapsed = 0f;
    private Color startColor;
    private bool isInitialized = false;

    public void StartFade(float fadeDuration)
    {
        sr = GetComponent<SpriteRenderer>();
        duration = fadeDuration;
        if (sr != null)
        {
            startColor = sr.color;
            isInitialized = true;
        }
    }

    void Update()
    {
        if (!isInitialized || sr == null) return;

        elapsed += Time.unscaledDeltaTime;
        float progress = duration > 0f ? elapsed / duration : 1f;

        // アルファ値だけを1から0へ変化させる
        float alpha = Mathf.Lerp(startColor.a, 0f, progress);
        sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
