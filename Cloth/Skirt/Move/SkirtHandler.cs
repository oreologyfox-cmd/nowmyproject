using UnityEngine;
using UnityEngine.EventSystems;

public class SkirtHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("参照")]
    public SkirtSpriteChange spriteChanger;
    public SleepGageDown gageDown;

    [SerializeField] private RectTransform targetImageRect;

    [Header("★追加: ON/OFFオブジェクト")]
    public GameObject skirtOnOffObject;

    [Header("★破棄時に非アクティブにするオブジェクト")]
    public GameObject targetDeactivateObject;

    [Header("切り替えしきい値")]
    public float threshold1 = 50f;
    public float threshold2 = 100f;
    public float threshold3 = 150f;
    public float fukubreak = 200f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 cumulativeDelta = Vector2.zero;

    private Vector2 initialAnchoredPosition;

    private int currentIndex = 0;

    void Awake()
    {
        // ★修正ポイント：Startよりも前に実行されるAwakeでコンポーネントを取得
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
        if (spriteChanger == null) return;
        cumulativeDelta.y = Mathf.Max(0, cumulativeDelta.y + eventData.delta.y);
        float distance = cumulativeDelta.y;

        rectTransform.anchoredPosition = initialAnchoredPosition + new Vector2(0, distance);

        // --- 破棄（非アクティブ）時の判定 ---
        if (distance > fukubreak)
        {
            if (gageDown != null)
            {
                // ★修正：専用イベントメソッドを呼び出して完全に脱げた時のゲージを増やし、最大値フラグを更新
                gageDown.OnSkirtBreak();
            }

            if (skirtOnOffObject != null)
            {
                skirtOnOffObject.SetActive(false);
            }

            spriteChanger.Deactivate();

            if (targetDeactivateObject != null)
            {
                targetDeactivateObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
            return;
        }

        int targetIndex;
        if (distance > threshold3) targetIndex = 3;
        else if (distance > threshold2) targetIndex = 2;
        else if (distance > threshold1) targetIndex = 1;
        else targetIndex = 0;

        if (targetIndex != currentIndex)
        {
            currentIndex = targetIndex;
            spriteChanger.UpdateSprite(targetIndex);

            if (gageDown != null)
            {
                // ★修正：専用イベントメソッドを呼び出してめくった時のゲージを増やし、最大値フラグを更新
                gageDown.OnSkirtShift();
            }
        }

    }

}