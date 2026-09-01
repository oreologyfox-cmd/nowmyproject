using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GageDownPartType
{
    HOppaileft, HOppairight, HMouse, HLtikubi, HRtikubi, HLegleft, HLegright, HOman,
    MOppaileft, MOppairight, MMouse, MLtikubi, MRtikubi, MLegleft, MLegright, MOman,
    TTin
}

public class SleepGageDown : MonoBehaviour
{
    [System.Serializable]
    public class PartSettings
    {
        public GageDownPartType partType;
        [Tooltip("押されている時の増加速度倍率")] public float increaseMultiplier = 1.0f;
        [Tooltip("ドラッグされている時の増加速度倍率")] public float dragMultiplier = 1.5f;
    }

    [Header("References")]
    [SerializeField] private KoufunGageDown koufunGageDown;
    [SerializeField] private Image squareImage;

    [Header("Gage Settings")]
    [Range(0f, 1f)] public float time = 0;
    [SerializeField] private float baseIncreaseSpeed = 1.0f;
    [SerializeField] private float baseDecreaseSpeed = 1.0f;

    [Header("Amounts (All 0.2f)")]
    [SerializeField] private float parkerNugashiAmount = 0.2f;
    [SerializeField] private float underwearShiftAmount = 0.2f;
    [SerializeField] private float underwearBreakAmount = 0.2f;
    [SerializeField] private float skirtShiftAmount = 0.2f;
    [SerializeField] private float skirtBreakAmount = 0.2f;
    [SerializeField] private float yshirtButtonAmount = 0.2f;

    [Header("Level Speed Modifiers")]
    [SerializeField] private float[] levelIncreaseMultipliers = { 1.0f, 0.8f, 0.6f, 0.4f, 0.2f };

    [Header("Speed Settings Per Part")]
    [SerializeField] private List<PartSettings> partSettingsList = new List<PartSettings>();

    private readonly HashSet<GageDownPartType> activeParts = new HashSet<GageDownPartType>();
    private readonly HashSet<GageDownPartType> activeDragParts = new HashSet<GageDownPartType>();
    private Dictionary<GageDownPartType, PartSettings> settingsMap;

    private bool hasReachedMax;

    // プロパティ
    public float ParkerNugashiAmount => parkerNugashiAmount;
    public float UnderwearShiftAmount => underwearShiftAmount;
    public float UnderwearBreakAmount => underwearBreakAmount;
    public float SkirtShiftAmount => skirtShiftAmount;
    public float SkirtBreakAmount => skirtBreakAmount;
    public float YshirtButtonAmount => yshirtButtonAmount;
    public float BraShiftAmount => underwearShiftAmount;
    public float BraBreakAmount => underwearBreakAmount;

    // 【追加】外部（InsertSpeedManagerなど）からピストン速度倍率を受け取るプロパティ
    public float ExternalSpeedMultiplier { get; set; } = 0f;

    void Start()
    {
        time = 0f; // 【追加】ゲーム開始時に強制的に0にする
        if (squareImage) squareImage.fillAmount = time;
        if (!koufunGageDown) koufunGageDown = FindFirstObjectByType<KoufunGageDown>();

        // 重複エラーを回避しつつDictionary化
        settingsMap = new Dictionary<GageDownPartType, PartSettings>();
        foreach (var setting in partSettingsList)
        {
            if (!settingsMap.ContainsKey(setting.partType))
            {
                settingsMap.Add(setting.partType, setting);
            }
            else
            {
                Debug.LogWarning($"[SleepGageDown] 重複した部位設定があります: {setting.partType}");
            }
        }
    }

    public void SetPartState(GageDownPartType part, bool isActive, bool isDragging = false)
    {
        var targetSet = isDragging ? activeDragParts : activeParts;
        if (isActive) targetSet.Add(part); else targetSet.Remove(part);
    }

    void Update()
    {
        if (hasReachedMax)
        {
            UpdateGauge(1.0f);
            return;
        }

        float mult = GetCurrentIncreaseMultiplier();
        float delta = Time.unscaledDeltaTime;

        // 部位が愛撫されている(mult > 0) か、またはピストン運動している(ExternalSpeedMultiplier > 0) ときに増加
        if (mult > 0 || ExternalSpeedMultiplier > 0)
        {
            float totalMult = mult + ExternalSpeedMultiplier;
            UpdateGauge(time + (delta * baseIncreaseSpeed * totalMult * GetLevelIncreaseModifier()));
        }
        else if (time > 0)
        {
            UpdateGauge(time - (delta * baseDecreaseSpeed));
        }
    }

    public void AddGauge(float amount) => UpdateGauge(time + amount);
    public void OnParkerNugashi() => AddGauge(parkerNugashiAmount);
    public void OnUnderwearShift() => AddGauge(underwearShiftAmount);
    public void OnUnderwearBreak() => AddGauge(underwearBreakAmount);
    public void OnSkirtShift() => AddGauge(skirtShiftAmount);
    public void OnSkirtBreak() => AddGauge(skirtBreakAmount);
    public void OnYshirtButton() => AddGauge(yshirtButtonAmount);

    private void UpdateGauge(float newTime)
    {
        time = Mathf.Clamp01(newTime);
        if (squareImage) squareImage.fillAmount = time;
        if (time >= 0.999f) hasReachedMax = true;
    }

    // GC Alloc（ゴミ）を出さないように foreach ループで高速化
    float GetCurrentIncreaseMultiplier()
    {
        float sum = 0f;

        foreach (var p in activeParts)
        {
            if (settingsMap.TryGetValue(p, out var s))
                sum += s.increaseMultiplier;
        }

        foreach (var p in activeDragParts)
        {
            if (settingsMap.TryGetValue(p, out var s))
                sum += s.dragMultiplier;
        }

        return sum;
    }

    float GetLevelIncreaseModifier()
    {
        if (!koufunGageDown || levelIncreaseMultipliers == null || levelIncreaseMultipliers.Length == 0) 
            return 1.0f;

        int lv = Mathf.Clamp(koufunGageDown.levelUp, 0, levelIncreaseMultipliers.Length - 1);
        return levelIncreaseMultipliers[lv];
    }

    public void ResetMaxFlag() => hasReachedMax = false;
}
