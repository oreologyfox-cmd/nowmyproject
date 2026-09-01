using UnityEngine;
using UnityEngine.EventSystems;

public class braDragInputHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("参照")]
    public braSpriteChange spriteChanger;
    [SerializeField] private SleepGageDown gageDown;

    [SerializeField] private RectTransform targetImageRect;
    // ★追加: 非アクティブにしたいオブジェクトを指定する変数
    [SerializeField] private GameObject targetToDeactivate;

    [Header("切り替えしきい値")]
    public float threshold1 = 50f;
    public float fukubreak = 100f;

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
        if (targetToDeactivate == null)
        {
            targetToDeactivate = gameObject;
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (spriteChanger == null) return;
        cumulativeDelta.y = Mathf.Max(0, cumulativeDelta.y + eventData.delta.y);
        float distance = cumulativeDelta.y;

        rectTransform.anchoredPosition = initialAnchoredPosition + new Vector2(0, distance);

        // --- 破棄（非アクティブ）時の判定 ---
        if (distance > fukubreak)
        {
            if (gageDown != null)
            {
                // ★修正：専用イベントメソッドを呼び出して完全に外れた時のゲージを増やし、最大値フラグを更新
                gageDown.OnUnderwearBreak();
            }

            spriteChanger.Deactivate();

            targetToDeactivate.SetActive(false);
            return;
        }

        // --- 2つのスプライト判定に簡略化 ---
        int targetIndex = (distance > threshold1) ? 1 : 0;

        if (targetIndex != previousIndex)
        {
            if (gageDown != null)
            {
                // ★修正：専用イベントメソッドを呼び出してずらした時のゲージを増やし、最大値フラグを更新
                gageDown.OnUnderwearShift();
            }
            previousIndex = targetIndex;
        }

        spriteChanger.UpdateSprite(targetIndex);

    }

}