using UnityEngine;
using UnityEngine.UI;
using System.Collections; // 【追加】コルーチンを使用するために必要
using System.Collections.Generic;

public enum SlBodyPartType { HOppaileft, HOppairight, HMouse, HLtikubi, HRtikubi, HLegleft, HLegright, HOman, MOppaileft, MOppairight, MMouse, MLtikubi, MRtikubi, MLegleft, MLegright, MOman, TTin }

public class IkuGageDown : MonoBehaviour
{
    [System.Serializable] public class PartSettings { public SlBodyPartType partType; public float increaseMultiplier = 1f; }

    [Header("Dependencies")]
    [SerializeField] private KoufunGageDown koufunGage;

    [Header("UI Components")]
    [SerializeField] private Image squareImage;

    [Header("満タン時に【表示】するオブジェクト")]
    [SerializeField] private GameObject[] objectsToShow;

    [Header("満タン時に【非表示】にするオブジェクト")]
    [SerializeField] private GameObject[] objectsToHide;

    // 【新規追加】演出を継続する時間（秒）
    [Header("演出キープ時間（秒）")]
    [SerializeField] private float effectDuration = 4f;

    [Header("Gage Settings")]
    [Range(0f, 1f)] public float time = 0;
    public float longTapTime = 1f;
    [SerializeField] private float baseIncreaseSpeed = 1f, baseDecreaseSpeed = 1f;

    [Header("レベルによるゲージの上昇率")]
    [SerializeField] private float[] levelIncreaseMultipliers = { 1f, 1.5f, 2f, 2.5f, 3f };

    [SerializeField] private List<PartSettings> partSettingsList = new List<PartSettings>();
    [SerializeField] private int fullCount = 0;

    public int FullCount => fullCount;
    public bool isLocked { get; private set; }
    public float ExternalSpeedMultiplier { get; set; }

    public int Icchata { get; private set; } = 0;
    private int currentKoufunLevel = 0;

    private readonly HashSet<SlBodyPartType> activeParts = new HashSet<SlBodyPartType>();
    private readonly Dictionary<SlBodyPartType, float> increaseSpeedMap = new Dictionary<SlBodyPartType, float>();

    // 【新規追加】重複動作を防ぐためのコルーチン参照用変数
    private Coroutine effectRoutine;

    public void Start()
    {
        UpdateUI();

        // 初期状態では「表示したいオブジェクト」をすべて隠しておく
        SetObjectsActiveState(objectsToShow, false);

        increaseSpeedMap.Clear();
        foreach (var s in partSettingsList)
            if (s != null && !increaseSpeedMap.TryAdd(s.partType, s.increaseMultiplier))
                Debug.LogWarning($"[IkuGageDown] 重複: {s.partType}");

        if (koufunGage != null)
        {
            HandleKoufunLevelUp(koufunGage.levelUp);
            koufunGage.OnLevelUp += HandleKoufunLevelUp;
        }
        else
        {
            Debug.LogError("[IkuGageDown] KoufunGageDown がインスペクタでアタッチされていません。");
        }
    }

    private void OnDestroy()
    {
        if (koufunGage != null)
        {
            koufunGage.OnLevelUp -= HandleKoufunLevelUp;
        }
    }

    private void HandleKoufunLevelUp(int newLevel)
    {
        currentKoufunLevel = newLevel;
        float currentMul = GetCurrentLevelMultiplier();
        Debug.Log($"[IkuGageDown] 興奮レベルを同期。現在の興奮レベル: {currentKoufunLevel} / Iku上昇倍率: {currentMul}倍");
    }

    private float GetCurrentLevelMultiplier()
    {
        if (levelIncreaseMultipliers == null || levelIncreaseMultipliers.Length == 0) return 1f;
        int idx = Mathf.Clamp(currentKoufunLevel, 0, levelIncreaseMultipliers.Length - 1);
        return levelIncreaseMultipliers[idx];
    }

    public void SetPartState(SlBodyPartType part, bool isActive)
    {
        if (!isLocked) { if (isActive) activeParts.Add(part); else activeParts.Remove(part); }
    }

    void Update()
    {
        if (isLocked) return;

        float currentInc = 0f;
        foreach (var p in activeParts) currentInc += increaseSpeedMap.TryGetValue(p, out float m) ? m : 1f;

        if (currentInc > 0 || ExternalSpeedMultiplier > 0)
        {
            Time.timeScale = 0.8f;
            float lvlMul = GetCurrentLevelMultiplier();
            time = Mathf.Min(time + Time.unscaledDeltaTime * baseIncreaseSpeed * (currentInc + ExternalSpeedMultiplier) * lvlMul, 1f);
            UpdateUI();
        }
        else if (time > 0)
        {
            Time.timeScale = 0.2f;
            time = Mathf.Max(time - Time.unscaledDeltaTime * baseDecreaseSpeed, 0f);
            UpdateUI();
            if (time <= 0) Time.timeScale = 1f;
        }

        if (time >= 0.99f) LockAllOperations();
    }

    public void AddGauge(float amount)
    {
        if (isLocked) return;

        float lvlMul = GetCurrentLevelMultiplier();
        time = Mathf.Min(time + amount * lvlMul, 1f);
        UpdateUI();

        if (time >= 0.99f) LockAllOperations();
    }

    private void UpdateUI() { if (squareImage) squareImage.fillAmount = time; }

    private void LockAllOperations()
    {
        Icchata++;
        fullCount++;
        time = 0f;
        UpdateUI();
        Time.timeScale = 1f;

        // 【変更】4秒間のタイマー演出コルーチンを開始する
        if (effectRoutine != null) StopCoroutine(effectRoutine);
        effectRoutine = StartCoroutine(FullGaugeEffectRoutine());
    }

    // 【新規追加】オブジェクトを4秒間切り替えて自動で戻すコルーチン
    private IEnumerator FullGaugeEffectRoutine()
    {
        // 演出中はゲージの再上昇やUpdate処理が動かないようにロック
        isLocked = true;

        // オブジェクトの表示・非表示を切り替え
        SetObjectsActiveState(objectsToShow, true);  // 4秒間表示
        SetObjectsActiveState(objectsToHide, false); // 4秒間非表示

        // 指定された秒数（4秒）だけ待機
        // ※Time.timeScaleの影響を受けないよう「WaitForSecondsRealtime」を使用
        yield return new WaitForSecondsRealtime(effectDuration);

        // 4秒経ったら状態を元に戻す
        RestoreOriginalObjectStates();

        // ロックを解除して次のプレイを可能にする
        isLocked = false;
        effectRoutine = null;
    }

    private void SetObjectsActiveState(GameObject[] targetArray, bool isActive)
    {
        if (targetArray == null) return;

        foreach (var obj in targetArray)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }

    public void RestoreOriginalObjectStates()
    {
        SetObjectsActiveState(objectsToShow, false); // 表示していたものを隠す
        SetObjectsActiveState(objectsToHide, true);  // 隠していたものを元に戻す（表示）
    }

    public void ResetLock()
    {
        // 手動リセットが呼ばれた場合は走っているタイマーを止める
        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
            effectRoutine = null;
        }

        isLocked = false;
        time = 0f;
        UpdateUI();
        RestoreOriginalObjectStates();
    }
}
