using UnityEngine;
using UnityEngine.U2D.Animation; // Sprite Resolverを使うために必要

public class HyoujouKirikae : MonoBehaviour
{
    // パーツごとのリゾルバーを登録
    [Header("Sprite Resolvers")]
    [SerializeField] private SpriteResolver eyeResolver;
    [SerializeField] private SpriteResolver eyebrowResolver;
    [SerializeField] private SpriteResolver mouthResolver;
    [SerializeField] private SpriteResolver noseResolver;

    // インスペクターから表情ごとの各パーツのラベル名を設定するための構造体
    [System.Serializable]
    public struct ExpressionLabels
    {
        public string eyeLabel;
        public string eyebrowLabel;
        public string mouthLabel;
        public string noseLabel;
    }

    [Header("Expression Settings")]
    [SerializeField] private ExpressionLabels Bad1;
    [SerializeField] private ExpressionLabels Bad2;
    [SerializeField] private ExpressionLabels Normal1;
    [SerializeField] private ExpressionLabels Normal2;

    /// <summary>
    /// 外部（NovelController）から呼ばれる表情切り替えメソッド
    /// </summary>
    public void ChangeExpression(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.Bad1:
                ApplyLabels(Bad1);
                break;
            case ExpressionType.Bad2:
                ApplyLabels(Bad2);
                break;
            case ExpressionType.Normal1:
                ApplyLabels(Normal1);
                break;
            case ExpressionType.Normal2:
                ApplyLabels(Normal2);
                break;
        }
    }

    /// <summary>
    /// 各Sprite Resolverにラベルを一括適用する
    /// </summary>
    private void ApplyLabels(ExpressionLabels labels)
    {
        // ヌルチェックをしてからラベルを切り替え
        if (eyeResolver != null) eyeResolver.SetCategoryAndLabel(eyeResolver.GetCategory(), labels.eyeLabel);
        if (eyebrowResolver != null) eyebrowResolver.SetCategoryAndLabel(eyebrowResolver.GetCategory(), labels.eyebrowLabel);
        if (mouthResolver != null) mouthResolver.SetCategoryAndLabel(mouthResolver.GetCategory(), labels.mouthLabel);
        if (noseResolver != null) noseResolver.SetCategoryAndLabel(noseResolver.GetCategory(), labels.noseLabel);
    }
}
