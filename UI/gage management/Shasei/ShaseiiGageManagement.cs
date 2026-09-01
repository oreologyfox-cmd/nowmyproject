using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable] public class GageChangedEvent : UnityEvent<float> { }

public class ShaseiiGageManagement : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private Image squareImage;
    [SerializeField] private List<GameObject> dragTargetObjects = new List<GameObject>();
    [SerializeField] private Scrollbar targetScrollbar;
    [SerializeField] private Nakadasibutton submitButton;

    [Header("速度設定")]
    [SerializeField] private float baseScrollSpeed = 0.2f;
    [SerializeField] private float scrollThreshold = 0.1f;
    [SerializeField] private float increaseSpeed = 0.2f;

    [Header("イベント・デバッグ")]
    public GageChangedEvent OnGageChanged;
    [SerializeField] private float Headseieki;
    [SerializeField] private float RtinSeieki;

    private bool isDragging, isLocked;
    private ShaseiGageHilight gageHilight;

    public int CurrentFillCount => submitButton != null ? submitButton.CurrentFillCount : 0;
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    public Image SquareImage => squareImage;
    public ShaseiGageHilight GageHilight => gageHilight;
    public float CurrentHeadseieki => Headseieki;
    public float CurrentRtinSeieki => RtinSeieki;

    void Start()
    {
        if (squareImage == null) return;
        gageHilight = squareImage.GetComponent<ShaseiGageHilight>();
        ResetGage();
        SetupDragEvents();
    }

    private void SetupDragEvents()
    {
        if (dragTargetObjects == null) return;
        foreach (var t in dragTargetObjects)
        {
            if (t == null) continue;
            var trigger = t.GetComponent<EventTrigger>() ?? t.AddComponent<EventTrigger>();
            AddTrigger(trigger, EventTriggerType.BeginDrag, data => { if (!isLocked) isDragging = true; });
            AddTrigger(trigger, EventTriggerType.EndDrag, data => isDragging = false);
            AddTrigger(trigger, EventTriggerType.PointerUp, data => isDragging = false);
        }
    }

    private void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    void Update()
    {
        if (isLocked || squareImage.fillAmount >= 1f) return;

        bool isScroll = targetScrollbar != null && targetScrollbar.value >= scrollThreshold;
        if (isDragging || isScroll)
        {
            float amt = (isDragging ? increaseSpeed : 0) + (isScroll ? baseScrollSpeed * targetScrollbar.value : 0);
            AddGageAmount(amt * Time.deltaTime);
        }
    }

    private void AddGageAmount(float amount)
    {
        if (squareImage == null || isLocked) return;
        squareImage.fillAmount = Mathf.Min(squareImage.fillAmount + amount, 1f);
        UpdateVariables(squareImage.fillAmount);
        OnGageChanged?.Invoke(squareImage.fillAmount);

        if (squareImage.fillAmount >= 1f) { isDragging = false; }
    }

    public void ResetGage()
    {
        if (squareImage == null) return;
        squareImage.fillAmount = 0f;
        UpdateVariables(0f);
        OnGageChanged?.Invoke(0f);
    }

    private void UpdateVariables(float v) => Headseieki = RtinSeieki = v;
}
