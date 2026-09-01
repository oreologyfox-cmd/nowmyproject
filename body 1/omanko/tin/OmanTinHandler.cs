using UnityEngine;
using UnityEngine.EventSystems;
using System; // ★これが必要です

[RequireComponent(typeof(RectTransform))]
public class OmanTinHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("参照")]
    [SerializeField] private OmanTinChange tinChanger;
    [SerializeField] private SleepGageDown gageDown;
    [SerializeField] private RectTransform targetImageRect;

    [Header("表示制御するオブジェクト（常に表示されます）")]
    [SerializeField] private GameObject activeObjectOnHold;

    [Header("切り替えしきい値（上昇時）")]
    [SerializeField] private float threshold1 = 20.0f;
    [SerializeField] private float threshold2 = 50.0f;
    [SerializeField] private float threshold3 = 80.0f;

    [Header("ボタンの移動距離")]
    [SerializeField] private float maxhold = 100.0f;

    [Header("戻り時の遊び（ピクセル数）")]
    [SerializeField] private float hysteresis = 10f;

    // ★外部のスクリプトがインデックスの変化を検知するためのイベントを追加
    public event Action<int> OnIndexChanged;

    private RectTransform rectTransform;
    private Vector2 initialAnchoredPosition;

    // 現在ドラッグをスタートした基準点
    private Vector2 currentBaseAnchoredPosition;
    private int currentIndex = 0;

    private const float GaugeIncrement = 0f;

    private RectTransform parentRectTransform;
    private Vector2 dragOffset = Vector2.zero;

    // 現在のドラッグ中にロックがかかっているか
    private bool isLockedInCurrentDrag = false;
    private float lockedHeight = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            parentRectTransform = rectTransform.parent as RectTransform;
        }

        if (activeObjectOnHold != null)
        {
            activeObjectOnHold.SetActive(true);
        }
    }

    private void Start()
    {
        if (rectTransform != null)
        {
            initialAnchoredPosition = rectTransform.anchoredPosition;
            currentBaseAnchoredPosition = initialAnchoredPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rectTransform == null || parentRectTransform == null) return;

        isLockedInCurrentDrag = false;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localCursorPos))
        {
            dragOffset = rectTransform.anchoredPosition - localCursorPos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (tinChanger == null || rectTransform == null || parentRectTransform == null) return;

        if (isLockedInCurrentDrag) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localCursorPos))
        {
            Vector2 targetAnchoredPos = localCursorPos + dragOffset;

            float totalHeightFromInitial = targetAnchoredPos.y - initialAnchoredPosition.y;
            totalHeightFromInitial = Mathf.Clamp(totalHeightFromInitial, 0f, maxhold);

            CheckThresholdAndLock(totalHeightFromInitial);

            float finalHeight = isLockedInCurrentDrag ? lockedHeight : totalHeightFromInitial;

            rectTransform.anchoredPosition = new Vector2(initialAnchoredPosition.x, initialAnchoredPosition.y + finalHeight);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        currentBaseAnchoredPosition = rectTransform.anchoredPosition;
        dragOffset = Vector2.zero;
    }

    private void CheckThresholdAndLock(float distance)
    {
        int targetIndex = currentIndex;

        float t1 = (currentIndex == 0) ? threshold1 : (threshold1 - hysteresis);
        float t2 = (currentIndex <= 1) ? threshold2 : (threshold2 - hysteresis);
        float t3 = (currentIndex <= 2) ? threshold3 : (threshold3 - hysteresis);

        if (distance > t3) targetIndex = 3;
        else if (distance > t2) targetIndex = 2;
        else if (distance > t1) targetIndex = 1;
        else targetIndex = 0;

        if (targetIndex > currentIndex)
        {
            if (gageDown != null)
            {
                int steps = Mathf.Abs(targetIndex - currentIndex);
                for (int i = 0; i < steps; i++)
                {
                    gageDown.AddGauge(GaugeIncrement);
                }
            }

            currentIndex = targetIndex;
            tinChanger.UpdateSprite(targetIndex);

            // ★イベントを通知
            OnIndexChanged?.Invoke(currentIndex);

            isLockedInCurrentDrag = true;

            if (targetIndex == 1) lockedHeight = threshold1;
            else if (targetIndex == 2) lockedHeight = threshold2;
            else if (targetIndex == 3) lockedHeight = threshold3;
        }
        else if (targetIndex < currentIndex)
        {
            currentIndex = targetIndex;
            tinChanger.UpdateSprite(targetIndex);

            // ★イベントを通知（下がったとき用）
            OnIndexChanged?.Invoke(currentIndex);
        }
    }

    public void ResetToDefault()
    {
        isLockedInCurrentDrag = false;
        currentIndex = 0;
        currentBaseAnchoredPosition = initialAnchoredPosition;
        rectTransform.anchoredPosition = initialAnchoredPosition;
        if (tinChanger != null) tinChanger.UpdateSprite(0);

        // ★リセット時も通知
        OnIndexChanged?.Invoke(0);
    }
}
