using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SleepHyoujouManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("ゲージ管理スクリプト（KoufunGageDown）")]
    public KoufunGageDown gageScript;

    [Tooltip("条件判定スクリプト（SleepWakeChecker）")]
    [SerializeField] private SleepWakeChecker conditionChecker;

    [Header("Sprite Library Settings")]
    [Tooltip("使用する Sprite Library Asset（エディタのドロップダウン抽出用）")]
    public SpriteLibraryAsset libraryAsset;

    private Dictionary<int, SleepLevelSetting> levelGroupMap = new Dictionary<int, SleepLevelSetting>();
    private int lastLevel = -1;
    private bool wasPressing = false;

    void Start()
    {
        // 自身にアタッチされている ExpressionLevelSetting をすべて取得
        SleepLevelSetting[] settings = GetComponents<SleepLevelSetting>();

        foreach (var setting in settings)
        {
            if (!levelGroupMap.ContainsKey(setting.targetLevel))
            {
                levelGroupMap[setting.targetLevel] = setting;
            }
            else
            {
                Debug.LogWarning($"重複エラー: レベル {setting.targetLevel} の ExpressionLevelSetting が複数アタッチされています。");
            }
        }

        if (conditionChecker == null)
        {
            conditionChecker = FindFirstObjectByType<SleepWakeChecker>();
        }

        // 初期化時にすべてのパーツを一回更新
        UpdateAllPartsExpression(false);
    }

    void Update()
    {
        if (conditionChecker != null && conditionChecker.IsConditionMet)
        {
            ResetAllTimers();
            return;
        }

        if (gageScript == null) return;

        bool isPressing = false;
        if (Pointer.current != null)
        {
            isPressing = Pointer.current.press.isPressed;
        }

        int currentLevel = gageScript.levelUp;

        // レベルや入力状態が変わった瞬間に即時更新
        if (currentLevel != lastLevel || isPressing != wasPressing)
        {
            UpdateAllPartsExpression(isPressing);
            ResetAllTimers();

            lastLevel = currentLevel;
            wasPressing = isPressing;
            return;
        }

        // 現在のレベルの設定を取得して、全パーツ個別にタイマー処理
        if (levelGroupMap.TryGetValue(currentLevel, out SleepLevelSetting currentSetting))
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

        wasPressing = isPressing;
    }

    // 全パーツの表情をまとめて更新する
    public void UpdateAllPartsExpression(bool isPressing)
    {
        if (gageScript == null) return;

        int currentLevel = gageScript.levelUp;

        if (!levelGroupMap.TryGetValue(currentLevel, out SleepLevelSetting currentSetting))
        {
            return;
        }

        foreach (var part in currentSetting.partsConfigs)
        {
            UpdateSinglePartExpression(part, isPressing);
        }
    }

    // 単一パーツの表情を更新する
    private void UpdateSinglePartExpression(SleepLevelSetting.PartExpressionConfig part, bool isPressing)
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
