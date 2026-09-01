using UnityEngine;
using UnityEngine.EventSystems;

public class OmanMouthHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("どのスプライトか")]
    public OmanMouthChange spriteChanger;
    [Header("ゲージ用")]
    [SerializeField] private SleepGageDown gageDown;
    [Header("どこの位置を参照するか")]
    [SerializeField] private RectTransform targetImageRect;

    [Header("クリックの量")]
    public float threshold1 = 50f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 cumulativeDelta = Vector2.zero;
    private Vector2 initialAnchoredPosition;

    private int previousIndex = 0;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        initialAnchoredPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ロックチェックを削除
        if (spriteChanger == null) return;

        cumulativeDelta.y = Mathf.Max(0, cumulativeDelta.y + eventData.delta.y);

        float distance = cumulativeDelta.y;

        rectTransform.anchoredPosition = initialAnchoredPosition + new Vector2(0, distance);

        // --- 2つのスプライト切り替え ---
        int targetIndex = (distance > threshold1) ? 1 : 0;

        if (targetIndex != previousIndex)
        {
            if (gageDown != null)
            {
                gageDown.AddGauge(0.2f);
            }
            previousIndex = targetIndex;
        }

        spriteChanger.UpdateSprite(targetIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 位置を初期位置に戻す
        rectTransform.anchoredPosition = initialAnchoredPosition;

        // 移動量の蓄積をリセットする
        cumulativeDelta = Vector2.zero;

        // スプライトのインデックスを初期状態（0）に戻す
        previousIndex = 0;
        if (spriteChanger != null)
        {
            spriteChanger.UpdateSprite(0);
        }
    }
}
