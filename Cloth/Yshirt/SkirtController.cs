using UnityEngine;

public class SkirtController : MonoBehaviour
{
    [Header("対象のゲームオブジェクト")]
    [SerializeField] private GameObject skirt;
    [SerializeField] private GameObject YshirtClose;
    [SerializeField] private GameObject YshirtOpen;
    [SerializeField] private GameObject YshirtOpen2;
    [SerializeField] private GameObject BodyOpen;
    [SerializeField] private GameObject BodyClose;

    private void Start()
    {
        // インスペクターの設定漏れチェック
        if (skirt == null || YshirtClose == null || YshirtOpen == null ||
            YshirtOpen2 == null || BodyOpen == null || BodyClose == null)
        {
            Debug.LogError("必要なゲームオブジェクトがアタッチされていません。");
            return;
        }

        // 初期状態に基づいた切り替え処理
        UpdateStates();
    }

    private void Update()
    {
        // 毎フレーム状態をチェックして更新
        UpdateStates();
    }

    private void UpdateStates()
    {
        if (skirt == null || YshirtClose == null || YshirtOpen == null ||
            YshirtOpen2 == null || BodyOpen == null || BodyClose == null) return;

        // 1. skirt がアクティブの時
        if (skirt.activeSelf)
        {
            YshirtClose.SetActive(true);
            YshirtOpen.SetActive(false);
            YshirtOpen2.SetActive(false);
        }
        // 2. skirt が非アクティブの時
        else
        {
            YshirtClose.SetActive(false);

            // skirt(False) + BodyOpen(True) の時
            if (BodyOpen.activeSelf)
            {
                YshirtOpen2.SetActive(true);
            }
            else
            {
                YshirtOpen2.SetActive(false);
            }

            // skirt(False) + BodyClose(True) の時
            if (BodyClose.activeSelf)
            {
                YshirtOpen.SetActive(true);
            }
            else
            {
                YshirtOpen.SetActive(false);
            }
        }
    }
}
