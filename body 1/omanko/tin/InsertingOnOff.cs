using UnityEngine;
using System.Collections.Generic;

public class InsertingOnOff : MonoBehaviour
{
    [Header("満タン時に表示したいオブジェクトの一覧")]
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();

    [Header("このオブジェクトがアクティブになった時に非表示にする一覧")]
    [SerializeField] private List<GameObject> hideOnActiveObjects = new List<GameObject>();

    [Header("初期化時に非表示にするか")]
    [SerializeField] private bool hideOnStart = true;

    // オブジェクトがアクティブ（有効化）になったときに呼び出される
    private void OnEnable()
    {
        HideSpecificObjects();
    }

    void Start()
    {
        if (hideOnStart)
        {
            SetObjectsActive(false);
        }
    }

    // 自身がアクティブになったときに、指定されたオブジェクトを非表示にする
    private void HideSpecificObjects()
    {
        if (hideOnActiveObjects == null) return;

        foreach (var obj in hideOnActiveObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false); // 強制的に非表示にする
                Debug.Log($"{gameObject.name} がアクティブになったため、{obj.name} を非表示にしました。");
            }
        }
    }

    public void OnGageChanged(float fillAmount)
    {
        // 0.95f以上の時は表示、それ未満の時は非表示にする
        if (fillAmount >= 0.95f)
        {
            SetObjectsActive(true);
        }
        else
        {
            SetObjectsActive(false);
        }
    }

    public void ResetObjects()
    {
        SetObjectsActive(false);
    }

    private void SetObjectsActive(bool isActive)
    {
        if (targetObjects == null) return;

        foreach (var obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
            else
            {
                Debug.LogWarning("リスト内に空（Null）の要素があります。");
            }
        }
    }
}
