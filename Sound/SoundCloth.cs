using UnityEngine;
using System.Collections.Generic;

public class MultiTargetDisableDetector : MonoBehaviour
{
    [Header("再生したい共通の効果音")]
    [SerializeField] private SEType soundType = SEType.fuku;

    [Header("監視したいオブジェクトのリスト")]
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();

    // 各オブジェクトの前回の状態を記憶するための辞書
    private Dictionary<GameObject, bool> lastActiveStates = new Dictionary<GameObject, bool>();

    private void Start()
    {
        // 初期状態の記録
        foreach (var obj in targetObjects)
        {
            if (obj != null)
            {
                lastActiveStates[obj] = obj.activeSelf;
            }
        }
    }

    private void Update()
    {
        // リスト内のオブジェクトを順にチェック
        foreach (var obj in targetObjects)
        {
            if (obj == null) continue;

            // 辞書に未登録のオブジェクトがあれば初期化（動的な追加にも対応可能）
            if (!lastActiveStates.ContainsKey(obj))
            {
                lastActiveStates[obj] = obj.activeSelf;
                continue;
            }

            bool currentActiveState = obj.activeSelf;
            bool lastActiveState = lastActiveStates[obj];

            // 「前回はアクティブ」かつ「今回は非アクティブ」になった瞬間を検知
            if (lastActiveState && !currentActiveState)
            {
                PlaySound();
            }

            // 状態を更新
            lastActiveStates[obj] = currentActiveState;
        }
    }

    private void PlaySound()
    {
        if (!gameObject.scene.isLoaded) return;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(soundType);
        }
    }
}
