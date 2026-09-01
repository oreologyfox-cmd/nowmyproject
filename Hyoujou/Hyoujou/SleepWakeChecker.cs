using UnityEngine;

public class SleepWakeChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SleepGageDown sleepGageDown;

    [Tooltip("睡眠時の表情管理コンポーネント")]
    [SerializeField] private SleepHyoujouManager sleepHyoujouManager;

    [Tooltip("起床時の表情管理コンポーネント")]
    [SerializeField] private WakeHyoujouManager wakeHyoujouManager;

    [Header("Settings")]
    [SerializeField] private float setupDelay = 1.0f;

    [Header("Status")]
    [SerializeField] private bool isConditionMet = false;

    private float elapsedTime = 0f;
    private bool? lastConditionState = null; // 前回の状態を記憶して、変化した瞬間だけ処理する

    public bool IsConditionMet => isConditionMet;

    void Start()
    {
        if (sleepGageDown == null) sleepGageDown = Object.FindFirstObjectByType<SleepGageDown>();

        // 表情マネージャーが未設定なら自動取得
        if (sleepHyoujouManager == null) sleepHyoujouManager = GetComponent<SleepHyoujouManager>();
        if (wakeHyoujouManager == null) wakeHyoujouManager = GetComponent<WakeHyoujouManager>();

        isConditionMet = false;
        lastConditionState = null;

        // シーン開始直後は安全のため両方オフにしておく（ディレイ後に正しく初期化されます）
        SetManagersActive(false, false);
    }

    void Update()
    {
        if (sleepGageDown == null)
        {
            isConditionMet = false;
            ToggleManagers(false);
            return;
        }

        // セットアップディレイ中の処理
        if (elapsedTime < setupDelay)
        {
            elapsedTime += Time.deltaTime;
            isConditionMet = false;
            return;
        }

        // ゲージが 0.99f 以上なら起床（Wake）、未満なら睡眠（Sleep）
        bool isSleepGageMax = sleepGageDown.time >= 0.99f;
        isConditionMet = isSleepGageMax;

        // 状態（Sleep / Wake）が変わった瞬間、または初期判定の時だけ切り替え処理を実行
        ToggleManagers(isConditionMet);
    }

    /// <summary>
    /// 条件に応じてコンポーネントの有効状態を切り替える
    /// </summary>
    private void ToggleManagers(bool isWake)
    {
        // 状態に変更がない場合は何もしない（無駄なコンポーネント有効化・無効化の負荷を減らす）
        if (lastConditionState.HasValue && lastConditionState.Value == isWake) return;

        lastConditionState = isWake;

        if (isWake)
        {
            // Wakeを有効に、Sleepを無効にする
            SetManagersActive(sleepActive: false, wakeActive: true);
        }
        else
        {
            // Sleepを有効に、Wakeを無効にする
            SetManagersActive(sleepActive: true, wakeActive: false);
        }
    }

    /// <summary>
    /// コンポーネントの enabled を安全に制御するヘルパーメソッド
    /// </summary>
    private void SetManagersActive(bool sleepActive, bool wakeActive)
    {
        if (sleepHyoujouManager != null)
        {
            sleepHyoujouManager.enabled = sleepActive;

            // 有効化された瞬間に、初期状態で表情が即座に反映されるように呼び出す
            if (sleepActive) sleepHyoujouManager.UpdateAllPartsExpression(false);
        }

        if (wakeHyoujouManager != null)
        {
            wakeHyoujouManager.enabled = wakeActive;

            // 有効化された瞬間に、初期状態で表情が即座に反映されるように呼び出す
            if (wakeActive) wakeHyoujouManager.UpdateAllPartsExpression(false);
        }
    }
}
