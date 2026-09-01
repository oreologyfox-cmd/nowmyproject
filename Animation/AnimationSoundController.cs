using UnityEngine;

public class AnimationSoundTrigger : MonoBehaviour
{
    // インスペクターから、このフレームで鳴らしたい音を指定する
    public AudioClip soundEffect;

    // Animation Event からこの関数を呼び出す
    public void TriggerAnimationSound()
    {
        if (soundEffect != null)
        {
            // あなたのAudioManagerの「再生関数」を呼び出す
            // 例：AudioManager.Instance.PlaySE(soundEffect); 
        }
    }
}