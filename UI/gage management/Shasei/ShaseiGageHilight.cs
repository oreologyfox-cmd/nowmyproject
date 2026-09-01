using UnityEngine;
using UnityEngine.UI;

public class ShaseiGageHilight : MonoBehaviour
{
    [Header("点滅スピード（値が大きいほど速い）")]
    [SerializeField] private float blinkSpeed = 5.0f;

    [Header("最小の明るさ（0.0〜1.0）")]
    [SerializeField] private float minAlpha = 0.2f;

    private Image gaugeImage;
    private bool isBlinking = false; // ★外部から点滅をコントロールするフラグ

    // ★外部から点滅状態を設定するためのプロパティ
    public bool IsBlinking
    {
        get => isBlinking;
        set
        {
            isBlinking = value;
            if (!isBlinking && gaugeImage != null)
            {
                // 点滅が終了したらアルファ値を1(最大)に戻す
                Color color = gaugeImage.color;
                color.a = 1.0f;
                gaugeImage.color = color;
            }
        }
    }

    void Start()
    {
        gaugeImage = GetComponent<Image>();
    }

    void Update()
    {
        if (gaugeImage == null) return;

        // ★点滅フラグがONのときだけ点滅させる
        if (isBlinking)
        {
            float sineWave = Mathf.Sin(Time.time * blinkSpeed);
            float alpha = Mathf.Lerp(minAlpha, 1.0f, (sineWave + 1.0f) / 2.0f);

            Color color = gaugeImage.color;
            color.a = alpha;
            gaugeImage.color = color;
        }
    }
}