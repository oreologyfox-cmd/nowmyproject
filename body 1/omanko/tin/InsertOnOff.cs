using UnityEngine;

public class InsertOnOff : MonoBehaviour
{
    [Header("監視対象のハンドラー")]
    [SerializeField] private OmanTinHandler tinHandler;

    [Header("targetIndexが3のときにONにするオブジェクト")]
    [SerializeField] private GameObject targetObject;

    // もし「3の時だけON、それ以外はOFF」ではなく「3になったらONにするだけ（OFFにはしない）」
    // などにしたい場合は、下のロジックを調整してください。

    private void OnEnable()
    {
        if (tinHandler != null)
        {
            // ハンドラーのインデックス変更イベントを購読（登録）
            tinHandler.OnIndexChanged += HandleIndexChanged;
        }
    }

    private void OnDisable()
    {
        if (tinHandler != null)
        {
            // 破棄・非アクティブ時にイベント解除（メモリリーク防止）
            tinHandler.OnIndexChanged -= HandleIndexChanged;
        }
    }

    /// <summary>
    /// インデックスが変更されたときに呼び出されるメソッド
    /// </summary>
    private void HandleIndexChanged(int newIndex)
    {
        if (targetObject == null) return;

        // newIndexが3であればtrue(ON)、それ以外ならfalse(OFF)にする
        if (newIndex == 3)
        {
            targetObject.SetActive(true);
        }
        else
        {
            targetObject.SetActive(false);
        }
    }
}
