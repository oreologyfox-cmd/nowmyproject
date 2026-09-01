using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; // 強制リリース処理に必要です

public class IkuGageAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private IkuGageDown ikuGageDown;

    [Header("Animation Settings (Show)")]
    [SerializeField] private GameObject animationObject;
    [SerializeField] private float displayDuration = 3.0f;

    [Header("Animation Settings (Hide)")]
    [Tooltip("アニメーション演出中だけ非アクティブ（非表示）にしたいオブジェクトのリスト")]
    [SerializeField] private GameObject[] hideTargetObjects; // ★配列に変更

    [Header("Input Block Settings")]
    [Tooltip("画面全体を覆う、Raycast TargetがONになった画像オブジェクト")]
    [SerializeField] private GameObject inputBlockerObject;

    private bool isPlaying = false;

    // 外部（IkuGageDownなど）から参照し、trueの時だけゲージを増やすように制限します
    public bool CanInput => !isPlaying;

    void Start()
    {
        if (animationObject != null)
        {
            animationObject.SetActive(false);
        }

        if (inputBlockerObject != null)
        {
            inputBlockerObject.SetActive(false);
        }

        if (ikuGageDown == null)
        {
            ikuGageDown = GetComponent<IkuGageDown>();
        }
    }

    void Update()
    {
        if (ikuGageDown == null) return;

        if (isPlaying)
        {
            return;
        }

        // ゲージが1（満タン）かつ、まだアニメーションが再生されていない時
        if (ikuGageDown.time >= 1.0f && !isPlaying)
        {
            StartCoroutine(PlayAnimationRoutine());
        }
    }

    private IEnumerator PlayAnimationRoutine()
    {
        isPlaying = true;

        // クリックやドラッグの進行中入力を強制リリース
        ReleaseActiveTouches();

        // 演出が始まったら、ゲージを制御しているスクリプト自体を一時停止する
        if (ikuGageDown != null)
        {
            ikuGageDown.enabled = false;
        }

        if (inputBlockerObject != null) inputBlockerObject.SetActive(true);
        if (animationObject != null) animationObject.SetActive(true);

        // ★演出開始：登録されたすべてのオブジェクトを非アクティブにする
        SetTargetsActive(false);

        yield return new WaitForSecondsRealtime(displayDuration);

        if (animationObject != null) animationObject.SetActive(false);

        // ★演出終了：登録されたすべてのオブジェクトをアクティブ（再表示）に戻す
        SetTargetsActive(true);

        // ゲージをリセットする前に、スクリプトを再起動して動けるようにする
        if (ikuGageDown != null)
        {
            ikuGageDown.enabled = true;
        }

        ResetOriginalGage();

        if (inputBlockerObject != null) inputBlockerObject.SetActive(false);

        isPlaying = false;
    }

    /// <summary>
    /// 対象のオブジェクト群の表示・非表示を一括で切り替えます
    /// </summary>
    private void SetTargetsActive(bool isActive)
    {
        if (hideTargetObjects == null) return;

        foreach (GameObject target in hideTargetObjects)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }
    }

    /// <summary>
    /// 現在進行中のすべてのクリック・ドラッグ入力を強制的にリセット・中断します。
    /// </summary>
    private void ReleaseActiveTouches()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            EventSystem.current.RaycastAll(pointerData, new System.Collections.Generic.List<RaycastResult>());
        }
    }

    private void ResetOriginalGage()
    {
        if (ikuGageDown != null)
        {
            ikuGageDown.AddGauge(-1.0f);
            ikuGageDown.Start();
        }
    }

    // 演出の途中でシーンが切り替わったりオブジェクトが消された場合の安全対策
    private void OnDestroy()
    {
        if (isPlaying)
        {
            SetTargetsActive(true);
        }
    }
}
