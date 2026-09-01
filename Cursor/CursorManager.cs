using UnityEngine;
using UnityEngine.UI; // ボタンの検出に必要
using UnityEngine.EventSystems; // イベント制御に必要
using UnityEngine.SceneManagement; // シーン切り替え時の自動適用に必要

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("カーソル画像の設定")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D hoverCursor;

    [Header("クリック基準点")]
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // シーンが読み込まれたら自動でボタンにイベントを割り当てる設定
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetDefaultCursor();
        SetupAllButtonsInScene(); // 最初のシーンのボタンを設定
    }

    private void OnDestroy()
    {
        // メモリリーク防止のためイベントを解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーンが切り替わったときに実行される処理
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetDefaultCursor(); // カーソルを初期化
        SetupAllButtonsInScene(); // 新しいシーンのボタンを設定
    }

    // シーン内のすべてのボタンを探してホバーイベントを設定する
    private void SetupAllButtonsInScene()
    {
        // シーン内の「非アクティブ」を含むすべてのButtonコンポーネントを取得
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button button in allButtons)
        {
            // プレハブ（画面に実在しないデータ）は除外する
            if (button.gameObject.scene.name == null) continue;

            // すでにEventTriggerが付いているか確認、無ければ追加
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            // 二重登録を防ぐために一度トリガーをクリア
            trigger.triggers.Clear();

            // 1. マウスが入ったとき（PointerEnter）のイベント登録
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { SetHoverCursor(); });
            trigger.triggers.Add(entryEnter);

            // 2. マウスが出たとき（PointerExit）のイベント登録
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { SetDefaultCursor(); });
            trigger.triggers.Add(entryExit);
        }
    }

    public void SetHoverCursor()
    {
        if (hoverCursor != null)
        {
            Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
        }
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }
}
