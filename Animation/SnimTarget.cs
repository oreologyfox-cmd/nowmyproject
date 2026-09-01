using System.Collections;
using UnityEngine;

public class AnimTarget : MonoBehaviour
{
    private Animator animator;
    private Coroutine timerCoroutine;
    private bool isPlaying = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 再生状態を直接セット（ON / OFF）
    public void SetPlayState(bool play)
    {
        StopTimerIfNeeded();
        isPlaying = play;
        ApplySpeed();
    }

    // 再生と停止を反転（トグル）
    public void TogglePlayState()
    {
        StopTimerIfNeeded();
        isPlaying = !isPlaying;
        ApplySpeed();
    }

    // 指定された秒数だけ再生
    public void PlayForSeconds(float seconds)
    {
        StopTimerIfNeeded();
        timerCoroutine = StartCoroutine(TimerRoutine(seconds));
    }

    private IEnumerator TimerRoutine(float seconds)
    {
        if (animator == null) yield break;

        isPlaying = true;
        ApplySpeed();

        yield return new WaitForSeconds(seconds);

        isPlaying = false;
        ApplySpeed();
        timerCoroutine = null;
    }

    private void ApplySpeed()
    {
        if (animator != null)
        {
            animator.speed = isPlaying ? 1f : 0f;
        }
    }

    private void StopTimerIfNeeded()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }
}
