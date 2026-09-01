using UnityEngine;

public class SceneFragChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private KoufunGageDown koufunGage;
    [SerializeField] private IkuGageDown ikuGage;
    [SerializeField] private SleepGageDown sleepGage;
    [SerializeField] private ShaseiiGageManagement shaseiiGage;

    [Header("Scene Flags")]
    [Header("バッドエンド: 睡眠1.0以上 且つ 興奮1以下")]
    [SerializeField] private bool Bad;

    [Header("続く: 睡眠1.0以上 且つ 興奮2以上 (Normal3, 4, 5 のいずれにも該当しない時)")]
    [SerializeField] private bool Continue;

    [Header("介抱エンド: 興奮0 且つ イク0 且つ 睡眠1.0未満")]
    [SerializeField] private bool normal;

    [Header("睡眠感エンド: 興奮2以上 且つ 睡眠1.0未満")]
    [SerializeField] private bool Normal2;

    [Header("快楽エンド①: イク2 且つ 睡眠1.0以上 且つ 興奮3 且つ 射精3以外")]
    [SerializeField] private bool Normal3;

    [Header("快楽エンド②: イク4 且つ 睡眠1.0以上 且つ 興奮3 且つ 射精3")]
    [SerializeField] private bool Normal4;

    [Header("起床エンド: 興奮2以上 且つ 睡眠1.0以上 (Normal3, 4の条件を満たさない時)")]
    [SerializeField] private bool Normal5;

    public bool IsScene1 => Bad;
    public bool IsNormal => normal;
    public bool IsNormal2 => Normal2;
    public bool IsNormal3 => Normal3;
    public bool IsNormal4 => Normal4;
    public bool IsNormal5 => Normal5;
    public bool IsContinue => Continue;

    void Start()
    {
        UpdateAllFlags();
    }

    void Update()
    {
        UpdateAllFlags();
    }

    private void UpdateAllFlags()
    {
        if (koufunGage == null || ikuGage == null || sleepGage == null || shaseiiGage == null) return;

        // --- 1. 基本ステータスの整理 ---
        bool isSleepHigh = sleepGage.time >= 1.0f; // 睡眠 1.0以上（旧 0.99f）
        bool isSleepLow = sleepGage.time < 1.0f;  // 睡眠 1.0未満

        // --- 2. 各エンディングの【完全排他】条件式 ---

        // 【Normal4】
        bool isScene4Condition = isSleepHigh && koufunGage.levelUp == 3 && ikuGage.FullCount == 4 && shaseiiGage.CurrentFillCount == 3;

        // 【Normal3】（Normal4と重複しないよう、射精カウントかイクカウントで差別化）
        bool isScene3Condition = isSleepHigh && koufunGage.levelUp == 3 && ikuGage.FullCount == 2 && shaseiiGage.CurrentFillCount != 3;

        // 【Normal5】（睡眠1.0以上・興奮2以上 のうち、Normal3/4に該当しない残りすべて）
        bool isScene5Condition = isSleepHigh && koufunGage.levelUp >= 2 && !isScene3Condition && !isScene4Condition;

        // 【Bad】（睡眠1.0以上・興奮1以下）
        bool isScene1Condition = isSleepHigh && koufunGage.levelUp <= 1;

        // 【Normal2】（睡眠1.0未満・興奮2以上）
        bool isScene2Condition = isSleepLow && koufunGage.levelUp >= 2;

        // 【Normal】（睡眠1.0未満・興奮0・イク0）
        bool isNormalCondition = isSleepLow && koufunGage.levelUp == 0 && ikuGage.FullCount == 0;

        // --- 3. 「続く(Continue)」の条件整理 ---
        // 睡眠1.0以上 且つ 興奮2以上 のうち、エンド（Normal3, 4, 5）にならなかった場合の受け皿
        bool isContinueCondition = isSleepHigh && koufunGage.levelUp > 1 && !isScene3Condition && !isScene4Condition && !isScene5Condition;


        // --- 4. フラグへの代入（条件が競合しないため、if-elseの順序に依存しません） ---
        Bad = isScene1Condition;
        Normal2 = isScene2Condition;
        Normal3 = isScene3Condition;
        Normal4 = isScene4Condition;
        Normal5 = isScene5Condition;
        normal = isNormalCondition;
        Continue = isContinueCondition;
    }
}
