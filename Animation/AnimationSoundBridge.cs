using UnityEngine;

public class AnimationSoundBridge : MonoBehaviour
{
    /// <summary>
    /// アニメーションイベントから呼ばれる中継メソッド
    /// </summary>
    /// <param name="typeName">SETypeの文字列（例: "shasei"）</param>
    public void PlaySE(string typeName)
    {
        // シングルトン（SoundManager.Instance）を通じて音を鳴らす
        if (SoundManager.Instance != null)
        {
            // 文字列からenum（SEType）に変換する
            if (System.Enum.TryParse(typeName, out SEType type))
            {
                SoundManager.Instance.PlaySE(type);
            }
            else
            {
                Debug.LogWarning($"AnimationSoundBridge: '{typeName}' はSETypeに存在しません。");
            }
        }
        else
        {
            Debug.LogWarning("AnimationSoundBridge: SoundManagerインスタンスが見つかりません。");
        }
    }
}
