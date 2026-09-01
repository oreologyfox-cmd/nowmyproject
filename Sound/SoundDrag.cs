using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoundMouth : MonoBehaviour
{
    [Header("対象のUIボタン（複数登録可能）")]
    [SerializeField] private List<Button> targetButtons = new List<Button>();

    [Header("共通で再生したい効果音の種類")]
    [SerializeField] private SEType commonSEType;

    [Header("再生インターバル（秒）")]
    [SerializeField] private float interval = 1.0f;

    private bool isDragging = false;
    private float timer = 0f;

    private void Start()
    {
        foreach (var button in targetButtons)
        {
            if (button == null) continue;

            GameObject buttonObj = button.gameObject;

            // 各ボタンにEventTriggerを動的に追加・取得
            EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = buttonObj.AddComponent<EventTrigger>();
            }

            // 全てのボタンに共通のドラッグイベントを登録
            AddEventTrigger(trigger, EventTriggerType.BeginDrag, (data) => OnBeginDrag());
            AddEventTrigger(trigger, EventTriggerType.Drag, (data) => OnDrag());
            AddEventTrigger(trigger, EventTriggerType.EndDrag, (data) => OnEndDrag());
        }
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, System.Action<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }

    // いずれかのボタンでドラッグが開始されたとき
    private void OnBeginDrag()
    {
        isDragging = true;
        RequestPlay(); // ドラッグ開始直後に1回目を再生
        timer = 0f;
    }

    private void OnDrag()
    {
        // 処理はUpdateで行うため空で問題ありません
    }

    // ドラッグが終了したとき（指やマウスを離したとき）
    private void OnEndDrag()
    {
        isDragging = false;

        // ドラッグをやめたら即座にSEを停止する
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopSE();
        }
    }

    private void Update()
    {
        if (!isDragging) return;

        timer += Time.deltaTime;

        // 1秒（interval）経過したら再生を要求
        if (timer >= interval)
        {
            RequestPlay();
            timer = 0f; // タイマーリセット
        }
    }

    private void RequestPlay()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(commonSEType);
        }
    }
}
