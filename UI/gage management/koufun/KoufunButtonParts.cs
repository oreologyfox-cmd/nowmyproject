using UnityEngine;
using UnityEngine.EventSystems;

// EventTrigger（長押しや押しっぱなし検知）を利用するためインターフェースを実装
public class KoufunButtonParts : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] KoufunGageDown gageController; // ゲージ管理への参照

    [Header("Button Settings")]
    [SerializeField] BodyPartType targetPart; // インスペクターでどの部位か選ぶ

    // ボタンが押された時
    public void OnPointerDown(PointerEventData eventData)
    {
        if (gageController != null)
        {
            gageController.SetPartState(targetPart, true);
        }
    }

    // ボタンが離された時
    public void OnPointerUp(PointerEventData eventData)
    {
        if (gageController != null)
        {
            gageController.SetPartState(targetPart, false);
        }
    }

    // 念のためオブジェクトが非アクティブになったらリセット
    void OnDisable()
    {
        if (gageController != null)
        {
            gageController.SetPartState(targetPart, false);
        }
    }
}