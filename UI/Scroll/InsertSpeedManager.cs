using UnityEngine;
using UnityEngine.UI; 

public class InsertSpeedManager : MonoBehaviour
{
[Header("連動させるスクロールバー")]
[SerializeField] private Scrollbar targetScrollbar; 

[Header("制御対象のゲージマネージャー")]
[SerializeField] private ShaseiiGageManagement gageManagement;

[Header("制御対象の睡眠ゲージマネージャー")]
[SerializeField] private SleepGageDown sleepGageDown;

[Header("制御対象の興奮ゲージマネージャー")]
[SerializeField] private KoufunGageDown koufunGageDown;

[Header("制御対象の絶頂(Iku)ゲージマネージャー")]
[SerializeField] private IkuGageDown ikuGageDown;

[Header("制御対象のAnimator")]
[SerializeField] private Animator targetAnimator;

[Header("射精ゲージ：最小の上昇速度（スクロールバーが0のとき）")]
[SerializeField] private float minSpeed = 0f;

[Header("射精ゲージ：最大の上昇速度（スクロールバーが1のとき）")]
[SerializeField] private float maxSpeed = 0.8f;

[Header("睡眠ゲージ：最小の速度倍率（スクロールバーが0のとき）")]
[SerializeField] private float minSleepSpeedMult = 0f;

[Header("睡眠ゲージ：最大の速度倍率（スクロールバーが1のとき）")]
[SerializeField] private float maxSleepSpeedMult = 2.0f;

[Header("興奮ゲージ：最小の速度倍率（スクロールバーが0のとき）")]
[SerializeField] private float minKoufunSpeedMult = 0f;

[Header("興奮ゲージ：最大の速度倍率（スクロールバーが1のとき）")]
[SerializeField] private float maxKoufunSpeedMult = 2.0f;

[Header("絶頂(Iku)ゲージ：最小の速度倍率（スクロールバーが0のとき）")]
[SerializeField] private float minIkuSpeedMult = 0f;

[Header("絶頂(Iku)ゲージ：最大の速度倍率（スクロールバーが1のとき）")]
[SerializeField] private float maxIkuSpeedMult = 1.5f;

[Header("Animatorの最小再生速度（スクロールバーが0のとき）")]
[SerializeField] private float minAnimSpeed = 0f;

[Header("Animatorの最大再生速度（スクロールバー1のとき）")]
[SerializeField] private float maxAnimSpeed = 1f;

void Start()
{
// 安全チェック
if (targetScrollbar == null || gageManagement == null || sleepGageDown == null || koufunGageDown == null || ikuGageDown == null)
{
Debug.LogError($"割り当てが不完全です！ Scrollbar:{targetScrollbar}, Gage:{gageManagement}, SleepGage:{sleepGageDown}, KoufunGage:{koufunGageDown}, IkuGage:{ikuGageDown}", this);
return;
}
// 【修正】開始時にスクロールバーの値を0にし、勝手にゲージが溜まるのを防ぐ
targetScrollbar.value = 0f;

// 初期値の反映（0fを反映）
UpdateGageSpeed(targetScrollbar.value);

// イベント登録
targetScrollbar.onValueChanged.AddListener(UpdateGageSpeed);

}

private void UpdateGageSpeed(float scrollbarValue)
{
// 1. 射精ゲージ速度の制御
if (gageManagement != null)
{
float newSpeed = Mathf.Lerp(minSpeed, maxSpeed, scrollbarValue);
SetPrivateField(gageManagement, "increaseSpeed", newSpeed);
}
// 2. 睡眠ゲージ速度の制御
if (sleepGageDown != null)
{
    float newSleepMult = Mathf.Lerp(minSleepSpeedMult, maxSleepSpeedMult, scrollbarValue);
    sleepGageDown.ExternalSpeedMultiplier = newSleepMult;
}

// 3. 興奮ゲージ速度の制御
if (koufunGageDown != null)
{
    float newKoufunMult = Mathf.Lerp(minKoufunSpeedMult, maxKoufunSpeedMult, scrollbarValue);
    koufunGageDown.ExternalSpeedMultiplier = newKoufunMult;
}

// 4. 絶頂(Iku)ゲージ速度の制御
if (ikuGageDown != null)
{
    float newIkuMult = Mathf.Lerp(minIkuSpeedMult, maxIkuSpeedMult, scrollbarValue);
    ikuGageDown.ExternalSpeedMultiplier = newIkuMult;
}

// 5. アニメーション速度の制御
if (targetAnimator != null)
{
    float newAnimSpeed = Mathf.Lerp(minAnimSpeed, maxAnimSpeed, scrollbarValue);
    targetAnimator.speed = newAnimSpeed;
}

}

private object SetPrivateField(object target, string fieldName, object value)
{
var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
if (field != null)
{
if (value != null) field.SetValue(target, value);
return field.GetValue(target);
}
return null;
}

void OnDestroy()
{
if (targetScrollbar != null)
{
targetScrollbar.onValueChanged.RemoveListener(UpdateGageSpeed);
}
}

}