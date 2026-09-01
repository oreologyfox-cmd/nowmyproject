using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClothingDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("システム参照")]
    [Tooltip("ゲージを管理するコンポーネント")]
    public SleepGageDown gageDown;

    [Header("設定")]
    [Tooltip("ゲージを増加させる量")]
    [SerializeField] private float gaugeAmount = 0.2f;
    [Tooltip("ドラッグ終了時に元の位置に自動で戻すか")]
    [SerializeField] private bool returnToInitialPosition = true;

    [Tooltip("しきい値を超えたときの移動先座標（UIのAnchored Position）")]
    [SerializeField] private Vector2 targetPosition;

    [Header("初期化オブジェクト（複数指定可能）")]
    [Tooltip("初期状態（Start時）で【表示（アクティブ）】にするオブジェクトのリスト")]
    [SerializeField] private List<GameObject> initialCloseObjects = new List<GameObject>();

    [Header("ドラッグ連動オブジェクト（複数指定可能）")]
    [Tooltip("引っ張った時（インデックス1）に【表示】するオブジェクトのリスト")]
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    [Tooltip("引っ張った時（インデックス1）に【非表示】にするオブジェクトのリスト")]
    [SerializeField] private List<GameObject> objectsToDeactivate = new List<GameObject>();

    [Header("切り替えしきい値")]
    [Tooltip("オブジェクトを切り替えるX軸の移動距離")]
    public float threshold1 = 50f;

    private RectTransform rectTransform;
    private Vector2 initialAnchoredPosition;
    private int currentIndex = 0;

    // 他のクラスから参照用（プロパティ）
    public int CurrentIndex => currentIndex;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        if (rectTransform != null)
        {
            // アクティブになった瞬間の位置を初期位置として記憶
            initialAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

    void Start()
    {
        // 初期状態のオブジェクトを一括でアクティブにする
        foreach (var obj in initialCloseObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // 開始時は初期状態（インデックス0の状態）として連動オブジェクトを配置
        SetObjectStates(false);
    }

    void OnDisable()
    {
        // オブジェクトが破棄（Destroy）されようとしている時は処理をスキップ
        if (this == null || !gameObject) return;

        // 自身が非アクティブになった時、引っ張った時に出していたオブジェクトを安全に非表示にする
        foreach (var obj in objectsToActivate)
        {
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        // UIを指やマウスの動きに合わせてドラッグ移動させる
        rectTransform.anchoredPosition += eventData.delta;

        // 初期位置からのX軸の移動距離を算出
        float distance = rectTransform.anchoredPosition.x - initialAnchoredPosition.x;

        // しきい値の判定
        int targetIndex = distance > threshold1 ? 1 : 0;

        // インデックスが変化した瞬間だけ処理
        if (targetIndex != currentIndex)
        {
            currentIndex = targetIndex;

            if (gageDown != null)
            {
                gageDown.AddGauge(gaugeAmount);
            }

            // ドラッグ状態（currentIndex == 1）に応じてオブジェクトを一括切り替え
            SetObjectStates(currentIndex == 1);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        // しきい値を超えて引っ張られた状態なら、指定した目標座標に移動させる
        if (currentIndex == 1)
        {
            rectTransform.anchoredPosition = targetPosition;
        }
        else
        {
            // しきい値を超えていなければ、設定に応じて初期位置に戻す
            if (returnToInitialPosition)
            {
                rectTransform.anchoredPosition = initialAnchoredPosition;
            }
        }
    }

    /// <summary>
    /// ドラッグ状態に応じて、登録されたすべてのオブジェクトの表示・非表示を切り替える
    /// </summary>
    private void SetObjectStates(bool isDragged)
    {
        // 引っ張った時に【表示】したいオブジェクト群の制御
        foreach (var obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(isDragged);
        }

        // 引っ張った時に【非表示】にしたいオブジェクト群の制御
        foreach (var obj in objectsToDeactivate)
        {
            if (obj != null) obj.SetActive(!isDragged);
        }
    }
}
