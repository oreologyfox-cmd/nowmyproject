using UnityEngine;
using UnityEngine.EventSystems;

public class PantsDragInputHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("参照")]
    public PantSpriteChange spriteChanger;
    public SleepGageDown gageDown;

    [SerializeField] private RectTransform targetImageRect;
    // ★非アクティブにしたいオブジェクトを指定する変数（未指定なら自分を非アクティブ化）
    [SerializeField] private GameObject targetToDeactivate;

    [Header("切り替えしきい値")]
    public float threshold1 = 50f;
    public float threshold2 = 100f;
    public float fukubreak = 150f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 cumulativeDelta = Vector2.zero;
    private Vector2 initialAnchoredPosition;

    private int lastIndex = 0;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        initialAnchoredPosition = rectTransform.anchoredPosition;
        // ★初期化: もしインスペクターで未指定なら、従来通り自分自身を対象にする（安全対策）
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
        cumulativeDelta.y = Mathf.Max(0, cumulativeDelta.y - eventData.delta.y);
        float distance = cumulativeDelta.y;

        rectTransform.anchoredPosition = initialAnchoredPosition - new Vector2(0, distance);

        // --- 破棄（非アクティブ）時の判定 ---
        if (distance > fukubreak)
        {
            if (gageDown != null)
            {
                // ★修正：専用イベントメソッドを呼び出して完全に外れた時のゲージを増やし、最大値フラグを更新
                gageDown.OnUnderwearBreak();
            }

            spriteChanger.Deactivate();

            // ★修正ポイント：指定オブジェクトがあればそれを、なければ自分を消す
            if (targetToDeactivate != null)
            {
                targetToDeactivate.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
            return;
        }

        int targetIndex;
        if (distance > threshold2) targetIndex = 2;
        else if (distance > threshold1) targetIndex = 1;
        else targetIndex = 0;

        if (targetIndex != lastIndex)
        {
            spriteChanger.UpdateSprite(targetIndex);

            if (gageDown != null)
            {
                // ★修正：専用イベントメソッドを呼び出してずらした時のゲージを増やし、最大値フラグを更新
                gageDown.OnUnderwearShift();
            }

            lastIndex = targetIndex;
        }

    }

}