using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoppaiChange : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{

    [Header("画像の割り当て (要素数5が必要)")]
    [Header("0:上, 1:中央(初期), 2:下, 3:左, 4:右")]
    public Sprite[] sprites;

    [Header("感度（何ピクセル動いたら画像を切り替えるか）")]
    public float threshold = 50f;

    [Header("非表示にしたいオブジェクト (tkb)")]
    public GameObject targetObject;

    private Image image;
    private Vector2 cumulativeDelta = Vector2.zero;

    void Start()
    {
        image = GetComponent<Image>();
        if (sprites != null && sprites.Length > 1)
        {
            image.sprite = sprites[1];
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {

        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (sprites == null || sprites.Length < 5) return;

        cumulativeDelta += eventData.delta;

        float absX = Mathf.Abs(cumulativeDelta.x);
        float absY = Mathf.Abs(cumulativeDelta.y);

        if (absY > threshold && absY > absX)
        {
            if (cumulativeDelta.y > 0)
                ChangeSprite(0);
            else
                ChangeSprite(2);

            cumulativeDelta = Vector2.zero;
        }
        else if (absX > threshold && absX > absY)
        {
            if (cumulativeDelta.x > 0)
                ChangeSprite(4);
            else
                ChangeSprite(3);

            cumulativeDelta = Vector2.zero;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        cumulativeDelta = Vector2.zero;
        ChangeSprite(1);

        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    void ChangeSprite(int index)
    {
        if (index >= 0 && index < sprites.Length)
        {
            image.sprite = sprites[index];
        }
    }
}