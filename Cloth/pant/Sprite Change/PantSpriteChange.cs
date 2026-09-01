using UnityEngine;
using UnityEngine.UI;

public class PantSpriteChange : MonoBehaviour
{
    [Header("スプライトリスト (0:通常, 1~3:段階, 4:最大)")]
    public Sprite[] sprites;

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        if (sprites.Length > 0) image.sprite = sprites[0];
    }

    public void UpdateSprite(int index)
    {
        if (sprites == null || index < 0 || index >= sprites.Length) return;

        if (image.sprite != sprites[index])
        {
            image.sprite = sprites[index];
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}