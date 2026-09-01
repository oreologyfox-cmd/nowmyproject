using UnityEngine;
using UnityEngine.EventSystems;

// このスクリプトはUIオブジェクトにのみ貼れるようにする
public class CursorAttach : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // マウスがボタンに入ったとき
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetHoverCursor();
        }
    }

    // マウスがボタンから離れたとき
    public void OnPointerExit(PointerEventData eventData)
    {
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetDefaultCursor();
        }
    }

    // ボタンが非アクティブ（画面が閉じたときなど）にも一応リセットをかける
    private void OnDisable()
    {
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetDefaultCursor();
        }
    }
}
