using UnityEngine;
using UnityEngine.EventSystems;

public class LtkbiMouthHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("変化させるスクリプトがあるオブジェクト")]
    public LtkbMouthChange spriteChanger;
    [Header("ゲージを増減させるオブジェクト")]
    [SerializeField] private SleepGageDown gageDown;
    [Header("位置記憶用")]
    [SerializeField] private RectTransform targetImageRect;

    [Header("切り替えしきい値")]
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

        // --- 2つのスプライト判定 ---
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

        // 移動距離の蓄積をリセットする
        cumulativeDelta = Vector2.zero;

        // スプライトのインデックスを初期状態（0）に戻す
        previousIndex = 0;
        if (spriteChanger != null)
        {
            spriteChanger.UpdateSprite(0);
        }
    }
}
