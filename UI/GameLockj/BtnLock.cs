using UnityEngine;

public class BtnLock : MonoBehaviour
{
    [Header("監視するオブジェクト")]
    [SerializeField] private GameObject targetObject;

    [Header("連動して表示・非表示を切り替えたいオブジェクト")]
    [SerializeField] private GameObject attachedObject;

    void Update()
    {
        // オブジェクトが未設定の場合はエラーを防ぐため処理をスキップ
        if (targetObject == null || attachedObject == null) return;

        // ターゲットのアクティブ状態を取得
        bool isTargetActive = targetObject.activeSelf;

        // ターゲットの状態に合わせて、表示・非表示を切り替える
        if (attachedObject.activeSelf != isTargetActive)
        {
            attachedObject.SetActive(isTargetActive);
        }
    }
}
