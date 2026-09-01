using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum BodyPartType
{
    HOman, Hmouth, HOppaiLeft, HOppaiRight, HTkbLeft, HTkbRight, HLegLeft, HLegRight,
    MOman, MMouse, MOppaiLeft, MOppaiRight, MTkbLeft, MTkbRight, MLegLeft, MLegRight,
    TMman, TMouth
}

public class KoufunGageDown : MonoBehaviour
{
    [System.Serializable]
    public class PartSettings
    {
        public BodyPartType partType;
        public float speedMultiplier = 1.0f;
    }

    [Header("UI Components")]
    [SerializeField] private Image squareImage;

    [Header("Gage Settings")]
    [Range(0f, 1f)][SerializeField] private float ikutime = 0;
    public float ikulongTapTime = 1.0f;
    [SerializeField] public float increaseSpeed = 1.0f;

    [Header("Speed Settings Per Part")]
    [SerializeField] private List<PartSettings> partSettingsList = new List<PartSettings>();

    [Header("Level System")]
    public int levelUp = 0;
    private const int MaxLevel = 4; // カンストレベル

    public event Action<int> OnLevelUp;

    public float Ikutime
    {
        get => ikutime;
        private set
        {
            if (Mathf.Approximately(ikutime, value)) return;
            ikutime = Mathf.Clamp01(value);
            if (squareImage != null) squareImage.fillAmount = ikutime;
        }
    }

    public float ExternalSpeedMultiplier { get; set; } = 0f;

    private readonly HashSet<BodyPartType> activeParts = new HashSet<BodyPartType>();
    private readonly Dictionary<BodyPartType, float> speedMap = new Dictionary<BodyPartType, float>();

    void Start()
    {
        if (squareImage != null) squareImage.fillAmount = ikutime;

        foreach (var setting in partSettingsList)
        {
            if (!speedMap.ContainsKey(setting.partType))
            {
                speedMap.Add(setting.partType, setting.speedMultiplier);
            }
            else
            {
                Debug.LogWarning($"[KoufunGageDown] 重複した部位設定があります: {setting.partType}");
            }
        }

        foreach (BodyPartType type in Enum.GetValues(typeof(BodyPartType)))
        {
            if (!speedMap.ContainsKey(type))
            {
                speedMap.Add(type, 1.0f);
            }
        }
    }

    public void SetPartState(BodyPartType part, bool isActive)
    {
        if (isActive) activeParts.Add(part);
        else activeParts.Remove(part);
    }

    void Update()
    {
        if (levelUp >= MaxLevel) return;

        float currentMultiplier = GetCurrentSpeedMultiplier();

        if (currentMultiplier > 0 || ExternalSpeedMultiplier > 0)
        {
            float totalMultiplier = currentMultiplier + ExternalSpeedMultiplier;

            // 計算式からレベル倍率（lvlMul）の乗算を削除
            Ikutime += (Time.unscaledDeltaTime / ikulongTapTime) * increaseSpeed * totalMultiplier;

            if (Ikutime >= 1.0f)
            {
                levelUp++;
                Debug.Log($"[KoufunGageDown] レベルアップ: {levelUp}");
                OnLevelUp?.Invoke(levelUp);

                if (levelUp < MaxLevel)
                {
                    Ikutime = 0f;
                }
            }
        }
    }

    float GetCurrentSpeedMultiplier()
    {
        float totalMultiplier = 0f;
        foreach (var part in activeParts)
        {
            if (speedMap.TryGetValue(part, out float multiplier))
            {
                totalMultiplier += multiplier;
            }
        }
        return totalMultiplier;
    }
}
