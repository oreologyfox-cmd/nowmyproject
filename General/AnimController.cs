using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimController : MonoBehaviour
{
    [System.Serializable]
    public struct AnimationClip2D
    {
        public string name;
        public Sprite[] frames;
        public float fps;
        public bool loop;
    }

    [SerializeField] private AnimationClip2D[] clips;

    private SpriteRenderer spriteRenderer;
    private AnimationClip2D currentClip;
    private int currentFrame;
    private float timer;
    private bool isPlaying;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isPlaying) return;

        timer += Time.deltaTime;
        float frameRate = 1f / currentClip.fps;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame++;

            if (currentFrame >= currentClip.frames.Length)
            {
                if (currentClip.loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    isPlaying = false;
                    return;
                }
            }
            spriteRenderer.sprite = currentClip.frames[currentFrame];
        }
    }

    // 外部（プレイヤー制御スクリプトなど）からこれを呼んで再生
    public void Play(string clipName)
    {
        if (currentClip.name == clipName && isPlaying) return;

        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                currentClip = clip;
                currentFrame = 0;
                timer = 0f;
                isPlaying = true;
                spriteRenderer.sprite = currentClip.frames[0];
                return;
            }
        }
    }
}
