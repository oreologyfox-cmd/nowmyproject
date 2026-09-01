using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;

public class WakeLevelSettiing : MonoBehaviour
{
    [Tooltip("このスクリプトが担当する起床時のレベル")]
    public int targetLevel;

    [System.Serializable]
    public class PartExpressionConfig
    {
        [Tooltip("部位の名前（例: Eye, Mouth, Effect など管理用）")]
        public string partName;

        [Tooltip("表情を変化させたいオブジェクトの SpriteResolver")]
        public SpriteResolver targetSpriteResolver;

        [Tooltip("Sprite Library Asset 内のカテゴリー名")]
        public string categoryName = "Face";

        [Header("Press Settings")]
        [Tooltip("クリック・ドラッグされている時の表情切り替え間隔（秒数）")]
        public float changeInterval = 3.0f;
        [Tooltip("クリック・ドラッグされている時にランダム表示させたいラベル名")]
        public List<string> spriteLabels = new List<string>();

        [Header("Idle Settings")]
        [Tooltip("クリック・ドラッグされていない時の表情切り替え間隔（秒数）")]
        public float idleChangeInterval = 5.0f;
        [Tooltip("クリック・ドラッグされていない時にランダム表示させたいラベル名")]
        public List<string> idleSpriteLabels = new List<string>();

        // 部位ごとに個別に持つタイマーと状態管理
        [HideInInspector] public float pressTimer = 0f;
        [HideInInspector] public float idleTimer = 0f;
    }

    [Tooltip("このレベルで制御するパーツの一覧（目、口、エフェクトなど）")]
    public List<PartExpressionConfig> partsConfigs = new List<PartExpressionConfig>();
}
