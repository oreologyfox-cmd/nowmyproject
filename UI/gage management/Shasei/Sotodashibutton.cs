using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sotodashibutton : MonoBehaviour
{
    [Header("連動させるゲージマネージャー")]
    [SerializeField] private ShaseiiGageManagement gageManagement;

    [Header("ゲージ満タンからリセットされるまでの遅延時間（秒）")]
    [SerializeField] private float resetDelaySeconds = 3.0f;

    [Header("ゲージが満タンになった回数（確認用）")]
    [SerializeField] private int fillCount = 0;

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

        if (gageManagement == null)
        {
            Debug.LogError("ShaseiiGageManagement がアタッチされていません！");
        }
    }

    private void OnSubmitButtonClicked()
    {
        if (isProcessing || gageManagement == null || gageManagement.IsLocked) return;

        if (gageManagement.SquareImage != null && gageManagement.SquareImage.fillAmount >= 1f)
        {
            StartCoroutine(SubmitAndResetRoutine());
        }
        else
        {
            Debug.Log("ゲージがまだ満タンではありません。");
        }
    }

    private IEnumerator SubmitAndResetRoutine()
    {
        isProcessing = true;
        gageManagement.IsLocked = true;

        fillCount++;
        Debug.Log($"ゲージ消費を実行！ 通算回数: {fillCount}");

        if (gageManagement.GageHilight != null)
        {
            gageManagement.GageHilight.IsBlinking = true;
        }

        yield return new WaitForSeconds(resetDelaySeconds);

        if (gageManagement.GageHilight != null)
        {
            gageManagement.GageHilight.IsBlinking = false;
        }

        gageManagement.ResetGage();

        gageManagement.IsLocked = false;
        isProcessing = false;
    }
}
