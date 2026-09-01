using UnityEngine;
using UnityEngine.UI;

// Imageコンポーネントの自動追加・削除防止を保証する
[RequireComponent(typeof(Image))]
public class OmanTinChange : MonoBehaviour
{
    [Header("スプライトリスト (0:通常, 1~3:段階, 4:最大)")]
    [SerializeField] private Sprite[] sprites;

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();

        // 配列がヌルでなく、要素が存在する場合に初期化
        if (sprites != null && sprites.Length > 0)
        {
            image.sprite = sprites[0];
        }
    }

    /// <summary>
    /// 指定したインデックスのスプライトに切り替えます
    /// </summary>
    public void UpdateSprite(int index)
    {
        if (sprites == null || index < 0 || index >= sprites.Length) return;

        if (image.sprite != sprites[index])
        {
            image.sprite = sprites[index];
        }
    }

    /// <summary>
    /// オブジェクトを非アクティブにします
    /// </summary>
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
