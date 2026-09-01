using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class WakeHyoujouManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("ゲージ管理スクリプト（KoufunGageDown）")]
    public KoufunGageDown gageScript;

    [Tooltip("条件判定スクリプト（SleepWakeChecker）")]
    [SerializeField] private SleepWakeChecker conditionChecker;

    [Header("Sprite Library Settings")]
    [Tooltip("使用する Sprite Library Asset（エディタのドロップダウン抽出用）")]
    public SpriteLibraryAsset libraryAsset;

    private Dictionary<int, WakeLevelSettiing> levelGroupMap = new Dictionary<int, WakeLevelSettiing>();
    private int lastLevel = -1;
    private bool wasPressing = false;

    void Start()
    {
        // 自身にアタッチされている WakeLevelSettiing をすべて取得
        WakeLevelSettiing[] settings = GetComponents<WakeLevelSettiing>();

        foreach (var setting in settings)
        {
            if (!levelGroupMap.ContainsKey(setting.targetLevel))
            {
                levelGroupMap[setting.targetLevel] = setting;
            }
            else
            {
                Debug.LogWarning($"重複エラー: レベル {setting.targetLevel} の WakeLevelSettiing が複数アタッチされています。");
            }
        }

        if (conditionChecker == null)
        {
            conditionChecker = FindFirstObjectByType<SleepWakeChecker>();
        }

        // 初期化時に初期値を同期
        if (gageScript != null)
        {
            lastLevel = gageScript.levelUp;
        }
        if (Pointer.current != null)
        {
            wasPressing = Pointer.current.press.isPressed;
        }

        UpdateAllPartsExpression(wasPressing);
    }

    void Update()
    {
        // 1. 入力とレベルの最新状態を取得
        bool isPressing = Pointer.current != null && Pointer.current.press.isPressed;
        int currentLevel = gageScript != null ? gageScript.levelUp : 0;

        // 2. 起床状態の条件（IsConditionMet）を満たしていない場合はタイマーをリセットして終了
        // ※SleepHyoujouManagerとは「!（否定）」の有無で条件を反転させています
        if (conditionChecker != null && !conditionChecker.IsConditionMet)
        {
            ResetAllTimers();
            lastLevel = currentLevel;
            wasPressing = isPressing;
            return;
        }

        if (gageScript == null) return;

        // 3. レベルや入力状態が変わった瞬間に即時更新
        if (currentLevel != lastLevel || isPressing != wasPressing)
        {
            UpdateAllPartsExpression(isPressing);
            ResetAllTimers();

            lastLevel = currentLevel;
            wasPressing = isPressing;
            return;
        }

        // 4. 状態変化がない場合の定時タイマー処理
        if (levelGroupMap.TryGetValue(currentLevel, out WakeLevelSettiing currentSetting))
        {
            foreach (var part in currentSetting.partsConfigs)
            {
                if (part.targetSpriteResolver == null) continue;

                if (isPressing)
                {
                    part.idleTimer = 0f;
                    part.pressTimer += Time.unscaledDeltaTime;

                    if (part.pressTimer >= part.changeInterval)
                    {
                        UpdateSinglePartExpression(part, true);
                        part.pressTimer = 0f;
                    }
                }
                else
                {
                    part.pressTimer = 0f;
                    part.idleTimer += Time.unscaledDeltaTime;

                    if (part.idleTimer >= part.idleChangeInterval)
                    {
                        UpdateSinglePartExpression(part, false);
                        part.idleTimer = 0f;
                    }
                }
            }
        }
    }

    public void UpdateAllPartsExpression(bool isPressing)
    {
        if (gageScript == null) return;

        int currentLevel = gageScript.levelUp;

        if (!levelGroupMap.TryGetValue(currentLevel, out WakeLevelSettiing currentSetting))
        {
            return;
        }

        foreach (var part in currentSetting.partsConfigs)
        {
            UpdateSinglePartExpression(part, isPressing);
        }
    }

    private void UpdateSinglePartExpression(WakeLevelSettiing.PartExpressionConfig part, bool isPressing)
    {
        if (part.targetSpriteResolver == null) return;

        string currentLabel = part.targetSpriteResolver.GetLabel();

        if (isPressing)
        {
            if (part.spriteLabels.Count > 0)
            {
                string selectedLabel = GetRandomDifferentLabel(part.spriteLabels, currentLabel);
                part.targetSpriteResolver.SetCategoryAndLabel(part.categoryName, selectedLabel);
            }
        }
        else
        {
            if (part.idleSpriteLabels.Count > 0)
            {
                string selectedLabel = GetRandomDifferentLabel(part.idleSpriteLabels, currentLabel);
                part.targetSpriteResolver.SetCategoryAndLabel(part.categoryName, selectedLabel);
            }
        }
    }

    private void ResetAllTimers()
    {
        foreach (var setting in levelGroupMap.Values)
        {
            foreach (var part in setting.partsConfigs)
            {
                part.pressTimer = 0f;
                part.idleTimer = 0f;
            }
        }
    }

    private string GetRandomDifferentLabel(List<string> labels, string currentLabel)
    {
        if (labels.Count == 1) return labels[0];
        List<string> candidates = labels.FindAll(label => label != currentLabel);
        if (candidates.Count == 0) candidates = labels;

        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }
}
