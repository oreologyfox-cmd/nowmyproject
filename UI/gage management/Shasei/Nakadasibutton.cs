using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Nakadasibutton : MonoBehaviour
{
    [Header("連動させるゲージマネージャー")]
    [SerializeField] private ShaseiiGageManagement gageManagement;

    [Header("オブジェクトを表示しておく時間（秒）")]
    [SerializeField] private float resetDelaySeconds = 3.0f;

    [Header("ゲージが満タンになった回数（確認用）")]
    [SerializeField] private int fillCount = 0;

    [Header("ロック中に【非表示】にするオブジェクト")]
    [SerializeField] private GameObject objectToHide;

    // ★ Nakadasibutton側で直接オブジェクトを指定できるように追加
    [Header("ロック中に【表示】するオブジェクト（演出用）")]
    [SerializeField] private GameObject objectToShow;

    private Button button;
    private bool isProcessing = false;

    public int CurrentFillCount => fillCount;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnSubmitButtonClicked);
        }

        // ゲーム起動時に、演出用のオブジェクトを初期非表示にしておく
        if (objectToShow != null)
        {
            objectToShow.SetActive(false);
        }
    }

    private void OnSubmitButtonClicked()
    {
        if (isProcessing || gageManagement == null || gageManagement.IsLocked) return;

        if (gageManagement.SquareImage != null && gageManagement.SquareImage.fillAmount >= 1f)
        {
            StartCoroutine(SubmitAndResetRoutine());
        }
    }

    private IEnumerator SubmitAndResetRoutine()
    {
        isProcessing = true;
        gageManagement.IsLocked = true;

        // 1. ロック開始時のオブジェクト切り替え
        if (objectToHide != null) objectToHide.SetActive(false);
        if (objectToShow != null) objectToShow.SetActive(true); // ★ 演出オブジェクトを表示

        fillCount++;

        // ハイライト点滅開始
        try
        {
            if (gageManagement != null && gageManagement.GageHilight != null)
            {
                gageManagement.GageHilight.IsBlinking = true;
            }
        }
        catch (System.Exception e) { Debug.LogError($"[ハイライト開始エラー] {e.Message}"); }

        // 指定時間（リアルタイム）待つ
        yield return new WaitForSecondsRealtime(resetDelaySeconds);

        // --- ここから終了処理（1行ずつ個別にエラーチェックし、連鎖フリーズを防ぐ） ---

        // 1. ハイライト停止の安全化
        try
        {
            if (gageManagement != null && gageManagement.GageHilight != null)
            {
                gageManagement.GageHilight.IsBlinking = false;
            }
        }
        catch (System.Exception e) { Debug.LogError($"[ハイライト停止エラー] {e.Message}"); }

        // 2. ゲージリセットの安全化
        try
        {
            if (gageManagement != null)
            {
                gageManagement.ResetGage();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ResetGage内部でクラッシュしています！] {e.Message}");
        }

        // 3. ロック解除時のオブジェクト切り替え（元に戻す）
        if (objectToHide != null) objectToHide.SetActive(true);
        if (objectToShow != null) objectToShow.SetActive(false); // ★ 演出オブジェクトを非表示

        // 4. フラグの初期化
        if (gageManagement != null) gageManagement.IsLocked = false;
        isProcessing = false;
    }
}
