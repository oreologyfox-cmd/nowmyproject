using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteButton : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public GameObject Object;
    public GameObject Button;

    [SerializeField] private SleepGageDown gageDown;

    void Start()
    {
        // 👇 変数が割り当てられている時だけ実行する（安全対策）
        if (Button != null)
        {
            Button.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} の 'Button' がインスペクターで未設定です。");
        }

        if (Object != null)
        {
            Object.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} の 'Object' がインスペクターで未設定です。");
        }
    }

    private void OnDisable()
    {
        // 既存のコード
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gageDown != null)
        {
            gageDown.AddGauge(0.2f);
        }

        if (Button != null)
        {
            Button.SetActive(false);
        }

        if (Object != null)
        {
            Object.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ中の処理
    }
}
