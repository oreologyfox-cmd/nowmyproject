using UnityEngine;

public class Modechange : MonoBehaviour
{
    public enum TouchMode
    {
        Hand,
        Mouth,
        Tin
    }

    public TouchMode CurrentMode => currentMode;

    [SerializeField] private TouchMode currentMode = TouchMode.Hand;

    // ★それぞれのモードの時「だけ」アクティブにしたいオブジェクトの配列
    [SerializeField] private GameObject[] handShowObjects;  // Handの時だけ表示
    [SerializeField] private GameObject[] mouthShowObjects; // Mouthの時だけ表示
    [SerializeField] private GameObject[] tinShowObjects;   // Tinの時だけ表示

    private void Start()
    {
        // ゲーム起動時の初期状態を反映
        UpdateObjectVisibility();
    }

    public void ChangeToHandMode()
    {
        currentMode = TouchMode.Hand;
        Debug.Log("手モードに変更しました");
        UpdateObjectVisibility();
    }

    public void ChangeToMouthMode()
    {
        currentMode = TouchMode.Mouth;
        Debug.Log("口モードに変更しました");
        UpdateObjectVisibility();
    }

    public void ChangeTinMode()
    {
        currentMode = TouchMode.Tin;
        Debug.Log("男性モードに変更しました");
        UpdateObjectVisibility();
    }

    // ★すべてのオブジェクトの表示・非表示を一括で更新する処理
    private void UpdateObjectVisibility()
    {
        // それぞれのモードと一致している時だけ true (アクティブ) になる
        SetObjectsActive(handShowObjects, currentMode == TouchMode.Hand);
        SetObjectsActive(mouthShowObjects, currentMode == TouchMode.Mouth);
        SetObjectsActive(tinShowObjects, currentMode == TouchMode.Tin);
    }

    // ★配列内のオブジェクトをまとめてアクティブ/非アクティブにする便利メソッド
    private void SetObjectsActive(GameObject[] list, bool isActive)
    {
        if (list == null) return;

        foreach (GameObject obj in list)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}