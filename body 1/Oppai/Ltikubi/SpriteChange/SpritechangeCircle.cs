using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SpritechangeCircle : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("変化させたいImageオブジェクトを割り当て")]
    public Image targetImage;

    [Header("画像の割り当て (要素数5が必要)")]
    [Header("0:上, 1:中央(初期), 2:下, 3:左, 4:右")]
    public Sprite[] sprites;

    [Header("感度（何ピクセル動いたら画像を切り替えるか）")]
    public float threshold = 50f;

    [Header("クリック・ドラッグ中に非アクティブにするオブジェクト")]
    public GameObject hideObject;

    private Vector2 cumulativeDelta = Vector2.zero;

    // 現在このスクリプトでドラッグ（クリック）中かを管理するフラグ
    private bool isDraggingNow = false;

    void Start()
    {
        // 初期画像のセット
        if (targetImage != null && sprites != null && sprites.Length > 1)
        {
            ChangeSprite(1);
        }
        else if (targetImage == null)
        {
            Debug.LogWarning($"{gameObject.name} の SpritechangeCircle: targetImage が割り当てられていません。");
        }
    }

    void Update()
    {
        // 毎フレーム監視：ドラッグ中にゲームがロックされたら、即座に強制中断する
        if (SleepLockManager.IsLocked && isDraggingNow)
        {
            ForceCancelDrag();
        }
    }

    // オブジェクトがクリック（タップ）された瞬間
    public void OnPointerDown(PointerEventData eventData)
    {
        // ロック中ならクリックを無効化
        if (SleepLockManager.IsLocked) return;

        isDraggingNow = true; // ドラッグ中フラグを立てる
        SetObjectActive(false);
    }

    // ドラッグ中
    public void OnDrag(PointerEventData eventData)
    {
        // ロック中、またはドラッグ開始が許可されていないなら処理しない
        if (SleepLockManager.IsLocked || !isDraggingNow) return;
        if (targetImage == null || sprites == null || sprites.Length < 5) return;

        cumulativeDelta += eventData.delta;

        float absX = Mathf.Abs(cumulativeDelta.x);
        float absY = Mathf.Abs(cumulativeDelta.y);

        // 縦方向の判定
        if (absY > threshold && absY > absX)
        {
            if (cumulativeDelta.y > 0)
                ChangeSprite(0); // 上
            else
                ChangeSprite(2); // 下

            cumulativeDelta = Vector2.zero; // カウントリセット
        }
        // 横方向の判定
        else if (absX > threshold && absX > absY)
        {
            if (cumulativeDelta.x > 0)
                ChangeSprite(4); // 右
            else
                ChangeSprite(3); // 左

            cumulativeDelta = Vector2.zero; // 横移動時もカウントをリセット
        }
    }

    // ドラッグが終了したとき（指やマウスを離したとき）
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingNow) return; // 既に強制終了していたら重ねて処理しない
        ResetToDefault();
    }

    // クリックやドラッグが完全に離された瞬間
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDraggingNow) return; // 既に強制終了していたら重ねて処理しない
        ResetToDefault();
    }

    // ロック発生時にUpdateから自動実行される強制中断処理
    private void ForceCancelDrag()
    {
        Debug.Log($"[{gameObject.name}] ロックを検知したため、実行中のドラッグを安全に強制中断しました。");
        ResetToDefault();
    }

    // 初期状態に戻す共通処理
    private void ResetToDefault()
    {
        isDraggingNow = false; // フラグをクリア
        cumulativeDelta = Vector2.zero;
        SetObjectActive(true);

        if (targetImage != null && sprites != null && sprites.Length > 1)
        {
            ChangeSprite(1); // 中央（初期状態）に戻す
        }
    }

    // オブジェクトの表示・非表示を切り替えるメソッド
    private void SetObjectActive(bool isActive)
    {
        if (hideObject != null)
        {
            if (hideObject.activeSelf != isActive) // 状態変化がある時だけSetActiveを呼ぶ（負荷軽減）
            {
                hideObject.SetActive(isActive);
            }
        }
    }

    // 安全にスプライトを切り替えるメソッド
    private void ChangeSprite(int index)
    {
        if (targetImage != null && sprites != null && index >= 0 && index < sprites.Length)
        {
            if (sprites[index] != null)
            {
                targetImage.sprite = sprites[index];
            }
        }
    }
}
