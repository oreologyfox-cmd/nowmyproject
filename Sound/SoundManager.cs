using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --- 1. 効果音の名前を定義 ---
public enum SEType
{
    shasei,
    Ashasei,
    sex,
    teman,
    fuku,
    Rip,
    Tekoki,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public struct SEData
    {
        public SEType type;      // 列挙型
        public AudioClip clip;   // 音声ファイル
    }

    [Header("再生用のオーディオソース")]
    [SerializeField] private AudioSource seSource;

    [Header("効果音のリスト（名前とファイルをセットで登録）")]
    [SerializeField] private List<SEData> seList = new List<SEData>();

    // 検索を高速化するための辞書（Dictionary）
    private Dictionary<SEType, AudioClip> seDictionary = new Dictionary<SEType, AudioClip>();

    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDictionary(); // 辞書の初期化
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // インスペクターで設定したデータを辞書に変換する
    private void InitDictionary()
    {
        foreach (var data in seList)
        {
            if (data.clip == null) continue;

            if (!seDictionary.ContainsKey(data.type))
            {
                seDictionary.Add(data.type, data.clip);
            }
            else
            {
                Debug.LogWarning($"SoundManager: {data.type} が重複して登録されています。");
            }
        }
    }

    /// <summary>
    /// 効果音を名前（SEType）で再生する（再生は1回だけ、重複させない）
    /// </summary>
    public void PlaySE(SEType type)
    {
        // すでに何かしらの音が再生中なら、重ねて再生せずに無視する
        if (seSource != null && seSource.isPlaying)
        {
            return;
        }

        if (seDictionary.TryGetValue(type, out AudioClip clip))
        {
            // PlayOneShotではなく通常のPlayで1回だけ再生
            seSource.clip = clip;
            seSource.loop = false;
            seSource.Play();
        }
        else
        {
            Debug.LogWarning($"SoundManager: {type} に対応するAudioClipが見つかりません。");
        }
    }

    /// <summary>
    /// 現在再生中のSEを停止する
    /// </summary>
    public void StopSE()
    {
        if (seSource != null)
        {
            seSource.Stop();
        }
    }
    /// <summary>
    /// アニメーションイベントからSEを再生するためのメソッド
    /// </summary>
    /// <param name="typeName">SETypeの文字列（例: "shasei", "sex"）</param>
    public void PlaySEFromAnimation(string typeName)
    {
        // 文字列からenum（SEType）に変換を試みる
        if (System.Enum.TryParse(typeName, out SEType type))
        {
            PlaySE(type);
        }
        else
        {
            Debug.LogWarning($"SoundManager: アニメーションから指定された '{typeName}' はSETypeに存在しません。");
        }
    }

}
