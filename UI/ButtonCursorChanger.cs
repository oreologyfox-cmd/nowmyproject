using UnityEngine;

public class ButtonCursorChanger : MonoBehaviour
{
    // ボタンに乗った時に表示したいカーソル画像
    [SerializeField] private Texture2D customCursor;

    // カーソルのどの部分をポインターの先端（クリック位置）にするか
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    // マウスがボタンの上に入った時
    public void OnPointerEnter()
    {
        if (customCursor != null)
        {
            Cursor.SetCursor(customCursor, hotSpot, CursorMode.Auto);
        }
    }

    // マウスがボタンの上から離れた時
    public void OnPointerExit()
    {
        // 引数をすべて null / 初期値にすると、デフォルトの矢印カーソルに戻ります
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // シーン切り替えやボタン非アクティブ時にカーソルが戻らない現象を防ぐ
    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}