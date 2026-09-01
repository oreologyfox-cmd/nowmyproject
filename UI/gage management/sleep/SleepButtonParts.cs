using UnityEngine;
using UnityEngine.EventSystems;

// ★IDragHandlerを追加
public class SleepButtonParts : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("References")]
    [SerializeField] private SleepGageDown gageDownScript;

    [Header("Button Settings")]
    [SerializeField] private GageDownPartType partType;

    private bool isDraggingNow = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gageDownScript != null)
        {
            gageDownScript.SetPartState(partType, true, false);
        }
    }

    // ★ドラッグ中に毎フレーム呼ばれるメソッド
    public void OnDrag(PointerEventData eventData)
    {
        if (gageDownScript == null || isDraggingNow) return;

        // ドラッグが開始されたら、通常押しを解除してドラッグ状態をONにする
        isDraggingNow = true;
        gageDownScript.SetPartState(partType, false, false); // 通常押しOFF
        gageDownScript.SetPartState(partType, true, true);   // ドラッグON
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (gageDownScript != null)
        {
            // 両方の状態を安全にOFFにする
            gageDownScript.SetPartState(partType, false, false);
            gageDownScript.SetPartState(partType, false, true);
        }
        isDraggingNow = false;
    }

    private void OnDisable()
    {
        if (gageDownScript != null)
        {
            gageDownScript.SetPartState(partType, false, false);
            gageDownScript.SetPartState(partType, false, true);
        }
        isDraggingNow = false;
    }
}