using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class EffectManager : MonoBehaviour
{
    [System.Serializable]
    public class ExpressionLevelGroup
    {
        [Tooltip("レベル（0〜3）")]
        public int level;
        [Tooltip("このレベルの時の表情切り替え間隔（秒数）")]
        public float changeInterval = 3.0f;
        [Tooltip("ランダム表示させたいラベル名")]
        public List<string> spriteLabels = new List<string>();
    }

    [Header("References")]
    [Tooltip("ゲージ管理スクリプト（KoufunGageDown）")]
    public KoufunGageDown gageScript;

    [Tooltip("表情を変化させたいオブジェクトの SpriteResolver")]
    public SpriteResolver targetSpriteResolver;

    [Header("Sprite Library Settings")]
    [Tooltip("使用する Sprite Library Asset")]
    public SpriteLibraryAsset libraryAsset;

    [Tooltip("Sprite Library Asset 内のカテゴリー名")]
    public string categoryName = "Face";

    [Header("Idle Settings")]
    [Tooltip("クリック・ドラッグされていない時の表情切り替え間隔（秒数）")]
    public float idleChangeInterval = 5.0f;

    // 💡 変更点：非操作時の専用表情ラベルリスト
    [Tooltip("クリック・ドラッグされていない時にランダム表示させたいラベル名")]
    public List<string> idleSpriteLabels = new List<string>();

    public List<ExpressionLevelGroup> expressionGroups = new List<ExpressionLevelGroup>();

    private Dictionary<int, ExpressionLevelGroup> levelGroupMap = new Dictionary<int, ExpressionLevelGroup>();
    private int lastLevel = -1;
    private float pressTimer = 0f;
    private float idleTimer = 0f;

    void Start()
    {
        foreach (var group in expressionGroups)
        {
            if (!levelGroupMap.ContainsKey(group.level))
            {
                levelGroupMap[group.level] = group;
            }
        }

        UpdateExpression(false); // 初期状態は非操作時として更新
    }

    void Update()
    {
        if (gageScript == null) return;

        // レベルが変わった時だけ即座に表情を更新して両方のタイマーをリセット
        if (gageScript.levelUp != lastLevel)
        {
            // レベル変更時は、現在の操作状態に合わせて表情を即時更新
            bool isPressingNow = false;
            if (Pointer.current != null)
            {
                isPressingNow = Pointer.current.press.isPressed;
            }
            UpdateExpression(isPressingNow);

            pressTimer = 0f;
            idleTimer = 0f;
        }

        bool isPressing = false;
        if (Pointer.current != null)
        {
            isPressing = Pointer.current.press.isPressed;
        }

        int currentLevel = gageScript.levelUp;

        if (isPressing)
        {
            idleTimer = 0f; // 非操作タイマーをリセット

            if (levelGroupMap.TryGetValue(currentLevel, out ExpressionLevelGroup group))
            {
                pressTimer += Time.unscaledDeltaTime;

                if (pressTimer >= group.changeInterval)
                {
                    UpdateExpression(true); // 操作時として更新
                    pressTimer = 0f;
                }
            }
        }
        else
        {
            pressTimer = 0f; // 操作タイマーをリセット
            idleTimer += Time.unscaledDeltaTime;

            if (idleTimer >= idleChangeInterval)
            {
                UpdateExpression(false); // 💡 非操作時として更新
                idleTimer = 0f;
            }
        }
    }

    // 💡 変更点：引数 isPressing によって抽選するリストを分岐
    public void UpdateExpression(bool isPressing)
    {
        if (gageScript == null || targetSpriteResolver == null) return;

        int currentLevel = gageScript.levelUp;
        lastLevel = currentLevel;

        if (isPressing)
        {
            // 操作時はレベルごとのグループから抽選
            if (levelGroupMap.TryGetValue(currentLevel, out ExpressionLevelGroup group) && group.spriteLabels.Count > 0)
            {
                int randomIndex = Random.Range(0, group.spriteLabels.Count);
                string selectedLabel = group.spriteLabels[randomIndex];
                targetSpriteResolver.SetCategoryAndLabel(categoryName, selectedLabel);
            }
            else
            {
                Debug.LogWarning($"レベル {currentLevel} に対応する表情ラベルが設定されていません。");
            }
        }
        else
        {
            // 💡 非操作時は専用の idleSpriteLabels から抽選
            if (idleSpriteLabels.Count > 0)
            {
                int randomIndex = Random.Range(0, idleSpriteLabels.Count);
                string selectedLabel = idleSpriteLabels[randomIndex];
                targetSpriteResolver.SetCategoryAndLabel(categoryName, selectedLabel);
            }
            else
            {
                // もし非操作専用が空なら、安全のために現在のレベルの通常表情を代わりに流用
                if (levelGroupMap.TryGetValue(currentLevel, out ExpressionLevelGroup group) && group.spriteLabels.Count > 0)
                {
                    int randomIndex = Random.Range(0, group.spriteLabels.Count);
                    string selectedLabel = group.spriteLabels[randomIndex];
                    targetSpriteResolver.SetCategoryAndLabel(categoryName, selectedLabel);
                }
                else
                {
                    Debug.LogWarning("非操作時の専用表情(Idle Sprite Labels)も、レベル対応表情も設定されていません。");
                }
            }
        }
    }
}
