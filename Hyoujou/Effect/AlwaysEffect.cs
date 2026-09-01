using UnityEngine;
using UnityEngine.U2D.Animation; // Sprite Resolver を使用するために必要

public class AlwaysEffect : MonoBehaviour
{
    [Header("Dependencies")]
    // 進行状況を管理している元のスクリプト
    [SerializeField] private KoufunGageDown gageLogic;
    // PSBのパーツを切り替えるための Sprite Resolver
    [SerializeField] private SpriteResolver targetResolver;

    [Header("Sprite Resolver Settings")]
    // インスペクターで設定した「Category」名（例: "Body", "Face" など）
    [SerializeField] private string targetCategory = "Body";

    // levelUp (0, 1, 2, 3) に対応する「Label」名（PSB内のレイヤー/スプライト名）を4つ登録します
    // Element 0 = "Level0", Element 1 = "Level1" ...
    [SerializeField] private string[] levelLabels = new string[4];

    private int lastLevel = -1;

    void Start()
    {
        // インスペクターで未設定の場合、同じオブジェクトなどから自動取得を試みる
        if (gageLogic == null) gageLogic = Object.FindFirstObjectByType<KoufunGageDown>();
        if (targetResolver == null) targetResolver = GetComponent<SpriteResolver>();

        UpdateSpriteResolver();
    }

    void Update()
    {
        if (gageLogic == null) return;

        // 元スクリプトの levelUp の値に変更があったときだけ処理を実行
        if (gageLogic.levelUp != lastLevel)
        {
            UpdateSpriteResolver();
        }
    }

    void UpdateSpriteResolver()
    {
        if (targetResolver == null || levelLabels == null) return;

        int currentLevel = gageLogic.levelUp;

        // 配列の範囲内（0〜3）であることをチェック
        if (currentLevel >= 0 && currentLevel < levelLabels.Length)
        {
            string labelName = levelLabels[currentLevel];

            // ラベル名が空でなければ、Sprite Resolver を切り替える
            if (!string.IsNullOrEmpty(labelName))
            {
                targetResolver.SetCategoryAndLabel(targetCategory, labelName);
                lastLevel = currentLevel; // 最後に適用したレベルを記録
            }
        }
    }
}
