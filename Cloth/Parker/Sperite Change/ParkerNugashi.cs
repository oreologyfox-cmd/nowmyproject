using UnityEngine;
using UnityEngine.EventSystems;

public class ParkerNugashi : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("Target Objects")]
    public GameObject Parker;
    public GameObject Parkerback;

    [Header("References")]
    [SerializeField] private SleepGageDown gageDown;

    void Start()
    {
        if (Parker != null) Parker.SetActive(true);
        if (Parkerback != null) Parkerback.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Parker Nugashi: OK");
        if (gageDown != null)
        {
            // 専用のイベントメソッドを呼び出してゲージを増加させ、最大値フラグも確実に更新する
            gageDown.OnParkerNugashi();
        }

        // パーカーオブジェクトを非表示にする
        if (Parker != null) Parker.SetActive(false);
        if (Parkerback != null) Parkerback.SetActive(false);

    }

    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ中の処理（必要に応じて記述）
    }

}