using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mouthsprite : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [Header("画像の割り当て (要素数3が必要)")]
    [Header("0:上, 1:中央(初期), 2:下")]
    public Sprite[] sprites;

    [Header("感度（何ピクセル動いたら画像を切り替えるか）")]
    public float threshold = 50f;

    [Header("非表示にしたいオブジェクト (tkb)")]
    public GameObject targetObject;

    private Image image;
    private float cumulativeDeltaY = 0f; // Y軸のみを記録

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
        if (sprites == null || sprites.Length < 3) return;

        // Y軸の移動量のみを加算
        cumulativeDeltaY += eventData.delta.y;

        // 絶対値がしきい値を超えたか判定
        if (Mathf.Abs(cumulativeDeltaY) > threshold)
        {
            if (cumulativeDeltaY > 0)
                ChangeSprite(0); // 上
            else
                ChangeSprite(2); // 下

            cumulativeDeltaY = 0f; // カウントをリセット
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cumulativeDeltaY = 0f;
        ChangeSprite(1); // 中央（初期値）に戻す

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