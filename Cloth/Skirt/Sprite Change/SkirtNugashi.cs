using UnityEngine;
using UnityEngine.EventSystems;

public class SkirtNugashi : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public GameObject Parker;
    public GameObject Parkerback;

    [SerializeField] private SleepGageDown gageDown;

    void Start()
    {
        Parker.SetActive(true);
        Parkerback.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ロックチェックを削除
        Debug.Log("OK");

        if (gageDown != null)
        {
            gageDown.AddGauge(0.2f);
        }

        Destroy(Parker);
        Destroy(Parkerback);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }
}
