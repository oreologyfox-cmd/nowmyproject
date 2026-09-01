using UnityEngine;
using UnityEngine.EventSystems;

// UIオブジェクト（ImageやButton）にアタッチして使用します
public class IkuButtonParts : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private IkuGageDown ikuGageDown; // 管理クラスへの参照
    [SerializeField] private SlBodyPartType partType;  // このボタンが担当する部位

    /// <summary>
    /// ボタンが押された瞬間に呼ばれる
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (ikuGageDown != null)
        {
            ikuGageDown.SetPartState(partType, true);
        }
    }

    /// <summary>
    /// ボタンから指やマウスが離れた瞬間に呼ばれる
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (ikuGageDown != null)
        {
            ikuGageDown.SetPartState(partType, false);
        }
    }

    // インスペクターで設定し忘れた場合の簡易自動取得
    private void Reset()
    {
        if (ikuGageDown == null)
        {
            ikuGageDown = FindFirstObjectByType<IkuGageDown>();
        }
    }
}