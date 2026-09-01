using UnityEngine;

public class GameLock : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    // アニメーション開始時（ロック開始時）に呼び出す
    public void ShowObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("表示対象のオブジェクトが設定されていません。");
        }
    }

    // アニメーション終了時（ロック解除時）に呼び出す
    public void HideObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}
